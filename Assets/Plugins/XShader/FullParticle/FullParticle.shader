// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FullParticle" {
	Properties{
		_MainTex("Particle Texture", 2D) = "white" {}
		_Intensity("Intensity",Range(0,10)) = 1
		_Color("Color",color) = (1,1,1,1)
		_UVMove("UVMove",vector)=(0,0,0,0)
		_FogColor("FogColor",color) = (1,1,1,1)
		_InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0

		[HideInInspector] _BlendPreset("__BlendModePreset", Float) = 0.0
		[HideInInspector] _BlendModeSrc("__BlendModeSrc", Float) = 1.0
		[HideInInspector] _BlendModeDst("__BlendModeDst", Float) = 0.0
		[HideInInspector] _BlendOp("__BlendOp", Float) = 0.0

		[HideInInspector] _PreMulAlpha("_PreMulAlpha", Float) = 0.0
		[HideInInspector] _RevVertColor("_RevVertColor", Float) = 0.0
		[HideInInspector] _RevTexColor("_RevTexColor", Float) = 0.0
		[HideInInspector] _AlphaFromLength("_AlphaFromLength", Float) = 0.0
		[HideInInspector] _AlphaIntensity("AlphaIntensity", Range(0,10)) = 1.0

		[HideInInspector] _ZTest("__ZTest", Float) = 2.0
		[HideInInspector] _Cull("__Cull", Float) = 2.0
		[HideInInspector] _FogEnable("__FogEnable", Float) = 0.0
		[HideInInspector] _FogColorEnable("__FogColorEnable", Float) = 0.0
		[HideInInspector] _ColorMask("__ColorMask", Float) = 14

		[HideInInspector] _StencilRef ("__StencilRef", Float) = 0
		[HideInInspector] _StencilComp ("__Stencil Comparison", Float) = 0
		[HideInInspector] _StencilOp ("__Stencil Operation", Float) = 0
		[HideInInspector] _StencilWriteMask ("__StencilWriteMask", Float) = 255
		[HideInInspector] _StencilReadMask ("__StencilReadMask", Float) = 255
	}

		Category{

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane"}
			
			SubShader {
				Pass {
					Blend [_BlendModeSrc] [_BlendModeDst]
					BlendOp [_BlendOp]
					Cull [_Cull]
					ZTest [_ZTest]
					ColorMask [_ColorMask]
					Lighting Off
					ZWrite Off

					Stencil
					{
						Ref [_StencilRef]
						Comp [_StencilComp]
						Pass [_StencilOp] 
						ReadMask [_StencilReadMask]
						WriteMask [_StencilWriteMask]
						 //Fail stencilOperation
						 //ZFail stencilOperation
					}
				
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag

					#pragma shader_feature PreMulAlpha_On 
					#pragma shader_feature RevVertColor_On
					#pragma shader_feature RevTexColor_On 
					#pragma shader_feature AlphaFromLength_On 
					#pragma shader_feature Intensity_On 
					#pragma shader_feature Color_On
					#pragma shader_feature UVMove_On
					#pragma shader_feature Fog_On
					#pragma shader_feature FogColor_On
					#pragma multi_compile_fog
					#pragma multi_compile_particles

					#include "UnityCG.cginc"

					sampler2D _MainTex;
#if Intensity_On
					fixed _Intensity;
#endif
#if Color_On
					fixed4 _Color;
#endif
#if AlphaFromLength_On
					fixed _AlphaIntensity;
#endif
#if UVMove_On
					fixed4 _UVMove;
#endif
#if Fog_On
					fixed4 _FogColor;
#endif

#ifdef SOFTPARTICLES_ON
					sampler2D_float _CameraDepthTexture;
					float _InvFade;
#endif
					
					struct appdata_t {
						float4 vertex : POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
					};

					struct v2f {
						float4 vertex : POSITION;
						fixed4 color : COLOR;
						float2 texcoord : TEXCOORD0;
#if Fog_On
						UNITY_FOG_COORDS(1)
#endif
#ifdef SOFTPARTICLES_ON
							float4 projPos : TEXCOORD2;
#endif
					};
					
					float4 _MainTex_ST;

					v2f vert (appdata_t v)
					{
						v2f o;
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
						o.color = v.color;
#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos(o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
#endif
#if UVMove_On
						o.texcoord.xy += _UVMove.xy*_Time.x;
#endif
#if Color_On
						o.color *= _Color;
#endif
#if RevVertColor_On
						o.color.rgb = 1 - o.color.rgb;
#endif
#if Intensity_On
						o.color *= _Intensity;
#endif
#if Fog_On
						UNITY_TRANSFER_FOG(o, o.vertex);
#endif
						return o;
					}
					
					fixed4 frag (v2f i) : COLOR
					{
#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate(_InvFade * (sceneZ - partZ));
						i.color.a *= fade;
#endif

						fixed4 col= tex2D(_MainTex, i.texcoord);
						col*=i.color;
#if PreMulAlpha_On
						col*=i.color.a;
#endif
#ifdef RevTexColor_On
						col.rgb=1-col.rgb;
#endif

#ifdef AlphaFromLength_On
						col.a=length(col.rgb)*_AlphaIntensity;
#endif


#if Fog_On && FogColor_On
						UNITY_APPLY_FOG_COLOR(i.fogCoord, col, _FogColor);
#else
	#if Fog_On
						UNITY_APPLY_FOG(i.fogCoord, col);
	#endif
#endif
//col.r=col.a;
//col.g=col.a;
//col.b=col.a;
						return col;
					}

	
					ENDCG 
				}
			}
		}
			CustomEditor "FullParticleShaderGUI"
}
