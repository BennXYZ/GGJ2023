using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Building : MonoBehaviour, IViewTarget
{
    public World Manager;

    [SerializeField]
    string buildingName;

    [field: SerializeField]
    public int Price { get; private set; }
    [Space]
    [SerializeField]
    int workers;
    [SerializeField]
    float workerSpawnDelay;
    [SerializeField]
    bool usesMinions = true;
    [Space]
    [SerializeField]
    float timer;
    [SerializeField]
    int productionWorker;

    [SerializeField]
    float resourceTicker = 0;
    [SerializeField]
    int resourcesPerTick = 1;
    [SerializeField]
    int maxNumberAssignedMinions;
    [SerializeField]
    float width = 3;
    [SerializeField]
    float rootWidth = 3;
    [SerializeField]
    int rootCount = 4;
    [SerializeField]
    float rootHeightDisplacement = 3;

    float resourceTimestamp;

    [Space]
    [SerializeField]
    Transform minionSpawnLocation;

    List<Material> previewMaterials = new List<Material>();

    List<Minion> assignedMinions = new List<Minion>();
    List<Minion> spawnedMinions = new List<Minion>();
    List<GameObject> spawnedRoots = new List<GameObject>();

    public float Width => width / 2;

    bool isEnabled;

    public float WorkPerformance
    {
        get
        {
            if (!usesMinions)
                return 1f;
            if(rootCount > 0)
                return (float)Mathf.Min(spawnedRoots.Count(s => s != null), assignedMinions.Count) / (float)rootCount;
            return 1f;
        }
    }
    public int MaxNumberAssignedMinions => maxNumberAssignedMinions;

    public bool CanAssignMinions => assignedMinions.Count < MaxNumberAssignedMinions;

    public float LocalPosition => transform.localPosition.x;

    public bool IsBuilding => true;

    public string BuildingName => buildingName;

    void Start()
    {
        isEnabled = true;
    }

    void Update()
    {
        if(resourceTimestamp > 0 && resourceTicker > 0 && resourcesPerTick > 0)
        {
            if(Time.time > resourceTimestamp + resourceTicker)
            {
                resourceTimestamp = Time.time;
                Manager.GainResource(resourcesPerTick);
            }
        }
    }

    public void CreateBuilding()
    {
        resourceTimestamp = Time.time;
        if (workers > 0)
            StartSpawningWorkers();
        SpawnRoots();
    }

    void StartSpawningWorkers()
    {
        StartCoroutine(SpawnWorkersCoroutine());
    }

    IEnumerator SpawnWorkersCoroutine()
    {
        int numberSpawned = 0;
        while(numberSpawned < workers)
        {
            yield return new WaitForSeconds(workerSpawnDelay);
            SpawnWorker();
            numberSpawned++;
        }
    }

    private void SpawnWorker()
    {
        Minion instance = Instantiate(Manager.MinionPrefab, minionSpawnLocation != null ? minionSpawnLocation.position : transform.position, Quaternion.identity);
        spawnedMinions.Add(instance);
        instance.SetHome(this);
        Manager.MinionSpawned(instance);
    }

    public void AssignMinion(Minion minion)
    {
        assignedMinions.Add(minion);
        minion.AssignBuilding(this);
    }

    public virtual MinionStates Interact(Minion minion)
    {
        return MinionStates.Idle;
    }

    void SpawnRoots()
    {
        if (Manager.RootPrefab != null)
            for (int i = 0; i < rootCount; i++)
            {
                spawnedRoots.Add(Instantiate(Manager.RootPrefab, GetRootPosition(i), Manager.RootPrefab.transform.rotation, transform));
            }
    }

    public void UsePreviewMaterial(Shader shader, Color tintColor)
    {
        if (previewMaterials.Count == 0)
        {
            List<Renderer> renderers = new();
            GetComponentsInChildren(true, renderers);
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                Material[] newMaterials = new Material[materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    newMaterials[i] = new Material(shader);
                    newMaterials[i].SetTexture("BaseMap", materials[i].mainTexture);
                    newMaterials[i].SetColor("TintColor", tintColor);
                }
                renderer.sharedMaterials = newMaterials;
                previewMaterials.AddRange(newMaterials);
            }
        }
        else
        {
            foreach (Material previewMaterial in previewMaterials)
            {
                previewMaterial.SetColor("TintColor", tintColor);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < rootCount; i++)
        {
            Gizmos.DrawSphere(GetRootPosition(i), 0.2f);
        }
    }

    public void PotentiallyDestroyRoots(float leftBorder, float rightBorder)
    {
        for (int i = 0; i < rootCount; i++)
        {
            float rootPos = GetRootPosition(i).x;
            if (rootPos > leftBorder && rootPos < rightBorder)
            {
                TryDeleteRoots(i);
            }
        }
    }

    public void PotentiallyRegrowRoots(float leftBorder, float rightBorder)
    {
        for (int i = 0; i < rootCount; i++)
        {
            float rootPos = GetRootPosition(i).x;
            if (rootPos > leftBorder && rootPos < rightBorder)
            {
                TryRegrowRoots(i);
            }
        }
    }

    private void TryRegrowRoots(int i)
    {
        if (spawnedRoots[i] == null)
        {
            spawnedRoots[i] = Instantiate(Manager.RootPrefab, GetRootPosition(i), Manager.RootPrefab.transform.rotation, transform);
        }
    }

    private void TryDeleteRoots(int i)
    {
        if(spawnedRoots[i] != null)
        {
            Destroy(spawnedRoots[i]);
            spawnedRoots[i] = null;
        }
    }

    Vector3 GetRootPosition(int index)
    {
        if(index < 0 || index > rootCount)
        {
            return Vector3.zero;
        }
        return transform.position + Vector3.up * rootHeightDisplacement + (-Vector3.right * rootWidth / 2) + Vector3.right * index * rootWidth / (rootCount - 1);
    }
}
