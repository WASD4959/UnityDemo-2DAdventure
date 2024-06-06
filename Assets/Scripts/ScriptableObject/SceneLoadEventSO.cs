using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject {
    public UnityAction<GameSceneSO, Vector3, bool> LoadRequestEvent;

/// <summary>
/// 场景加载请求
/// </summary>
/// <param name="sceneToLoad">要加载的场景</param>
/// <param name="posToGo">场景内player的坐标</param>
/// <param name="fadeScreen">是否渐入渐出</param>
    public void RaiseLoadRequestEvent(GameSceneSO sceneToLoad, Vector3 posToGo, bool fadeScreen){
        LoadRequestEvent?.Invoke(sceneToLoad, posToGo, fadeScreen);
    }
}
