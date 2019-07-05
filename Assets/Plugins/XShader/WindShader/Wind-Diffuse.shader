Shader "XShader/Wind/Diffuse" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		_Wind("Wind params",Vector) = (1,1,1,1)
		_WindEdgeFlutter("Wind edge fultter factor", float) = 0.5
		_WindEdgeFlutterFreqScale("Wind edge fultter freq scale",float) = 0.5

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		CGPROGRAM


		//#pragma vertex vertWind
		#pragma surface surf Lambert fullforwardshadows  vertex:vertWind

		#include "Wind.cginc"
		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};


		void vertWind(inout appdata_full v)
		{
			v.vertex = Wind(v);
			//v.vertex = mul(UNITY_MATRIX_MVP, mdlPos);
		}


		void surf(Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			o.Alpha = c.a;
		}
		
		ENDCG
	}

	FallBack "Diffuse"
}
