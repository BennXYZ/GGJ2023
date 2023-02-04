using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    int player;

    float targetPosition;
    bool holdingResource;


    [SerializeField]
    float movementSpeed, minRandomWanderingRange, maxRandomWanderingRange;

    [SerializeField]
    float minIdleTime, maxIdleTime;

    Building assignedBuilding;

    float idleTimeEnd;

    MinionStates previousState = MinionStates.Idle;
    MinionStates currentState = MinionStates.Idle;

    Dictionary<MinionStates, Func<MinionStates>> minionStateMachine;
    Dictionary<MinionStates, Action> enterStateMachine;

    private void Awake()
    {
        minionStateMachine = new Dictionary<MinionStates, Func<MinionStates>>()
        {
            [MinionStates.Idle] = UpdateIdle,
            [MinionStates.Moving] = UpdateMoving,
        };
        enterStateMachine = new Dictionary<MinionStates, Action> { 
            [MinionStates.Idle] = EnterIdle 
        };
    }

    private void Update()
    {
        currentState = minionStateMachine[currentState]();
        if(currentState != previousState)
        {
            if (enterStateMachine.ContainsKey(currentState))
                enterStateMachine[currentState]();
            previousState = currentState;
        }
    }

    void EnterIdle()
    {
        idleTimeEnd = Time.time + UnityEngine.Random.Range(minIdleTime, maxIdleTime);
    }

    MinionStates UpdateIdle()
    {
        if (Time.time > idleTimeEnd)
            return EndIdle();
        //Wait for End of Idle to Check for new Command.
        return MinionStates.Idle;
    }

    private MinionStates EndIdle()
    {
        //Has no Command. Wanders Around.
        {
            targetPosition = assignedBuilding.transform.position.x + UnityEngine.Random.Range(minRandomWanderingRange, maxRandomWanderingRange) * 
                (UnityEngine.Random.Range(0, 2) * 2 - 1);
            return MinionStates.Moving;
        }
        return MinionStates.Idle;
    }

    MinionStates UpdateMoving()
    {
        //Move towards goal.
        bool targetIsRight = assignedBuilding.transform.position.x < targetPosition;
        int directionMultiplier = targetIsRight ? 1 : -1;
        transform.Translate(Vector3.right * directionMultiplier * movementSpeed * Time.deltaTime);
        if (transform.position.x < targetPosition != targetIsRight)
            return MinionStates.Idle;
        return MinionStates.Moving;
    }
}

public enum MinionStates
{
    Idle,
    Moving
}