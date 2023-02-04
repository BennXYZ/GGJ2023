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
    float movementSpeed = 3, minMovementDistance = 1;

    [SerializeField]
    float minIdleTime = 1, maxIdleTime = 3;

    Building assignedBuilding;
    Building nextAssignedBuilding;
    Building currentTargetBuilding;

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
            [MinionStates.Idle] = EnterIdle,
            [MinionStates.Moving] = EnterMoving
        };

        //Debugging
        {
            assignedBuilding = FindObjectOfType<Building>();
        }
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

    public int TargetPrio()
    {
        if (assignedBuilding == null)
            return 0;
        if (currentState == MinionStates.Idle)
            return 1;
        return 2;
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

    public void AssignBuilding(Building building)
    {
        nextAssignedBuilding = building;
    }

    void AttemptNextJob()
    {
        if (nextAssignedBuilding != assignedBuilding)
            assignedBuilding = nextAssignedBuilding;
    }

    void CancelCurrentBuilding()
    {

    }

    private MinionStates EndIdle()
    {
        //Has no Command. Wanders Around.
        {
            return MinionStates.Moving;
        }
        return MinionStates.Idle;
    }

    Building GetTargetBuilding()
    {
        return assignedBuilding;
    }

    private void EnterMoving()
    {
        currentTargetBuilding = GetTargetBuilding();
        targetPosition = currentTargetBuilding.transform.position.x + UnityEngine.Random.Range(-currentTargetBuilding.Width * 0.5f, currentTargetBuilding.Width * 0.5f);

        if (Mathf.Abs(transform.position.x - targetPosition) < minMovementDistance)
        {
            float moveDirection = targetPosition - transform.position.x;
            moveDirection = Mathf.Sign(moveDirection) * minMovementDistance;
            targetPosition = transform.position.x + moveDirection;
        }
    }

    MinionStates UpdateMoving()
    {
        //Move towards goal.
        bool targetIsRight = transform.position.x < targetPosition;
        int directionMultiplier = targetIsRight ? 1 : -1;
        transform.Translate(Vector3.right * directionMultiplier * movementSpeed * Time.deltaTime);
        if (transform.position.x < targetPosition != targetIsRight)
        {
            //TODO: Also Cancel if target is disabled
            if (currentTargetBuilding == null)
                CancelCurrentBuilding();
            else
                return currentTargetBuilding.Interact(this);
        }
        return MinionStates.Moving;
    }
}

public enum MinionStates
{
    Idle,
    Moving
}