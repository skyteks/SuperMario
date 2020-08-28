using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    private GameObject player;
    public Range xRange;
    public Range yRange;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate()
    {
        float x = xRange.Clamp(player.transform.position.x);
        float y = yRange.Clamp(player.transform.position.y);
        transform.position = new Vector3(x, y, transform.position.z);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 topLeft = new Vector3(xRange.min, yRange.max);
        Vector3 topRight = new Vector3(xRange.max, yRange.max);
        Vector3 bottomLeft = new Vector3(xRange.min, yRange.min);
        Vector3 bottomRight = new Vector3(xRange.max, yRange.min);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
