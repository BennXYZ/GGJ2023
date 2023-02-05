using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IViewTarget
{
    public World manager;

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
    float workPerformance;
    [Space]
    [SerializeField]
    float timer;
    [SerializeField]
    int productionWorker;
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

    [Space]
    [SerializeField]
    Transform minionSpawnLocation;

    List<Material> previewMaterials = new List<Material>();

    List<Minion> assignedMinions = new List<Minion>();
    List<Minion> spawnedMinions = new List<Minion>();
    List<GameObject> spawnedRoots = new List<GameObject>();

    public float Width => width / 2;

    bool isEnabled;

    public float WorkPerformance => workPerformance;
    public int MaxNumberAssignedMinions => maxNumberAssignedMinions;

    public bool CanAssignMinions => assignedMinions.Count < MaxNumberAssignedMinions;

    public float LocalPosition => transform.localPosition.x;

    public bool IsBuilding => true;

    public string BuildingName => buildingName;

    [SerializeField]
    float startTimer = 0f;
    float consumputionDuration = 0.0f;
    float consumptionTimer = 0;

    void Start()
    {
        isEnabled = true;
        consumptionTimer = consumputionDuration;
        if (workers > 0)
            StartSpawningWorkers();
        SpawnRoots();
    }

    void Update()
    {

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
        Minion instance = Instantiate(manager.MinionPrefab, minionSpawnLocation != null ? minionSpawnLocation.position : transform.position, Quaternion.identity);
        spawnedMinions.Add(instance);
        instance.SetHome(this);
        manager.MinionSpawned(instance);
    }

    public int Tick(float deltaTime)
    {

        Debug.Log(deltaTime);
        if (startTimer <= 0.0f && isEnabled)
        {
            consumptionTimer -= deltaTime;
            if (consumptionTimer <= 0)
            {
                consumptionTimer += consumputionDuration;
                return 1;
            }
        }
        else
        {
            startTimer -= deltaTime;
        }
        return 0;
        //        return cost/3600 * deltaTime;
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
        if (manager.RootPrefab != null)
            for (int i = 0; i < rootCount; i++)
            {
                spawnedRoots.Add(Instantiate(manager.RootPrefab, GetRootPosition(i), manager.RootPrefab.transform.rotation, transform));
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

    Vector3 GetRootPosition(int index)
    {
        if(index < 0 || index > rootCount)
        {
            return Vector3.zero;
        }
        return transform.position + Vector3.up * rootHeightDisplacement + (-Vector3.right * rootWidth / 2) + Vector3.right * index * rootWidth / (rootCount - 1);
    }
}
