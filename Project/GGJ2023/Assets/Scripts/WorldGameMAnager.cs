using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGameMAnager : MonoBehaviour
{
    List<Building> listOfBuildings;
    int currentFoodCost;

    int maxNumberOfUnits;
    List<Minion> existingUnits;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DespawnUnits(int number)
    {
        int currentlyRemoved = 0;
        for (int i = 0; i < existingUnits.Count; i++)
        {
            Minion unit = existingUnits[i];
            if (unit.TargetPrio() == 0)
            {
                Despawn(unit);
                currentlyRemoved++;
                if (currentlyRemoved == number)
                    return;
                i--;
            }
        }
        for (int i = 0; i < existingUnits.Count; i++)
        {
            Minion unit = existingUnits[i];
            if (unit.TargetPrio() == 1)
            {
                Despawn(unit);
                currentlyRemoved++;
                if (currentlyRemoved == number)
                    return;
                i--;
            }
        }
        for (int i = 0; i < existingUnits.Count; i++)
        {
            Minion unit = existingUnits[i];
            Despawn(unit);
            currentlyRemoved++;
            if (currentlyRemoved == number)
                return;
            i--;
        }
    }

    private void Despawn(Minion unit)
    {
        throw new NotImplementedException();
    }

    void UpdateCosts()
    {
        
        foreach (var build in listOfBuildings)
        {
            int result = build.Tick(Time.deltaTime);
            
        }
    }
}
