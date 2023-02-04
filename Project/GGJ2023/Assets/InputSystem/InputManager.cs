using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Linq;

[CreateAssetMenu(fileName = "InputManager", menuName = "ScriptableObjects/InputManager")]
public class InputManager : ScriptableObject
{
    List<PlayerEventAssignment> knownGamePads = new List<PlayerEventAssignment>();

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceStateChange;
        UpdateFoundControllers();
    }

    private void Awake()
    {
        InputSystem.onDeviceChange += OnDeviceStateChange;
        UpdateFoundControllers();
    }

    private void OnDeviceStateChange(InputDevice device, InputDeviceChange state)
    {
        switch (state)
        {
            case InputDeviceChange.Added:
                if (device is Gamepad)
                {
                    knownGamePads.Add(new PlayerEventAssignment(device as Gamepad, GetEarliestFreeNumber()));
                    knownGamePads.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));
                }
                // New Device.
                break;
            case InputDeviceChange.Disconnected:
                foreach(var gamePad in knownGamePads)
                    if(device is Gamepad && gamePad.UsesGamepad(device as Gamepad))
                    {
                        knownGamePads.Remove(gamePad);
                        break;
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
            knownGamePads.Add(new PlayerEventAssignment(gamePad, GetEarliestFreeNumber()));
            knownGamePads.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));
        }
    }

    int GetEarliestFreeNumber()
    {
        int lowestNumber = 1;
        while(true)
        {
            if (!knownGamePads.Any(g => g.PlayerId == lowestNumber))
                return lowestNumber;
            lowestNumber++;
        }
        UnityEngine.Debug.LogError("InputManager - Couldn't get a free number.");
        return -1;
    }

    public void HookUpPlayer(int playerId, Action aButtonEvent, Action bButtonEvent, Action xButtonEvent, Action yButtonEvent, Action<Vector2> leftStickUpdateEvent,
            Action<Vector2> rightStickUpdateEvent)
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

        public PlayerEventAssignment(Gamepad gamePad, int playerId)
        {
            this.gamePad = gamePad;
            PlayerId = playerId;
        }

        public bool UsesGamepad(Gamepad gamePad)
        {
            return this.gamePad == gamePad;
        }
    }
}
