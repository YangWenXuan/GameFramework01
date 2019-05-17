//#define MIGRATE_3_2_TO_4_1_AVAILABLE

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.Editor
{
    static class MenuItems
	{
		[MenuItem("frame8/OSA/Code reference")]
		public static void OpenOnlineCodeReference()
		{ Application.OpenURL("http://thefallengames.com/unityassetstore/optimizedscrollviewadapter/doc"); }

		[MenuItem("frame8/OSA/Manual and FAQ")]
		public static void OpenOnlineManual()
		{ Application.OpenURL("https://1drv.ms/w/s!AkSVvbsn1QdDgwDDexlkzmXVMCIb"); }

		[MenuItem("frame8/OSA/Quick start tutorial (YouTube)")]
		public static void OpenTutorial()
		{ Application.OpenURL("https://youtu.be/rcgnF16JybY"); }

#if MIGRATE_3_2_TO_4_1_AVAILABLE
		[MenuItem("frame8/OSA/Migrate from 3.2 to " + C.OSA_VERSION_STRING)]
		public static void MigrateFrom34To40()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				Debug.Log("OSA migration tool: Exit Play mode first");
				return;
			}
			if (EditorApplication.isCompiling)
			{
				Debug.Log("OSA migration tool: Wait for scripts to compile first");
				return;
			}

			bool proceed = EditorUtility.DisplayDialog(
				"OSA migration tool", 
				"This will find all components implementing IOSA in the scene and will try to migrate them to "+ C.OSA_VERSION_STRING + ". " +
					"This is just a first step to automate as much work as possible, but additional manual adjustments will be needed. Consult the Migration guide", 
				"I made a backup. Go!", 
				"Cancel"
			);
			if (proceed)
			{
				var obj = CEditor.GetAllGameObjectsInScene();
				int successful = 0, notSuccesful = 0;
				foreach (var go in obj)
				{
#pragma warning disable 0618 // Type or member is obsolete
					//var c = go.GetComponent<ISRIA>();
					// Retrieving the components implementing IOSA, since it's expected that the compile-time error given by SRIA was already fixed by
					// replacing it with OSA in code
					var c = go.GetComponent<IOSA>();
#pragma warning restore 0618 // Type or member is obsolete

					if (c != null)
					{
						var p = c.BaseParameters;
						var asMB = (c as MonoBehaviour);
						if (p == null)
						{
							Debug.LogError("OSA migration tool: params on " + go + " were null. Try again by selecting the game object before, to show its inspector", go);
							notSuccesful++;
						}
						else
						{
							var scrollRect = asMB.GetComponent<ScrollRect>();
							string error, additionalInfoOnSuccess;
							if (!p.MigrateFieldsToVersion4(scrollRect, out error, out additionalInfoOnSuccess))
							{
								Debug.LogError("OSA migration tool: Skipping '" + go + "': " + error);
								notSuccesful++;
							}
							else
							{
								scrollRect.enabled = false;
								Debug.Log(
									"OSA migration tool: ScrollRect on '" + go + "' was disabled. " +
									"You can safely remove it because it's not needed anymore starting with v4.0. " +
									"A similar interface is provided directly through OSA(events, callbacks, parameters etc.)\r\n" +
									"Additional info: " + additionalInfoOnSuccess
								);
								EditorUtility.SetDirty(asMB);
								EditorUtility.SetDirty(go);
								UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
								successful++;
							}
						}
					}
				}

				EditorUtility.DisplayDialog(
					"Done",
					successful + " successful migrations" + (notSuccesful > 0 ? (". " + notSuccesful + " not successful") : "") + ".\r\nConsult the Migration guide for the next steps", 
					"Ok"
				);
			}
		}
#endif

		[MenuItem("frame8/OSA/Changelog")]
		public static void OpenOnlineChangelog()
		{ Application.OpenURL("http://thefallengames.com/unityassetstore/optimizedscrollviewadapter/Changelog.txt"); }

		[MenuItem("frame8/OSA/Thank you!")]
		public static void OpenThankYou()
		{ Application.OpenURL("http://thefallengames.com/unityassetstore/optimizedscrollviewadapter/thankyou"); }

		[MenuItem("frame8/OSA/Ask us a question")]
		public static void AskQuestion()
		{ Application.OpenURL("https://forum.unity.com/threads/30-off-optimized-scrollview-adapter-listview-gridview.395224"); }

		[MenuItem("frame8/OSA/About")]
		public static void OpenAbout()
		{
			EditorUtility.DisplayDialog(
				"OSA " + C.OSA_VERSION_STRING,
				"May the result of our passion and hard work aid you along your journey in creating something marvellous!" +
				"\r\n\r\nOptimized ScrollView Adapter by The Fallen Games" +
				"\r\nlucian@thefallengames.com" +
				"\r\ngeorge@thefallengames.com",
				"Close"
			);
		}

		[MenuItem("CONTEXT/ScrollRect/Optimize with OSA")]
		static void OptimizeSelectedScrollRectWithOSA(MenuCommand command)
		{
			ScrollRect scrollRect = (ScrollRect)command.context;
			var validationResult = InitOSAWindow.Validate(true, scrollRect);
			// Manually checking for validation, as this provides richer info about the case when initialization is not possible
			if (!validationResult.isValid)
			{
				CWiz.ShowCouldNotExecuteCommandNotification(null);
				Debug.Log("OSA: Could not optimize '" + scrollRect.name + "': " + validationResult.reasonIfNotValid);
				return;
			}

			InitOSAWindow.Open(InitOSAWindow.Parameters.Create(validationResult));
		}

		[MenuItem("GameObject/UI/Optimized ScrollView (OSA)", false, 10)]
		static void CreateOSA(MenuCommand menuCommand)
		{

			string reasonIfNotValid;
			// Manually checking for validation, as this provides richer info about the case when creation is not possible
			if (!CreateOSAWindow.Validate(true, out reasonIfNotValid))
			{
				CWiz.ShowCouldNotExecuteCommandNotification(null);
				Debug.Log("OSA: Could not create ScrollView on the selected object: " + reasonIfNotValid);
				return;
			}

			CreateOSAWindow.Open(new CreateOSAWindow.Parameters());
		}
	}
}
