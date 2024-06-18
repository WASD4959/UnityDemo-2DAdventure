using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using System.IO;

[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    [Header("事件监听")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO LoadDataEvent;

    private List<ISaveable> saveableList = new List<ISaveable>();
    private Data saveData;
    private string jsonFolder;

    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        saveData = new Data();

        jsonFolder = Application.persistentDataPath + "/SAVE_DATA";

        ReadSavedData();
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

        // //Log savedCharacter positions;
        // foreach (var item in saveData.floatSavedData)
        // {
        //     Debug.Log(item.Key + "      " + item.Value);
        // }

        var resultPath = jsonFolder + "/data.sav";
        var jsonData = JsonConvert.SerializeObject(saveData);

        if(!File.Exists(jsonFolder)){
            Directory.CreateDirectory(jsonFolder);
        }

        File.WriteAllText(resultPath, jsonData);
    }
    
    public void Load(){
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }

    private void ReadSavedData(){
        var resultPath = jsonFolder + "/data.sav";

        if(File.Exists(resultPath)){
            var stringData = File.ReadAllText(resultPath);
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);
            saveData = jsonData;
        }
    }
}
