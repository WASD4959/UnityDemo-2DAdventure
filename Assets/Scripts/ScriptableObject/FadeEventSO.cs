using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FadeEventSO")]
public class FadeEventSO : ScriptableObject {
    public UnityAction<Color, float, bool> OnEventRaised;

    public void RaiseEvent(Color target, float duration, bool fadeIn){
        OnEventRaised?.Invoke(target, duration, fadeIn);
    }

    /// <summary>
    /// 逐渐变黑
    /// </summary>
    /// <param name="duration"></param>
    public void FadeIn(float duration){
        RaiseEvent(Color.black, duration, true);
    }

    /// <summary>
    /// 逐渐透明
    /// </summary>
    /// <param name="duration"></param>
    public void FadeOut(float duration){
        RaiseEvent(Color.clear, duration, false);
    }
}