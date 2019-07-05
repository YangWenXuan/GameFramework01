//
// AtsMobileVegatation.cs
//
// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;

namespace Game
{
		public class AtsMobileVegatation : MonoBehaviour
		{
				// These are only needed in case you are using the terrain engine version of the shader
			public	Color VertexLitTranslucencyColor = new Color (0.73f, 0.85f, 0.4f, 1);
				public		float ShadowStrength = 0.8f;
				// wind
				public			Vector4 Wind =new Vector4 (0.85f, 0.075f, 0.4f, 0.5f);
				public		float WindFrequency = 0.75f;
				public		float GrassWindFrequency = 1.5f;

				void  Start ()
				{
						Shader.SetGlobalColor ("_Wind", Wind);
						Shader.SetGlobalColor ("_GrassWind", Wind);
				}

				void Update(){
						// simple wind animation
						// var WindRGBA : Color = Wind *  ( (Mathf.Sin(Time.realtimeSinceStartup * WindFrequency) + Mathf.Sin(Time.realtimeSinceStartup * WindFrequency * 0.975) )   * 0.5 );

						Color WindRGBA  = Wind *  ( (Mathf.Sin(Time.realtimeSinceStartup * WindFrequency)));
						WindRGBA.a = Wind.w;
						Color GrassWindRGBA  = Wind *  ( (Mathf.Sin(Time.realtimeSinceStartup * GrassWindFrequency)));
						GrassWindRGBA.a = Wind.w;

						Shader.SetGlobalColor("_Wind", WindRGBA);
						Shader.SetGlobalColor("_GrassWind", GrassWindRGBA);

						//Shader.SetGlobalFloat("_Shininess;", VertexLitShininess);
				}


		}
}
