Shader "Debug-Stencil"
{
	Properties{
		_Color ("Tint", Color) = (1,1,1,1)

		__Stencil("Stencil ID(USE ME !!!)", Float) = 1
		//__StencilComp("Stencil Comparison(USE ME !!!)", Float) = 8
		//__StencilOp("Stencil Operation(USE ME !!!)", Float) = 2
		__StencilWriteMask("Stencil Write Mask(USE ME !!!)", Float) = 255
		__StencilReadMask("Stencil Read Mask(USE ME !!!)", Float) = 255

		//_ColorMask ("Color Mask", Float) = 15

	}

	SubShader{
		Tags{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
		}
		

		Stencil{
			Ref [__Stencil]
			Comp Equal
			Pass Keep
			ReadMask [__StencilReadMask]
			WriteMask [__StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Off
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGBA

		Pass{
			Name "Default"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			
			struct appdata_t{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			v2f vert(appdata_t IN){
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target{
				half4 color = IN.color;
				return color;
			}
			ENDCG
		}
	}
}
