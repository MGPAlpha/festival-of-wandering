using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
   public string sceneName;

   public void LoadStage() {
       SceneManager.LoadScene(sceneName);
       GetComponent<UISoundLoader>().EndTrack();
   }
}
