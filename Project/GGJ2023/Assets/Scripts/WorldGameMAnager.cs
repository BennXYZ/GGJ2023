using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WorldGameManager : MonoBehaviour
{
    enum GameState { Game, Build, Pause }

    public enum PlayerType { Over, Under }

    [SerializeField]
    PlayerType playerID;
    [SerializeField]
    Minion minionPrefab;

    public Minion MinionPrefab => minionPrefab;

    [SerializeField]
    BuildingContainer buildings;
    [SerializeField]
    GameObject spawn;

    [SerializeField]
    private float groundOffset;

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
    List<Minion> existingUnits = new();
    int CurrentlyAvailableMinions
    {
        get => existingUnits.Count(u => u.TargetPrio() == 0);
    }
    int CurrentMaxUnits
    {
        get => existingUnits.Count;
    }
    GameState gameState;
    Vector3 cameraMovement;

    HashSet<int> existingWorldSegments = new();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(buildings, "No buildings object assigned");
        gameState = GameState.Game;
        inputManager.AssignButton("X", (int)playerID, ToggleMode);
    }

    private void CreateRandomGround(int offset)
    {
        if (!existingWorldSegments.Contains(offset))
        {
            IReadOnlyList<Transform> worldSegments = buildings.GetWorldSegments(playerID);
            Transform prefab = worldSegments[UnityEngine.Random.Range(0, worldSegments.Count - 1)];
            Instantiate(prefab, transform.position + new Vector3(offset * 5, groundOffset, 0), Quaternion.identity, transform);

            existingWorldSegments.Add(offset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int possibleSpawnLocations = (int)(cam.transform.position.x) / 5 + Math.Sign(cam.transform.position.x);

        for (int i = 0; i < 9; i++)
        {
            CreateRandomGround(possibleSpawnLocations + i - 5);
        }

        if (gameState == GameState.Game)
        {
            inputManager.GetLeftJoystick((int)playerID);
            cameraMovement = new Vector3(InputManager.Instance.GetRightJoystick((int)playerID).x, 0f);
            cam.transform.Translate(cameraMovement * cameraMoveSpeed * Time.deltaTime);
            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, -9, 9), cam.transform.position.y, cam.transform.position.z);

            inputManager.AssignButton("A", (int)playerID, SetBuildingPosition);

        }

    }

    /// <summary>
    /// Can be used when a minion is checking for work. assigns minion to first building with free minions slots.
    /// </summary>
    /// <param name="minion"></param>
    void CheckBuildingsToAssign(Minion minion)
    {
        foreach(Building building in listOfBuildings)
        {
            if(building.CanAssignMinions)
            {
                building.AssignMinion(minion);
                return;
            }
        }
    }

    /// <summary>
    /// Not sure if we need this, but this just checks all buildings and minions and assigns minions to buildings
    /// </summary>
    void CheckBuildingsToAssign()
    {
        foreach (Building building in listOfBuildings)
        {
            if (building.CanAssignMinions)
            {
                AssignMinionsToBuilding(building);
            }
        }
    }

    /// <summary>
    /// Finds free workers to assign to a building.
    /// </summary>
    void AssignMinionsToBuilding(Building building)
    {
        for (int i = 0; i < building.MaxNumberAssignedMinions; i++)
        {
            foreach(Minion minion in existingUnits)
            {
                if (minion.TargetPrio() == 0)
                    building.AssignMinion(minion);
                if (!building.CanAssignMinions)
                    return;
            }
        }
    }

    public void MinionSpawned(Minion instance)
    {
        existingUnits.Add(instance);
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

        IReadOnlyList<Building> buildingSelection = buildings.GetBuildings(playerID);
        Debug.Assert(buildingSelection != null, "Player id is invalid");
        newBuilding = Instantiate(buildingSelection[buildingIndex], spawnPosition, Quaternion.Euler(0, 180, 0));
        newBuilding.transform.parent = spawn.transform;
        newBuilding.UsePreviewMaterial(buildings.PreviewShader, buildings.PreviewInvalid);

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
