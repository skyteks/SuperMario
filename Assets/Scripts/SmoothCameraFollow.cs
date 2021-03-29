using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SmoothCameraFollow : MonoBehaviour
{
    public Transform objectToFollow;
    public float speed = 1f;

    public float upLimit = float.PositiveInfinity;
    public float downLimit = float.NegativeInfinity;
    public float leftLimit = float.NegativeInfinity;

    private Camera cam;
    public Bounds rect { get; private set; }

    private Vector3 currentUp;
    private Vector3 currentDown;
    private float halfWidth;
    private float halfHeight;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        if (objectToFollow == null)
        {
            enabled = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (cam == null)
        {
            Awake();
        }

        if (!float.IsInfinity(upLimit))
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(999999999f, upLimit, 0f), new Vector3(-999999999f, upLimit, 0f));
        }
        if (!float.IsInfinity(downLimit))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(999999999f, downLimit, 0f), new Vector3(-999999999f, downLimit, 0f));
        }
        if (!float.IsInfinity(leftLimit))
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(new Vector3(leftLimit, 999999999f, 0f), new Vector3(leftLimit, -999999999f, 0f));
        }

        Gizmos.color = Color.grey;
        Gizmos.DrawLine(cam.ViewportToWorldPoint(cam.rect.min), cam.ViewportToWorldPoint(cam.rect.max));

        if (objectToFollow != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(cam.transform.position, objectToFollow.position);
        }

        Gizmos.color = Color.red.ToWithA(0.2f);
        CalculateRect();
        Gizmos.DrawCube(rect.center, rect.size);
    }

    void Update()
    {
        CalculateRect();

        // clamp camera to borders
        Vector3 clamped = new Vector3(Mathf.Max(objectToFollow.position.x, leftLimit + halfWidth), Mathf.Clamp(objectToFollow.position.y, downLimit + halfHeight, upLimit - halfHeight), 0f);
        transform.position = Vector3.Lerp(transform.position, clamped, Time.deltaTime * speed);
    }

    private void CalculateRect()
    {
        // get camera view border values for clamping
        currentUp = cam.ViewportToWorldPoint(cam.rect.max);
        currentDown = cam.ViewportToWorldPoint(cam.rect.min);

        halfWidth = currentUp.x - cam.transform.position.x;
        halfHeight = currentUp.y - cam.transform.position.y;

        rect = new Bounds(cam.transform.position, new Vector2(halfWidth * 2f, halfHeight * 2f));
    }
}
