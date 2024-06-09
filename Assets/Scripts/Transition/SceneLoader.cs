using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Transform playerTrans;
    public Vector3 firstPosition;

    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO firstLoadScene;

    [Header("事件广播")]
    public FadeEventSO FadeEvent;
    public VoidEventSO SceneLoadedEvent;
    private GameSceneSO currentLoadScene;

    private GameSceneSO sceneToLoad;
    private Vector3 posToGo;
    private bool fadeScreen;
    private bool isLoading;
    public float fadeDuration;

    private void Awake() {

    }
    
    private void Start() {
        NewGame();
    }

    private void OnEnable() {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }

    private void OnDisable() {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }

    private void NewGame(){
        sceneToLoad = firstLoadScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstPosition, true);
    }

    /// <summary>
    /// 场景加载请求
    /// </summary>
    /// <param name="sceneToLoad"></param>
    /// <param name="posToGo"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO sceneToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if(isLoading)
            return;

        isLoading = true;
        this.sceneToLoad = sceneToLoad;
        this.posToGo = posToGo;
        this.fadeScreen = fadeScreen;

        if(currentLoadScene != null)
            StartCoroutine(UnLoadPreviousScene());
        else
            LoadNewScene();
    }

    private IEnumerator UnLoadPreviousScene(){
        if(fadeScreen){
            // 场景变黑
            FadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);  
        yield return currentLoadScene.sceneReference.UnLoadScene();

        //关闭人物
        playerTrans.gameObject.SetActive(false);

        //加载新场景
        LoadNewScene();
    }

    private void LoadNewScene(){
        var loadingOperation = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);

        loadingOperation.Completed += OnLoadCompleted;
    }

    /// <summary>
    /// 场景加载结束
    /// </summary>
    /// <param name="handle"></param>
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        currentLoadScene = sceneToLoad;
        playerTrans.position = posToGo;
        playerTrans.gameObject.SetActive(true);

        if(fadeScreen){
            FadeEvent.FadeOut(fadeDuration);
        }

        isLoading = false;

        // if(currentLoadScene.sceneType == SceneType.Location)
        //场景加载完成后事件
        SceneLoadedEvent.RaiseEvent();
    }
}
