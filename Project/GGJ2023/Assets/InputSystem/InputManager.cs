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
    public const string A_KEY = "A";
    public const string B_KEY = "B";
    public const string X_KEY = "X";
    public const string Y_KEY = "Y";
    public const string L_KEY = "L";
    public const string R_KEY = "R";
    public const string UP_KEY = "UP";
    public const string DOWN_KEY = "DOWN";
    public const string LEFT_KEY = "LEFT";
    public const string RIGHT_KEY = "RIGHT";
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
                    case A_KEY:
                        entry.AssignAButton(onButton);
                        break;
                    case B_KEY:
                        entry.AssignBButton(onButton);
                        break;
                    case X_KEY:
                        entry.AssignXButton(onButton);
                        break;
                    case Y_KEY:
                        entry.AssignYButton(onButton);
                        break;
                    case L_KEY:
                        entry.AssignLButton(onButton);
                        break;
                    case R_KEY:
                        entry.AssignRButton(onButton);
                        break;
                    case UP_KEY:
                        entry.AssignUpButton(onButton);
                        break;
                    case DOWN_KEY:
                        entry.AssignDownButton(onButton);
                        break;
                    case RIGHT_KEY:
                        entry.AssignRightButton(onButton);
                        break;
                    case LEFT_KEY:
                        entry.AssignLeftButton(onButton);
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
                    case A_KEY:
                        return entry.GamePad.aButton.isPressed;
                    case B_KEY:
                        return entry.GamePad.bButton.isPressed;
                    case X_KEY:
                        return entry.GamePad.xButton.isPressed;
                    case Y_KEY:
                        return entry.GamePad.yButton.isPressed;
                    case L_KEY:
                        return entry.GamePad.leftShoulder.isPressed;
                    case R_KEY:
                        return entry.GamePad.rightShoulder.isPressed;
                    case UP_KEY:
                        return entry.GamePad.dpad.up.isPressed;
                    case DOWN_KEY:
                        return entry.GamePad.dpad.down.isPressed;
                    case LEFT_KEY:
                        return entry.GamePad.dpad.left.isPressed;
                    case RIGHT_KEY:
                        return entry.GamePad.dpad.right.isPressed;
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
        readonly UnityEvent OnButtonUp = new UnityEvent();
        readonly UnityEvent OnButtonRight = new UnityEvent();
        readonly UnityEvent OnButtonLeft = new UnityEvent();
        readonly UnityEvent OnButtonDown = new UnityEvent();

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
                HandleButton(GamePad.dpad.down, OnButtonDown);
                HandleButton(GamePad.dpad.left, OnButtonLeft);
                HandleButton(GamePad.dpad.right, OnButtonRight);
                HandleButton(GamePad.dpad.up, OnButtonUp);
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

        public void AssignUpButton(Action onButton)
        {
            OnButtonUp.AddListener(delegate
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

        public void AssignDownButton(Action onButton)
        {
            OnButtonDown.AddListener(delegate
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

        public void AssignRightButton(Action onButton)
        {
            OnButtonRight.AddListener(delegate
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

        public void AssignLeftButton(Action onButton)
        {
            OnButtonLeft.AddListener(delegate
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
