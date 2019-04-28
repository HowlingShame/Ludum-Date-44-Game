using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EckTechGames
{
	[InitializeOnLoad]
	public class AutoSaveExtension
	{
		// Static constructor that gets called when unity fires up.
		static AutoSaveExtension()
		{
			EditorApplication.playModeStateChanged += AutoSaveWhenPlaymodeStarts;
		}

		private static void AutoSaveWhenPlaymodeStarts(PlayModeStateChange state)
		{
			if(state == PlayModeStateChange.ExitingEditMode)
			{
				// Save the scene and the assets.
				EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
				AssetDatabase.SaveAssets();
			}
		}
	}
}