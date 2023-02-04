using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public WorldGameManager manager;

    [SerializeField]
    int cost;
    [Space]
    [SerializeField]
    int workers;
    [SerializeField]
    float workPerformance;
    [Space]
    [SerializeField]
    float timer;
    [SerializeField]
    int productionWorker;
    [SerializeField]
    float friendzone;
    [SerializeField]
    float width = 3;

    [Space]
    [SerializeField]
    Transform minionSpawnLocation;

    List<Material> previewMaterials = new List<Material>();

    public float Width => width;

    bool isEnabled;

    float WorkPerformance => workPerformance;

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
            yield return new WaitForSeconds(timer);
            SpawnWorker();
            numberSpawned++;
        }
    }

    private void SpawnWorker()
    {
        Minion instance = Instantiate(manager.MinionPrefab, minionSpawnLocation != null ? minionSpawnLocation.position : transform.position, Quaternion.identity);
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

    public virtual MinionStates Interact(Minion minion)
    {
        return MinionStates.Idle;
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
}
