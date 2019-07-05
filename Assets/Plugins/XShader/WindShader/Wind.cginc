// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'


#include "TerrainEngine.cginc"

fixed _WindEdgeFlutter;
fixed _WindEdgeFlutterFreqScale;

inline float4 AnimateVertexWind(float4 pos, float3 normal, float4 animParams, float4 wind, float2 time)
{
	// animParams stored in color
	// animParams.x = branch phase
	// animParams.y = edge flutter factor
	// animParams.z = primary factor
	// animParams.w = secondary factor

	float fDetailAmp = 0.1f;
	float fBranchAmp = 0.3f;

	// Phases (object, vertex, branch)
	float fObjPhase = dot(unity_ObjectToWorld[3].xyz, 1);
	float fBranchPhase = fObjPhase + animParams.x;

	float fVtxPhase = dot(pos.xyz, animParams.y + fBranchPhase);

	// x is used for edges; y is used for branches
	float2 vWavesIn = time + float2(fVtxPhase, fBranchPhase);

	// 1.975, 0.793, 0.375, 0.193 are good frequencies
	float4 vWaves = (frac(vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193)) * 2.0 - 1.0);

	vWaves = SmoothTriangleWave(vWaves);
	float2 vWavesSum = vWaves.xz + vWaves.yw;

	// Edge (xz) and branch bending (y)
	float3 bend = animParams.y * fDetailAmp * normal.xyz;
	bend.y = animParams.w * fBranchAmp;
	pos.xyz += ((vWavesSum.xyx * bend) + (wind.xyz * vWavesSum.y * animParams.w)) * wind.w;

	// Primary bending
	// Displace position
	pos.xyz += animParams.z * wind.xyz;

	return pos;
}

float4 Wind(appdata_full v) {
	float			bendingFact = v.color.a;

	float4	wind;
	wind.xyz = mul((float3x3)unity_WorldToObject, _Wind.xyz);
	wind.w = _Wind.w  * bendingFact;

	float4	windParams = float4(0, _WindEdgeFlutter, bendingFact.xx);
	float 	windTime = _Time.y * float2(_WindEdgeFlutterFreqScale, 1);
	float4	mdlPos = AnimateVertexWind(v.vertex, v.normal, windParams, wind, windTime);
	return mdlPos;
}
