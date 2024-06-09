#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
StructuredBuffer<float3> _Positions;
#endif
float _Step;
void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) // Check if procedural instancing is enabled
	float3 position = _Positions[unity_InstanceID]; // Get the position from the buffer
	unity_ObjectToWorld = 0.0; // Initialize unity_ObjectToWorld
	unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0); // Set the position
	unity_ObjectToWorld._m00_m11_m22 = _Step; // Set the scale
	#endif
}
void ShaderGraphFunction_float (float3 In, out float3 Out){
	Out = In;
}
void ShaderGraphFunction_Half (half3 In, out half3 Out){
	Out = In;
}