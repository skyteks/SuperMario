using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Billboard Behavior
/// </summary>
public class Billboard : MonoBehaviour
{
    [System.Flags]
    public enum UpdateMethods : int
    {
        Update = 1,
        LateUpdate = 2,
        FixedUpdate = 4,
        OnPreRender = 8,
        EndOfFrame = 16,
    }
    [EnumFlagsAttribute]
    public UpdateMethods updateMethods;
    private List<int> chosenMethods;
    private WaitForEndOfFrame waitForEndOfFrame;

    void Awake()
    {
        waitForEndOfFrame = new WaitForEndOfFrame();
    }

    void OnEnable()
    {
        StartCoroutine(Coroutine());
    }

    void OnValidate()
    {
        chosenMethods = ReturnSelectedElements();
    }

    void Update()
    {
        OnValidate();
        if (chosenMethods.Contains((int)UpdateMethods.Update))
        {
            Reallign();
        }
    }

    void LateUpdate()
    {
        OnValidate();
        if (chosenMethods.Contains((int)UpdateMethods.LateUpdate))
        {
            Reallign();
        }
    }

    void FixedUpdate()
    {
        OnValidate();
        if (chosenMethods.Contains((int)UpdateMethods.FixedUpdate))
        {
            Reallign();
        }
    }

    void OnPreRender()
    {
        OnValidate();
        if (chosenMethods.Contains((int)UpdateMethods.OnPreRender))
        {
            Reallign();
        }
    }

    private IEnumerator Coroutine()
    {
        for (; ; )
        {
            yield return waitForEndOfFrame;
            OnValidate();
            if (updateMethods == UpdateMethods.EndOfFrame)
            {
                Reallign();
            }
            yield return null;
        }
    }

    public void Reallign()
    {
        Quaternion oldRot = transform.rotation;
        transform.LookAt(Camera.main.transform, Vector3.up);
        transform.rotation = Quaternion.Euler(new Vector3(oldRot.eulerAngles.x, transform.rotation.eulerAngles.y, oldRot.eulerAngles.z));
    }

    private List<int> ReturnSelectedElements()
    {
        int lenght = System.Enum.GetValues(typeof(UpdateMethods)).Length;
        List<int> selectedElements = new List<int>();
        for (int i = 0; i < lenght; i++)
        {
            int layer = 1 << i;
            if (((int)updateMethods & layer) != 0)
            {
                selectedElements.Add(i);
            }
        }
        return selectedElements;
    }
}
