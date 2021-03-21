using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventForward : MonoBehaviour
{
    public UnityEvent<int> OnAnimationEvent;

    public void OnAnimationEventCallback(int i)
    {
        // invoke function on scripts that aren't on this gameObject
        OnAnimationEvent.Invoke(i);
    }
}
