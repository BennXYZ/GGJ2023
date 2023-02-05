using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
        foreach (var entry in knownGamePads)
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
                    case "L":
                        entry.AssignLButton(onButton);
                        break;
                    case "R":
                        entry.AssignRButton(onButton);
                        break;
                }
            }
        }
    }

    public bool IsButtonDown(string button, int playerId)
    {
        foreach (var entry in knownGamePads)
        {
            if (entry.PlayerId == playerId)
            {
                switch (button)
                {
                    case "A":
                        return entry.GamePad.aButton.isPressed;
                    case "B":
                        return entry.GamePad.bButton.isPressed;
                    case "X":
                        return entry.GamePad.xButton.isPressed;
                    case "Y":
                        return entry.GamePad.yButton.isPressed;
                    case "L":
                        return entry.GamePad.leftShoulder.isPressed;
                    case "R":
                        return entry.GamePad.rightShoulder.isPressed;
                }
            }
        }
        return false;
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
            if (knownGamePads.Any(g => g.UsesGamepad(gamePad)))
            {
                continue;
            }
            foreach (var entry in knownGamePads)
                if (!entry.HasGamePad)
                {
                    entry.AssignGamePad(gamePad);
                    break;
                }
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
        if (entry == null)
        {
            return;
        }
    }

    public class PlayerEventAssignment
    {
        public Gamepad GamePad { get; private set; }
        UnityEvent<bool> enabledChanged = new UnityEvent<bool>();

        readonly UnityEvent OnButtonA = new UnityEvent();
        readonly UnityEvent OnButtonB = new UnityEvent();
        readonly UnityEvent OnButtonX = new UnityEvent();
        readonly UnityEvent OnButtonY = new UnityEvent();
        readonly UnityEvent OnButtonL = new UnityEvent();
        readonly UnityEvent OnButtonR = new UnityEvent();
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
                if (value != Enabled)
                {
                    enabled = value;
                    enabledChanged.Invoke(Enabled);
                }
            }
        }

        public bool HasGamePad => GamePad != null && GamePad.enabled;

        public PlayerEventAssignment(int playerId)
        {
            PlayerId = playerId;
        }

        public void Update()
        {
            if (GamePad != null && GamePad.enabled && GamePad.wasUpdatedThisFrame)
            {
                HandleButton(GamePad.aButton, OnButtonA);
                HandleButton(GamePad.bButton, OnButtonB);
                HandleButton(GamePad.xButton, OnButtonX);
                HandleButton(GamePad.yButton, OnButtonY);
                HandleButton(GamePad.leftShoulder, OnButtonL);
                HandleButton(GamePad.rightShoulder, OnButtonR);
                if (LeftJoyStick != GamePad.leftStick.ReadValue())
                    LeftJoyStick = GamePad.leftStick.ReadValue();
                if (RightJoyStick != GamePad.rightStick.ReadValue())
                    RightJoyStick = GamePad.rightStick.ReadValue();
            }
        }

        void HandleButton(ButtonControl button, UnityEvent buttonEvent)
        {
            if (button.wasPressedThisFrame)
                buttonEvent.Invoke();
        }

        public bool UsesGamepad(Gamepad gamePad)
        {
            return this.GamePad == gamePad;
        }

        public void AssignGamePad(Gamepad gamepad)
        {
            this.GamePad = gamepad;
        }

        public void RemoveGamepad()
        {
            GamePad = null;
        }

        public void AssignAButton(Action onButton)
        {
            OnButtonA.AddListener(delegate
            {
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
            OnButtonB.AddListener(delegate
            {
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
            OnButtonX.AddListener(delegate
            {
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
            OnButtonY.AddListener(delegate
            {
                try
                {
                    onButton.Invoke();
                }
                catch
                {

                }
            });
        }

        public void AssignLButton(Action onButton)
        {
            OnButtonL.AddListener(delegate
            {
                try
                {
                    onButton.Invoke();
                }
                catch
                {

                }
            });
        }

        public void AssignRButton(Action onButton)
        {
            OnButtonR.AddListener(delegate
            {
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
