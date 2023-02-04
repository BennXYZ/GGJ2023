using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour
{
    List<PlayerEventAssignment> knownGamePads;

    public static InputManager Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InputSystem.onDeviceChange += OnDeviceStateChange;
        knownGamePads = new List<PlayerEventAssignment>();
        for (int i = 0; i < 4; i++)
        {
            knownGamePads.Add(new PlayerEventAssignment(i));
        }
        UpdateFoundControllers();
    }

    private void Update()
    {
        foreach (var entry in knownGamePads)
            entry.Update();
    }

    public Vector2 GetLeftJoystick(int playerId)
    {
        foreach(var entry in knownGamePads)
        {
            if (entry.PlayerId == playerId)
                return entry.LeftJoyStick;
        }
        return Vector2.zero;
    }

    public Vector2 GetRightJoystick(int playerId)
    {
        foreach (var entry in knownGamePads)
        {
            if (entry.PlayerId == playerId)
                return entry.RightJoyStick;
        }
        return Vector2.zero;
    }

    public void AssignButton(string button, int playerId, Action onButton)
    {
        foreach (var entry in knownGamePads)
        {
            if (entry.PlayerId == playerId)
            {
                switch (button)
                {
                    case "A":
                        entry.AssignAButton(onButton);
                        break;
                    case "B":
                        entry.AssignBButton(onButton);
                        break;
                    case "X":
                        entry.AssignXButton(onButton);
                        break;
                    case "Y":
                        entry.AssignYButton(onButton);
                        break;
                }
            }
        }
    }

    public void UnlinkButtons(int playerId)
    {

    }

    public void UnlinkButton(string button, int playerId)
    {

    }

    private void OnDeviceStateChange(InputDevice device, InputDeviceChange state)
    {
        switch (state)
        {
            case InputDeviceChange.Added:
                if (device is Gamepad)
                {
                    foreach (var entry in knownGamePads)
                        if (!entry.HasGamePad)
                            entry.AssignGamePad(device as Gamepad);
                }
                // New Device.
                break;
            case InputDeviceChange.Disconnected:
                if (device is Gamepad)
                {
                    foreach (var gamePad in knownGamePads)
                        if (gamePad.UsesGamepad(device as Gamepad))
                        {
                            gamePad.RemoveGamepad();
                            break;
                        }
                }
                // Device got unplugged.
                break;
        }
    }

    public int GetPlayerId(InputDevice gamePad)
    {
        if (gamePad is Gamepad)
            foreach (var device in knownGamePads)
                if (device.UsesGamepad(gamePad as Gamepad))
                {
                    return device.PlayerId;
                }
        return -1;
    }

    void UpdateFoundControllers()
    {
        foreach (Gamepad gamePad in Gamepad.all)
        {
            if(knownGamePads.Any(g => g.UsesGamepad(gamePad)))
            {
                continue;
            }
            foreach (var entry in knownGamePads)
                if (!entry.HasGamePad)
                    entry.AssignGamePad(gamePad);
        }
    }

    int GetEarliestFreeNumber()
    {
        foreach (var entry in knownGamePads)
            if (entry.HasGamePad)
                return entry.PlayerId;
        return -1;
    }

    public void HookUpPlayer(int playerId, Action aButtonEvent, Action bButtonEvent, Action xButtonEvent, Action yButtonEvent, 
        Action<Vector2> leftStickUpdateEvent, Action<Vector2> rightStickUpdateEvent)
    {
        PlayerEventAssignment entry = knownGamePads.FirstOrDefault(g => g.PlayerId == playerId);
        if(entry == null)
        {
            return;
        }
    }

    public class PlayerEventAssignment
    {
        private Gamepad gamePad;
        UnityEvent<bool> enabledChanged = new UnityEvent<bool>();

        readonly UnityEvent<bool> OnButtonA = new UnityEvent<bool>();
        readonly UnityEvent<bool> OnButtonB = new UnityEvent<bool>();
        readonly UnityEvent<bool> OnButtonX = new UnityEvent<bool>();
        readonly UnityEvent<bool> OnButtonY = new UnityEvent<bool>();
        readonly UnityEvent<Vector2> OnLeftStickUpdate = new UnityEvent<Vector2>();
        readonly UnityEvent<Vector2> OnRightStickUpdate = new UnityEvent<Vector2>();

        public Vector2 LeftJoyStick { get; private set; }
        public Vector2 RightJoyStick { get; private set; }

        public int PlayerId { get; private set; }

        bool enabled = true;
        bool Enabled
        {
            get => enabled;
            set
            {
                if(value != Enabled)
                {
                    enabled = value;
                    enabledChanged.Invoke(Enabled);
                }
            }
        }

        public bool HasGamePad => gamePad != null && gamePad.enabled;

        public PlayerEventAssignment(int playerId)
        {
            PlayerId = playerId;
        }

        public void Update()
        {
            if (gamePad != null && gamePad.enabled && gamePad.wasUpdatedThisFrame)
            {
                HandleButton(gamePad.aButton, OnButtonA);
                HandleButton(gamePad.bButton, OnButtonB);
                HandleButton(gamePad.xButton, OnButtonX);
                HandleButton(gamePad.yButton, OnButtonY);
                if(LeftJoyStick != gamePad.leftStick.ReadValue())
                    LeftJoyStick = gamePad.leftStick.ReadValue();
                if (RightJoyStick != gamePad.rightStick.ReadValue())
                    RightJoyStick = gamePad.rightStick.ReadValue();
            }
        }

        void HandleButton(ButtonControl button, UnityEvent<bool> buttonEvent)
        {
            if (button.wasPressedThisFrame)
                buttonEvent.Invoke(true);
            if (button.wasReleasedThisFrame)
                buttonEvent.Invoke(false);
        }

        public bool UsesGamepad(Gamepad gamePad)
        {
            return this.gamePad == gamePad;
        }

        public void AssignGamePad(Gamepad gamepad)
        {
            this.gamePad = gamepad;
        }

        public void RemoveGamepad()
        {
            gamePad = null;
        }

        public void AssignAButton(Action onButton)
        {
            OnButtonA.AddListener(delegate {
                try
                {
                    onButton.Invoke();
                }
                catch
                {

                }
            });
        }

        public void AssignBButton(Action onButton)
        {
            OnButtonB.AddListener(delegate {
                try
                {
                    onButton.Invoke();
                }
                catch
                {

                }
            });
        }

        public void AssignXButton(Action onButton)
        {
            OnButtonX.AddListener(delegate {
                try
                {
                    onButton.Invoke();
                }
                catch
                {

                }
            });
        }

        public void AssignYButton(Action onButton)
        {
            OnButtonY.AddListener(delegate {
                try
                {
                    onButton.Invoke();
                }
                catch
                {

                }
            });
        }
    }
}
