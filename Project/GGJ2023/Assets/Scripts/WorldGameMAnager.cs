using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGameManager : MonoBehaviour
{
    enum GameState {Game, Build, Pause};

    [SerializeField] 
    int playerID;
    [SerializeField]
    ScriptableObject buildings;

    [SerializeField]
    Minion minionPrefab;

    public Minion MinionPrefab => minionPrefab;

    BuildingContainer buildings;
    [SerializeField]
    GameObject spawn;
    [Space]
    
    [SerializeField]
    float cameraMoveSpeed = 4.0f;
    [SerializeField]
    Camera cam;
    [Space]

    [SerializeField]
    InputManager inputManager;

    GameObject newBuilding;
    List<Building> listOfBuildings;
    int currentFoodCost;
    int maxNumberOfUnits;
    List<Minion> existingUnits;
    GameState gameState;
    Vector3 cameraMovement;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Game;
    }

    // Update is called once per frame
    void Update()
    {
        inputManager.AssignButton("X",playerID, BuildMod);
        
        if (gameState == GameState.Game)
        {
            inputManager.GetLeftJoystick(playerID);
            cameraMovement = new Vector3(InputManager.Instance.GetRightJoystick(playerID).x, 0f);
            cam.transform.Translate(cameraMovement * cameraMoveSpeed * Time.deltaTime);
            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, -9, 9), cam.transform.position.y, cam.transform.position.z);

            inputManager.AssignButton("A",playerID, SetBuildingPosiotion);
                
        }

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

    }

    void BuildMod()
    {   
        if (gameState != GameState.Build)
        {
            
            int buildingIndex = 0;
            
        
            Debug.Log("Buildmod");
            // enable UI elements
            // show current build object on the center of the field of view
            Vector3 spawnPosition = spawn.transform.position;

            if (playerID == 1)
            {   
                newBuilding = Instantiate(buildings.listOfBuildingsOver[buildingIndex].gameObject,spawnPosition, Quaternion.Euler(0,180,0));
                //newBuilding.transform.parent = spawn.transform;

                // Disable build UI
                Debug.Log(newBuilding.name);

            }else if (playerID == 2){
                newBuilding = Instantiate(buildings.listOfBuildingsDown[buildingIndex].gameObject, spawnPosition, Quaternion.Euler(0,180,0));
                
                
                // Disable build UI
                Debug.Log(newBuilding.name);
            }else{
                Debug.LogError("PlayerId out of range. PlayerId: " + playerID +".");
            }
            newBuilding.transform.parent = spawn.transform;
            gameState = GameState.Build;
        
            Debug.Log("camera");
        }

    }

    void SetBuildingPosiotion(){

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
