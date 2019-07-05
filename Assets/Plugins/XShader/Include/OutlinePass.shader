Shader "Hidden/OutlinePass"
{
	Properties
	{
		//OUTLINE
		_Outline("Outline Width", Range(0,10)) = 0.05
		_OutlineColor("Outline Color", Color) = (1, 0, 0, 1)
		[Toggle(SMOOTH_Z_ARTEFACTS)] SMOOTH_Z_ARTEFACTS("SMOOTH_Z_ARTEFACTS?", Float) = 0
		[Toggle(OUTLINE_CONST_SIZE)] OUTLINE_CONST_SIZE("OUTLINE_CONST_SIZE?", Float) = 0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass
		{
			Name "OUTLINE"
			
			Cull Front
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature SMOOTH_Z_ARTEFACTS
			#pragma shader_feature OUTLINE_CONST_SIZE
			#include "UnityCG.cginc"


			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f
			{
				float4 pos : POSITION;
			};
			
			sampler2D _MainTex;//虽然没有用图片，但这个还是必须有???
			float4 _MainTex_ST;
			float _Outline;
			fixed4 _OutlineColor;
			
			v2f vert (a2v v)
			{
				v2f o;
				
			#ifdef SMOOTH_Z_ARTEFACTS
				//Correct Z artefacts
				float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
				float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
				normal.z = -0.6;
				
				//Camera-independent size
				#ifdef OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					pos = pos + float4(normalize(normal),0) * _Outline * dist;
				#else
					pos = pos + float4(normalize(normal),0) * _Outline;
				#endif
				
			#else
				#ifdef OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline * dist);
				#else
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline);
				#endif
			#endif
				
				o.pos = mul(UNITY_MATRIX_P, pos);
				return o;
			}
			
			fixed4 frag (v2f IN) : COLOR
			{
				return _OutlineColor;
			}
		ENDCG
		}
		}
	
}
