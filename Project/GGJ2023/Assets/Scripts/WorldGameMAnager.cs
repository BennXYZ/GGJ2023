using System;
using System.Collections;
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
    int maxNumberOfUnits;
    List<Minion> existingUnits = new();
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
