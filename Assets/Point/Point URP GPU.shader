Shader "Graph/Point URP GPU"
{
    Properties {
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
	}
    
    SubShader
    {
        CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows addshadow
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
		#pragma editor_sync_compilation
		#pragma target 4.5


        struct Input
        {
            float3 worldPos;
        };

        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			StructuredBuffer<float3> _Positions;
		#endif

        float _Step;

		void ConfigureProcedural () {
			#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
				float3 position = _Positions[unity_InstanceID]; // Get the position of the current instance from the buffer. 
			
			/*The position is stored in the last column of the 4×4 transformation matrix, | s.x   0    0    p.x |
				                      while the scale is stored in the matrix diagonal.   |  0   s.y   0    p.y |
			                       The last component of the matrix is always set to 1.   |  0    0   s.z   p.z |
												  All other components are zero for us.   |  0    0    0	  1 | */	

				unity_ObjectToWorld = 0.0;
				unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
				unity_ObjectToWorld._m00_m11_m22 = _Step;
			#endif
		}
        
        float _Smoothness;
        
		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Albedo = saturate(input.worldPos * 0.5 + 0.5);
			surface.Smoothness = _Smoothness;
		}
        
        ENDCG
    }
    
    Fallback "Diffuse"
}

