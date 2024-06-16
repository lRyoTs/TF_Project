using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputActionsManager
{
    public static PlayerControls inputActions = new PlayerControls();
    public static event Action<InputActionMap> actionMapChange;
    
    public static void ToggleActionMap(InputActionMap actionMap)
    {
        if (actionMap.enabled) return;

        inputActions.Disable();
        actionMapChange?.Invoke(actionMap);
        actionMap.Enable();
    }

    public static void DisableActionMap()
    {
        inputActions.Disable();
    }
}
