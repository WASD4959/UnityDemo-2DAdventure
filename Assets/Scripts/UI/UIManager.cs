using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStatBar playerStatBar;

    [Header("事件监听")]
    public CharacterEventSo healthEvent;
    public SceneLoadEventSO unLoadedSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO backToMenuEvent;
    public VoidEventSO gameWinEvent;
    public FloatEventSO syncVolumeEvent;

    [Header("广播")]
    public VoidEventSO PauseEvent;

    [Header("组件")]
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public GameObject restartBtn;
    public GameObject mobileTouch;
    public Button settingBtn;
    public GameObject pausePanel;
    public Slider volumeSlider;

    private void Awake() {
        #if UNITY_STANDALONE
            mobileTouch.SetActive(false);
        #endif

        settingBtn.onClick.AddListener(TogglePausePanel);
    }

    private void OnEnable() {
        healthEvent.OnEventRaised += OnHealthEvent;
        unLoadedSceneEvent.LoadRequestEvent += OnUnLoadedSceneEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
        gameWinEvent.OnEventRaised += OnGameWinEvent;
    }

    private void OnDisable() {
        healthEvent.OnEventRaised -= OnHealthEvent;
        unLoadedSceneEvent.LoadRequestEvent -= OnUnLoadedSceneEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
        gameWinEvent.OnEventRaised -= OnGameWinEvent;
    }

    private void OnSyncVolumeEvent(float amount)
    {
        volumeSlider.value = (amount + 80)/100;
    }

    private void OnLoadDataEvent()
    {
        gameOverPanel.SetActive(false);
        gameWinPanel.SetActive(false);
    }

    
    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restartBtn);
    }

    private void OnGameWinEvent()
    {
        gameWinPanel.SetActive(true);
    }

    private void OnUnLoadedSceneEvent(GameSceneSO sceneToGo, Vector3 posToGo, bool fadeIn)
    {
        var isMenu = sceneToGo.sceneType == SceneType.Menu;
        playerStatBar.gameObject.SetActive(!isMenu);
    }

    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChange(percentage);
        playerStatBar.OnPowerChange(character);
    }

    private void TogglePausePanel()
    {
        if(Time.timeScale == 1) PauseEvent.RaiseEvent();
        pausePanel.SetActive(!pausePanel.activeInHierarchy);
        Time.timeScale = 1 - Time.timeScale;
    }
}
 