using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
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
    Building newBuilding;
    int currentFoodCost;
    int CurrentlyAvailableMinions
    {
        get => existingUnits.Count(u => u.TargetPrio() == 0);
    }
    int CurrentMaxUnits
    {
        get => existingUnits.Count;
    }
    Vector3 cameraMovement;

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
        worldGenerator.RefreshGround(usedCamera.transform.localPosition.x, playerID);

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
        worldGenerator.RefreshGround(usedCamera.transform.localPosition.x, playerID);
    }

    /// <summary>
    /// Can be used when a minion is checking for work. assigns minion to first building with free minions slots.
    /// </summary>
    /// <param name="minion"></param>
    void CheckBuildingsToAssign(Minion minion)
    {
        foreach(Building building in existingBuildings)
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
        foreach (Building building in existingBuildings)
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
        CheckBuildingsToAssign(instance);
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

public enum PlayerType
{
    Over,
    Under
}

public abstract class WorldMode
{
    public World World { get; private set; }

    protected WorldMode(World worldGameManager)
    {
        World = worldGameManager;
    }

    public bool Active { get; set; }

    public abstract void Shutdown();

    public abstract void Update();

    public abstract void Startup();
}