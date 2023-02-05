using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    [SerializeField]
    PlayerType playerID;
    public PlayerType PlayerID => playerID;

    [SerializeField]
    World otherWorld;

    [SerializeField]
    Minion minionPrefab;

    [SerializeField]
    GameObject rootPrefab;

    [SerializeField]
    TMPro.TMP_Text minionsText, foodText, buildingTitle, capacityText, aButtonText, xButtonText, yButton;

    [SerializeField]
    GameObject buildingModeBorder;

    public Minion MinionPrefab => minionPrefab;
    public GameObject RootPrefab => rootPrefab;

    [field: SerializeField]
    public BuildingContainer Buildings { get; private set; }
    [field: SerializeField]
    public Transform Spawn { get; private set; }

    [SerializeField]
    private float resources = 1000;

    public int Resources => Mathf.FloorToInt(resources);

    [SerializeField]
    private float groundOffset;

    [Space]
    [SerializeField]
    float cameraMoveSpeed = 4.0f;
    public float CameraMoveSpeed => cameraMoveSpeed;
    [SerializeField]
    Camera usedCamera;
    public Camera UsedCamera => usedCamera;

    [SerializeField]
    private FoodPoint foodPoint;
    public FoodPoint FoodPoint => foodPoint;

    [Space]
    [SerializeField]
    InputManager inputManager;

    List<Building> existingBuildings = new();
    public IReadOnlyList<Building> ExistingBuildings => existingBuildings;
    List<IViewTarget> viewTargets = new();
    public IReadOnlyList<IViewTarget> ViewTargets => viewTargets;

    List<Minion> existingUnits = new();

    [SerializeField]
    float currentFoodCost = 240;

    int CurrentlyAvailableMinions
    {
        get => existingUnits.Count(u => u.TargetPrio() == 0);
    }
    int CurrentMaxUnits
    {
        get => existingUnits.Count;
    }

    [SerializeField]
    private float additionalBuildArea;

    public float MinCameraPosition => ViewTargets[ViewTargets.Count - 1].LocalPosition - additionalBuildArea;
    public float MaxCameraPosition => ViewTargets[0].LocalPosition + additionalBuildArea;

    private WorldGenerator worldGenerator;

    WorldBuildMode buildMode;
    WorldPlayMode playMode;

    WorldMode currentWorldMode;

    // Start is called before the first frame update
    void Start()
    {
        viewTargets.Add(foodPoint);

        buildMode = new WorldBuildMode(this);
        playMode = new WorldPlayMode(this);

        Debug.Assert(Buildings, "No buildings object assigned");
        inputManager.AssignButton(InputManager.X_KEY, (int)playerID, ToggleMode);
        inputManager.AssignButton(InputManager.L_KEY, (int)playerID, buildMode.PreviousBuilding);
        inputManager.AssignButton(InputManager.R_KEY, (int)playerID, buildMode.NextBuilding);
        inputManager.AssignButton(InputManager.A_KEY, (int)playerID, buildMode.Build);
        inputManager.AssignButton(InputManager.LEFT_KEY, (int)playerID, playMode.MoveNegative);
        inputManager.AssignButton(InputManager.RIGHT_KEY, (int)playerID, playMode.MovePositive);

        worldGenerator = gameObject.AddComponent<WorldGenerator>();
        worldGenerator.Initialize(Buildings, groundOffset);
        worldGenerator.RefreshGround(usedCamera.transform.localPosition.x, playerID);

        SetActiveWorldMode(playMode);
    }

    public void GainResource(int amount)
    {
        resources += amount;
    }

    public void AddBuilding(Building buildingPrefab)
    {
        Building newBuilding = Instantiate(buildingPrefab, Spawn.position, Quaternion.identity, transform);
        newBuilding.Manager = this;
        resources -= newBuilding.Price;
        { // Add to buildings
            bool added = false;
            for (int i = 0; i < existingBuildings.Count; i++)
            {
                if (existingBuildings[i].LocalPosition < newBuilding.LocalPosition)
                {
                    existingBuildings.Insert(i, newBuilding);
                    added = true;
                    break;
                }
            }
            if (!added)
                existingBuildings.Add(newBuilding);
        }
        { // Add to viewtargets
            bool added = false;
            for (int i = 0; i < viewTargets.Count; i++)
            {
                if (viewTargets[i].LocalPosition < newBuilding.LocalPosition)
                {
                    viewTargets.Insert(i, newBuilding);
                    added = true;
                    break;
                }
            }
            if (!added)
                viewTargets.Add(newBuilding);
        }
        newBuilding.CreateBuilding();
        otherWorld.OtherBuildingBuilt(newBuilding);
    }

    public bool CollidesWithOtherBuilding(float pos)
    {
        return otherWorld.AnyBuildingsAtPoint(pos);
    }

    private bool AnyBuildingsAtPoint(float pos)
    {
        foreach(var build in existingBuildings)
        {
            if (pos > build.transform.position.x - build.Width && pos < build.transform.position.x + build.Width)
                return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        // Increases hunger
        currentFoodCost += Time.deltaTime;

        // Eats resources
        resources -= Time.deltaTime / 60 * currentFoodCost;

        currentWorldMode?.Update();

        foodPoint.SetFoodCount(Resources / 100);

        UpdateUI();
    }

    public void OtherBuildingBuilt(Building building)
    {
        foreach (Building build in ExistingBuildings)
        {
            build.PotentiallyDestroyRoots(building.transform.position.x - building.Width, building.transform.position.x + building.Width);
        }
    }

    public void OtherBuildingDestroyed(Building building)
    {
        foreach(Building build in ExistingBuildings)
        {
            build.PotentiallyRegrowRoots(building.transform.position.x - building.Width, building.transform.position.x + building.Width);
        }
    }

    private void UpdateUI()
    {
        if (minionsText == null)
            return;
        minionsText.text = CurrentlyAvailableMinions + "/" + CurrentMaxUnits;
        foodText.text = Resources.ToString();
        buildingTitle.text = currentWorldMode.GetBuildingTitle();
        capacityText.text = currentWorldMode.GetBuildingCapacity();
        aButtonText.text = buildMode.Active ? "Place" : "Enable/ Disable";
        xButtonText.text = buildMode.Active ? "Play Mode" : "Build Mode";
        yButton.gameObject.SetActive(!buildMode.Active);
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
        foreach (Building building in existingBuildings)
        {
            if (building.CanAssignMinions)
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
            foreach (Minion minion in existingUnits)
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
        if(buildingModeBorder != null)
            buildingModeBorder.SetActive(buildMode.Active);
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

    public abstract string GetBuildingTitle();

    public abstract string GetBuildingCapacity();
}