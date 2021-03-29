using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnScreentime : MonoBehaviour
{
    public MonoBehaviour[] components;

    private SmoothCameraFollow cameraFollow;

    void Awake()
    {
        cameraFollow = Camera.main.transform.GetComponent<SmoothCameraFollow>();

        foreach (MonoBehaviour component in components)
        {
            component.enabled = false;
        }
    }

    void LateUpdate()
    {
        if (cameraFollow.rect.Contains(transform.position))
        {
            foreach (MonoBehaviour component in components)
            {
                component.enabled = true;
            }
            Selfdestruct();
        }
    }

    public void Selfdestruct()
    {
        enabled = false;
        Destroy(this);
    }
}
