using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private List<Scene> m_AdditiveScenes = new List<Scene>();
     private Scene m_LastLoadedScene;
   public void LoadSceneAdditivelyByPath(string scenePath)
    {
        // Check if the scene at the given path is already loaded to prevent reloading
        Scene sceneToLoad = SceneManager.GetSceneByPath(scenePath);
        if (!sceneToLoad.IsValid())
        {
            // Track the scenes loaded additively by path
            if (!m_AdditiveScenes.Contains(sceneToLoad))
                m_AdditiveScenes.Add(sceneToLoad);
            
            StartCoroutine(LoadAdditiveScene(scenePath));
        }
        else
        {
            Debug.LogWarning($"[SceneLoader]: Scene at path {scenePath} is already loaded.");
        }
    }

    private IEnumerator LoadAdditiveScene(string scenePath)
    {
        if (string.IsNullOrEmpty(scenePath))
        {
            yield break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress;
            yield return null;
        }

        m_LastLoadedScene = SceneManager.GetSceneByPath(scenePath);
        SceneManager.SetActiveScene(m_LastLoadedScene);
    }

    public void UnloadSceneByPath(string scenePath)
    {
        
        Scene sceneToUnload = SceneManager.GetSceneByPath(scenePath);
        if (sceneToUnload.IsValid())
        {
            StartCoroutine(UnloadScene(sceneToUnload));
        }
    }
    // Unload by an explicit path immediately
    public void UnloadSceneImmediately(string scenePath)
    {
        Scene sceneToUnload = SceneManager.GetSceneByPath(scenePath);
        if (sceneToUnload.IsValid())
        {
            // Attempt to unload the scene immediately
            bool unloadSuccessful = SceneManager.UnloadScene(sceneToUnload);
            if (unloadSuccessful)
            {
                Debug.Log($"Scene {scenePath} unloaded successfully.");
                m_AdditiveScenes.Remove(sceneToUnload);
            }
            else
            {
                Debug.LogWarning($"Failed to unload scene {scenePath}.");
            }
        }
        else
        {
            Debug.LogWarning($"Scene at path {scenePath} is not valid or already unloaded.");
        }
    }

    private IEnumerator UnloadScene(Scene scene)
    {
        // Break if only have one scene loaded
        if (SceneManager.sceneCount <= 1)
        {
            Debug.Log("[SceneLoader: Cannot unload only loaded scene " + scene.name);
            yield break;
        }

        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }
}
