using UnityEngine;

public class GPUGraph : MonoBehaviour {

	const int maxResolution = 1000;

	// Shader.PropertyToID returns the unique identifier for a shader property name
	static readonly int
		positionsId = Shader.PropertyToID("_Positions"), 
		resolutionId = Shader.PropertyToID("_Resolution"), 
		stepId = Shader.PropertyToID("_Step"),
		timeId = Shader.PropertyToID("_Time"), 
		transitionProgressId = Shader.PropertyToID("_TransitionProgress");

	[SerializeField]
	ComputeShader computeShader;

	[SerializeField]
	Material material;

	[SerializeField]
	Mesh mesh;

	[SerializeField, Range(10, maxResolution)]
	int resolution = 10;

	[SerializeField]
	FunctionLibrary.FunctionName function;

	public enum TransitionMode { Cycle, Random }

	[SerializeField]
	TransitionMode transitionMode;

	[SerializeField, Min(0f)]
	float functionDuration = 1f, transitionDuration = 1f;

	float duration;

	bool transitioning;

	FunctionLibrary.FunctionName transitionFunction;

	ComputeBuffer positionsBuffer;

	void OnEnable () {
		// Create a buffer to store the positions of the vertices
		positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4); 
	}

	void OnDisable () {
		// Release the buffer when the script is disabled
		positionsBuffer.Release();
		positionsBuffer = null;
	}

	void Update () {
		duration += Time.deltaTime;
		if (transitioning) {
			if (duration >= transitionDuration) {
				duration -= transitionDuration;
				transitioning = false;
			}
		}
		else if (duration >= functionDuration) {
			duration -= functionDuration;
			transitioning = true;
			transitionFunction = function;
			PickNextFunction();
		}

 		UpdateFunctionOnGPU();
	}

	void PickNextFunction () {
		function = transitionMode == TransitionMode.Cycle ? 
					FunctionLibrary.GetNextFunctionName(function) : 
					FunctionLibrary.GetRandomFunctionNameOtherThan(function);
	}

	void UpdateFunctionOnGPU () {
		float step = 2f / resolution; // 2 is the range of the graph
		computeShader.SetInt(resolutionId, resolution);
		computeShader.SetFloat(stepId, step);
		computeShader.SetFloat(timeId, Time.time);
		if (transitioning) {
			computeShader.SetFloat(transitionProgressId, Mathf.SmoothStep(0f, 1f, duration / transitionDuration));
		}

		var kernelIndex = (int)function + (int)(transitioning ? transitionFunction : function) * FunctionLibrary.FunctionCount;//kernel index is the sum of the function and the transition function
		computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);

		int groups = Mathf.CeilToInt(resolution / 8f); //Groups are the number of threads in a group
		computeShader.Dispatch(kernelIndex, groups, groups, 1); // 1 is the number of groups in the z dimension

		material.SetBuffer(positionsId, positionsBuffer);
		material.SetFloat(stepId, step);
		var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
		Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
	}
}