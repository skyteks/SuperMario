using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class adds some extension methods for the GameObject
/// </summary>
public static class InputAction_CallBackContext_Extension
{
    public static bool IsActive(this InputAction.CallbackContext callbackContext)
    {
        return callbackContext.started || callbackContext.performed;
    }
}
