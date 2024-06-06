using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public GameSceneSO sceneToGo;
    public SceneLoadEventSO loadEventSO;
    public Vector3 positionToGo;
    public void TriggerAction()
    {
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}
