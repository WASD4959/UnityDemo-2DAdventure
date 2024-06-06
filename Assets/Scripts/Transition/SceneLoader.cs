using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO firstLoadScene;
    [Header("事件广播")]
    public FadeEventSO FadeEvent;
    [SerializeField] private GameSceneSO currentLoadScene;

    private GameSceneSO sceneToLoad;
    private Vector3 posToGo;
    private bool fadeScreen;
    public float fadeDuration;

    private void Awake() {
        currentLoadScene = firstLoadScene;
        currentLoadScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
    }

    private void OnEnable() {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }

    private void OnDisable() {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }

    private void OnLoadRequestEvent(GameSceneSO sceneToLoad, Vector3 posToGo, bool fadeScreen)
    {
        this.sceneToLoad = sceneToLoad;
        this.posToGo = posToGo;
        this.fadeScreen = fadeScreen;

        if(currentLoadScene != null)
            StartCoroutine(UnLoadPreviousScene());
    }


    private IEnumerator UnLoadPreviousScene(){
        if(fadeScreen){
            // 场景变黑
            FadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);  
        yield return currentLoadScene.sceneReference.UnLoadScene();

        LoadNewScene();
        //*场景变透明
        FadeEvent.FadeOut(fadeDuration);
    }

    private void LoadNewScene(){
        sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        // TODO: player坐标改变
    }
}
