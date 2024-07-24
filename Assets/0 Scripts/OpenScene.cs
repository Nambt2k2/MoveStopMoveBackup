#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenScenes : Editor {
	[MenuItem("Open Scene/Scene_Loading #1")]
	public static void OpenLoading() {
		OpenScene(NameScene.Loading);
	}

	[MenuItem("Open Scene/Scene_MoveStopMove #2")]
	public static void OpenMoveStopMove() {
		OpenScene(NameScene.MoveStopMove);
	}

	[MenuItem("Open Scene/Scene_ZombieCity #3")]
	public static void Open() {
		OpenScene(NameScene.ZombieCity);
	}

	static void OpenScene(NameScene sceneName) {
		EditorSceneManager.SaveOpenScenes();
		EditorSceneManager.OpenScene("Assets/5 Scenes/" + sceneName + ".unity");
	}
}
#endif
