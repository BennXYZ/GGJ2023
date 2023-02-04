using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerInput))]
public class ConvertInput : MonoBehaviour
{
    [SerializeField]
    InputManager inputManager;

    [SerializeField, Range(1,2)]
    int player = 1;

    [SerializeField]
    UnityEvent<Vector2> stickEvent;

    [SerializeField]
    UnityEvent<bool> buttonEvent;

    public void CheckStick(CallbackContext input)
    {
        if(inputManager.GetPlayerId(input.control.device) == player)
            stickEvent.Invoke(input.ReadValue<Vector2>());
    }

    public void CheckButton(CallbackContext input)
    {
        if (inputManager.GetPlayerId(input.control.device) == player)
        {
            if (input.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
                buttonEvent.Invoke(true);
            else if (input.phase == UnityEngine.InputSystem.InputActionPhase.Canceled)
                buttonEvent.Invoke(false);
        }
    }
}
