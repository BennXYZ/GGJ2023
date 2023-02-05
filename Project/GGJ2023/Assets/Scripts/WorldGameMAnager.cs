using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    Over,
    Under
}

public abstract class WorldMode
{
    public WorldGameManager World { get; private set; }

    protected WorldMode(WorldGameManager worldGameManager)
    {
        World = worldGameManager;
    }

    public bool Active { get; set; }

    public abstract void Shutdown();

    public abstract void Update();

    public abstract void Startup();
}

public class WorldGameManager : MonoBehaviour
{
    [SerializeField]
    PlayerType playerID;
    public PlayerType PlayerID => playerID;

    [SerializeField]
    Minion minionPrefab;

    public Minion MinionPrefab => minionPrefab;

    [field: SerializeField]
    public BuildingContainer Buildings { get; private set; }
    [field:SerializeField]
    public Transform Spawn { get; private set; }

    [field: SerializeField]
    public int Resources { get; private set; }

    [SerializeField]
    private float groundOffset;

    [Space]
    [SerializeField]
    float cameraMoveSpeed = 4.0f;
    public float CameraMoveSpeed => cameraMoveSpeed;
    [SerializeField]
    Camera usedCamera;
    public Camera UsedCamera => usedCamera;

    [Space]
    [SerializeField]
    InputManager inputManager;

    List<Building> existingBuildings = new();
    public IReadOnlyList<Building> ExistingBuildings => existingBuildings;
    List<Minion> existingUnits = new();

    private WorldGenerator worldGenerator;

    WorldBuildMode buildMode;
    WorldPlayMode playMode;

    WorldMode currentWorldMode;

    // Start is called before the first frame update
    void Start()
    {
        buildMode = new WorldBuildMode(this);
        playMode = new WorldPlayMode(this);

        Debug.Assert(Buildings, "No buildings object assigned");
        inputManager.AssignButton("X", (int)playerID, ToggleMode);
        inputManager.AssignButton("L", (int)playerID, buildMode.PreviousBuilding);
        inputManager.AssignButton("R", (int)playerID, buildMode.NextBuilding);
        inputManager.AssignButton("A", (int)playerID, buildMode.Build);

        worldGenerator = gameObject.AddComponent<WorldGenerator>();
        worldGenerator.Initialize(Buildings, groundOffset);
        worldGenerator.RefreshGround(usedCamera.transform.position.x, playerID);

        SetActiveWorldMode(playMode);
    }

    public void AddBuilding(Building buildingPrefab)
    {
        Building newBuilding = Instantiate(buildingPrefab, Spawn.position, Quaternion.identity, transform);
        Resources -= newBuilding.Price;
        for (int i = 0; i < existingBuildings.Count; i++)
        {
            if (existingBuildings[i].transform.position.x < newBuilding.transform.position.x)
            {
                existingBuildings.Insert(i, newBuilding);
                return;
            }
        }
        existingBuildings.Add(newBuilding);
    }

    // Update is called once per frame
    void Update()
    {
        currentWorldMode?.Update();
    }

    private void LateUpdate()
    {
        worldGenerator.RefreshGround(usedCamera.transform.position.x, playerID);
    }

    public void DespawnUnits(int number)
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

    private void ToggleMode()
    {
        if (buildMode.Active)
            SetActiveWorldMode(playMode);
        else
            SetActiveWorldMode(buildMode);
    }

    private void SetActiveWorldMode(WorldMode worldMode)
    {
        if (currentWorldMode != null)
        {
            currentWorldMode.Shutdown();
            currentWorldMode.Active = false;
        }
        currentWorldMode = worldMode;
        if (currentWorldMode != null)
        {
            currentWorldMode.Active = true;
            currentWorldMode.Startup();
        }
    }

    void UpdateCosts()
    {
        int result = 0;
        foreach (Building building in existingBuildings)
        {
            result += building.Tick(Time.deltaTime);
        }
    }
}
