using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
//using DesignPatterns.Events;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    // [Tooltip("Fades on and off to black")]
        //[SerializeField] private ScreenFader m_ScreenFader;
        
    // Default loaded scene that serves as the entry point and does not unload
    private Scene _entryScene;

    // The previously loaded scene
    private Scene _lastLoadedScene;

    // Collection of scenes additively loaded
    private List<Scene> _additiveScenes = new List<Scene>();

    // Properties
    public Scene EntryScene => _entryScene;

    // MonoBehaviour event functions

    private void Start()
    {
        // Set scene 0 as the Bootloader/Bootstrapscene
        _entryScene = SceneManager.GetActiveScene();
    }

    // Load a scene additively by path (does not unload the last scene)
    public void LoadSceneAdditivelyByPath(string scenePath)
    {
        // Check if the scene at the given path is already loaded to prevent reloading
        Scene sceneToLoad = SceneManager.GetSceneByPath(scenePath);

        if (!sceneToLoad.IsValid())
        {
            // Track the scenes loaded additively by path
            if (!_additiveScenes.Contains(sceneToLoad))
                _additiveScenes.Add(sceneToLoad);
            
            StartCoroutine(LoadAdditiveScene(scenePath));
        }
        else
        {
            Debug.LogWarning($"[SceneLoader]: Scene at path {scenePath} is already loaded.");
        }
    }

    // Coroutine to load a scene asynchronously by scene path string in Additive mode,
    // keeps the original scene as the active scene.
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

        _lastLoadedScene = SceneManager.GetSceneByPath(scenePath);
        SceneManager.SetActiveScene(_lastLoadedScene);
    }

    // Unload by an explicit path
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
        // 指定されたパスからシーンを取得
        Scene sceneToUnload = SceneManager.GetSceneByPath(scenePath);

        // シーンが有効であるか確認
        if (sceneToUnload.IsValid())
        {
            // 非同期アンロードを開始
            StartCoroutine(UnloadSceneAsync(sceneToUnload));
        }
        else
        {
            Debug.LogWarning($"Scene at path {scenePath} is not valid or already unloaded.");
        }
    }

    // 非同期でシーンをアンロードするコルーチン
    private IEnumerator UnloadSceneAsync(Scene scene)
    {
        // アンロード処理を開始
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);

        if (asyncUnload == null)
        {
            Debug.LogWarning($"Failed to start unloading scene {scene.path}.");
            yield break;
        }

        // アンロードの進行状況を待機
        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        // アンロードが完了したらリストから削除
        Debug.Log($"Scene {scene.path} unloaded successfully.");
        _additiveScenes.Remove(scene);
    }

    // Coroutine to unload a specific Scene asynchronously
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
