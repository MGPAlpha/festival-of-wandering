using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FestivalPreviewStage : PreviewSceneStage
{
    [MenuItem("Festival/Previewer")]
    public static void OpenPreviewer() {
        StageUtility.GoToStage(ScriptableObject.CreateInstance<FestivalPreviewStage>(), true);
    }

    GameObject player;
    GameObject mainCam;
    GameObject followCam;

    protected override bool OnOpenStage() {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
        GameObject cameraPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Main Camera.prefab");
        GameObject followCamPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FollowCam.prefab");
        player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, scene);
        mainCam = (GameObject)PrefabUtility.InstantiatePrefab(cameraPrefab, scene);
        followCam = (GameObject)PrefabUtility.InstantiatePrefab(followCamPrefab, scene);
        followCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = player.transform;
        player.GetComponent<Player>()._cco = followCam.GetComponent<CinemachineCameraOffset>();
        EditorSceneManager.SetActiveScene(scene);
        // EditorApplication.EnterPlaymode();
        return true;
    }
    
    protected override void OnCloseStage() {
        base.OnCloseStage();
    }

    protected override GUIContent CreateHeaderContent() {
        return new GUIContent("Previewer");
    }
}
