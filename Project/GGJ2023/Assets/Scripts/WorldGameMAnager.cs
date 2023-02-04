using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGameManager : MonoBehaviour
{
    enum GameState { Game, Build, Pause }

    [SerializeField]
    int playerID;
    [SerializeField]
    Minion minionPrefab;

    public Minion MinionPrefab => minionPrefab;

    [SerializeField]
    BuildingContainer buildings;
    [SerializeField]
    GameObject spawn;
    [SerializeField]
    Shader previewShader;
    [SerializeField]
    Color previewInvalid;
    [SerializeField]
    Color previewValid;

    [Space]

    [SerializeField]
    float cameraMoveSpeed = 4.0f;
    [SerializeField]
    Camera cam;
    [Space]

    [SerializeField]
    InputManager inputManager;

    Building newBuilding;
    List<Building> listOfBuildings = new();
    int currentFoodCost;
    int maxNumberOfUnits;
    List<Minion> existingUnits = new();
    GameState gameState;
    Vector3 cameraMovement;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Game;
        inputManager.AssignButton("X", playerID, ToggleMode);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.Game)
        {
            inputManager.GetLeftJoystick(playerID);
            cameraMovement = new Vector3(InputManager.Instance.GetRightJoystick(playerID).x, 0f);
            cam.transform.Translate(cameraMovement * cameraMoveSpeed * Time.deltaTime);
            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, -9, 9), cam.transform.position.y, cam.transform.position.z);

            inputManager.AssignButton("A", playerID, SetBuildingPosition);

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

    void ToggleMode()
    {
        if (gameState == GameState.Game)
        {
            gameState = GameState.Build;
            InitializeBuildMode();
        }
        else
        {
            gameState = GameState.Game;
        }
    }

    void InitializeBuildMode()
    {
        int buildingIndex = 0;

        Debug.Log("Buildmod");
        // enable UI elements
        // show current build object on the center of the field of view
        Vector3 spawnPosition = spawn.transform.position;

        IReadOnlyList<Building> buildingSelection = buildings.GetBuildingsByPlayerId(playerID);
        Debug.Assert(buildingSelection != null, "Player id is invalid");
        newBuilding = Instantiate(buildingSelection[buildingIndex], spawnPosition, Quaternion.Euler(0, 180, 0));
        newBuilding.transform.parent = spawn.transform;
        newBuilding.UsePreviewMaterial(previewShader, previewValid);

        Debug.Log("camera");
    }

    void SetBuildingPosition()
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
