using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public string sceneToSave;
    public Dictionary<string, SerializeVector3> characterPosDic = new Dictionary<string, SerializeVector3>();
    public Dictionary<string, float> floatSavedData = new Dictionary<string, float>();

    public void SaveGameScene(GameSceneSO savedScene){
        sceneToSave = JsonUtility.ToJson(savedScene);
        // Debug.Log(sceneToSave);
    }

    public GameSceneSO GetSavedScene()
    {
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();
        JsonUtility.FromJsonOverwrite(sceneToSave, newScene);
        return newScene;
    }
}

public class SerializeVector3{
    public float x, y, z;

    public SerializeVector3(Vector3 vec)
    {
        this.x = vec.x;
        this.y = vec.y;
        this.z = vec.z;
    }

    public Vector3 DeserializeVector3()
    {
        return new Vector3(x,y,z);
    }
}