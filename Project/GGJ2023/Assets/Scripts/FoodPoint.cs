﻿using System.Collections.Generic;
using UnityEngine;

public class FoodPoint : MonoBehaviour, IViewTarget
{
    public float LocalPosition => transform.localPosition.x;
    public bool IsBuilding => false;

    [SerializeField]
    string buildingName;

    [SerializeField]
    private float width;
    public float Width => width / 2;

    public string BuildingName => buildingName;

    [SerializeField]
    private Transform foodPrefab;

    List<Transform> foodInstances = new();

    public void SetFoodCount(int foodCount)
    {
        if (foodPrefab == null)
            return;
            
        while (foodInstances.Count > foodCount)
        {
            int index = Random.Range(0, foodInstances.Count);
            Destroy(foodInstances[index].gameObject);
            foodInstances.RemoveAt(index);
        }

        while (foodInstances.Count < foodCount)
        {
            Transform foodInstance = Instantiate(foodPrefab, transform);
            foodInstance.localPosition = new Vector3(Random.Range(-Width, Width), Random.Range(0.3f, 0.9f), Random.Range(0.1f, 0.3f));
            foodInstance.localRotation = Quaternion.Euler(0, 180, 0);
            foodInstances.Add(foodInstance);
        }
    }
}
