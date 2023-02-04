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

    float WorkPerformance => workPerformance;

    float startTimer = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public int Tick(float deltaTime)
    {
        int costs = 0;

        return costs;
    }

    public virtual MinionStates Interact(Minion minion)
    {
        return MinionStates.Idle;
    }

    void CurrentWorkPerformance()
    {

    }
}
