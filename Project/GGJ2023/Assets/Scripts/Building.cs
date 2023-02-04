using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField]
    string name;
    [SerializeField]
    int cost;
    [Space]
    [SerializeField]
    int workers;
    [SerializeField]
    float workPerformance;
    [Space]
    [SerializeField]
    float timer;
    [SerializeField]
    int productionWorker;
    [SerializeField]
    float friendzone;
    [SerializeField]
    float width;
    public float Width => width;

    bool isEnabled;
    
    float WorkPerformance => workPerformance;

    [SerializeField]
    float startTimer = 0f;
    float consumputionDuration = 0.0f;
    float consumptionTimer = 0;

    void Start()
    {
        isEnabled = true;
        consumptionTimer = consumputionDuration;
    }

    void Update()
    {

    }

    public int Tick(float deltaTime)
    {

        Debug.Log(deltaTime);
        if (startTimer <= 0.0f && isEnabled)
        {
            consumptionTimer -= deltaTime;
            if (consumptionTimer <= 0)
            {
                consumptionTimer += consumputionDuration;
                return 1;
            }
        }
        else
        {
            startTimer -= deltaTime;
        }
        return 0;
        //        return cost/3600 * deltaTime;
    }

    public virtual MinionStates Interact(Minion minion)
    {
        return MinionStates.Idle;
    }

  
}
