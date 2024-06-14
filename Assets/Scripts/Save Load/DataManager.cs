using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    [Header("事件监听")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO LoadDataEvent;

    private List<ISaveable> saveableList = new List<ISaveable>();
    private Data saveData;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        saveData = new Data();
    }

    // // Test Load Function
    // private void Update(){
    //     if(Keyboard.current.lKey.wasPressedThisFrame)
    //         Load();
    // }

    private void OnEnable() {
        saveDataEvent.OnEventRaised += Save;
        LoadDataEvent.OnEventRaised += Load;
    }

    private void OnDisable() {
        saveDataEvent.OnEventRaised -= Save;
        LoadDataEvent.OnEventRaised -= Load;
    }

    public void RegisterSaveData(ISaveable saveable){
        if(!saveableList.Contains(saveable)){
            saveableList.Add(saveable);
        }
    }

    public void UnRegisterSaveData(ISaveable saveable){
        saveableList.Remove(saveable);
    }

    public void Save(){
        foreach (var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }

        //Log savedCharacter positions;
        foreach (var item in saveData.floatSavedData)
        {
            Debug.Log(item.Key + "      " + item.Value);
        }
    }
    
    public void Load(){
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }
}
