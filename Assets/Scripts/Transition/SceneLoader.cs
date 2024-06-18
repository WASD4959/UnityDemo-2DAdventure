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

public class SceneLoader : MonoBehaviour, ISaveable
{
    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;

    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGame;
    public VoidEventSO backToMenu;

    [Header("事件广播")]
    public FadeEventSO FadeEvent;
    public VoidEventSO SceneLoadedEvent;
    public SceneLoadEventSO unloadedSceneEvent;

    [Header("场景")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;
    private GameSceneSO currentLoadScene;
    private GameSceneSO sceneToLoad;
    private Vector3 posToGo;
    private bool fadeScreen;
    private bool isLoading;
    public float fadeDuration;

    private void Awake() {
       
    }
    
    private void Start() {
        loadEventSO.RaiseLoadRequestEvent(menuScene, menuPosition, true);
        // NewGame();
    }

    private void OnEnable() {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGame.OnEventRaised += NewGame;
        backToMenu.OnEventRaised += OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable() {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGame.OnEventRaised -= NewGame;
        backToMenu.OnEventRaised -= OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void OnBackToMenuEvent()
    {
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
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

        //广播事件调整血条显示
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad, posToGo, true);

        
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

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadScene);
    }

    public void LoadData(Data data)
    {
        var playerID = playerTrans.GetComponent<DataDefinition>().ID;
        if(data.characterPosDic.ContainsKey(playerID)){
            sceneToLoad = data.GetSavedScene();

            loadEventSO.RaiseLoadRequestEvent(sceneToLoad, data.characterPosDic[playerID].DeserializeVector3(), true);
        }
    }
}
