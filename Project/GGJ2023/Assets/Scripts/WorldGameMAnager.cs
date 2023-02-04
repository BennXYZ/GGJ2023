using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGameManager : MonoBehaviour
{
    [SerializeField]
    ScriptableObject buildings;
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

    void BuildMod()
    {
        
    }

    void UpdateCosts()
    {
        int result = 0;
        foreach (var build in listOfBuildings)
        {
            result += build.Tick(Time.deltaTime);
            
        }
    }
}
