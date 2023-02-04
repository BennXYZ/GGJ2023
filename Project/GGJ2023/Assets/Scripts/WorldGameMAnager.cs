using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGameMAnager : MonoBehaviour
{
    List<Building> listOfBuildings;
    int currentFoodCost;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateCosts()
    {
        
        foreach (var build in listOfBuildings)
        {
            int result = build.Tick(Time.deltaTime);
            
        }
    }
}
