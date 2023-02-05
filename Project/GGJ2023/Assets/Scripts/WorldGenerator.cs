using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    private BuildingContainer buildings;
    private float groundOffset;
    private HashSet<int> existingWorldSegments;

    public void RefreshGround(float cameraPosition, PlayerType playerID)
    {
        int possibleSpawnLocations = (int)cameraPosition / 5 + Math.Sign(cameraPosition);
        float reducedCamera = cameraPosition / 5 - 4;
        for (int i = 0; i < 9; i++)
        {
            float floatOffset = reducedCamera + i;
            CreateRandomGround(Mathf.FloorToInt(floatOffset), playerID);
        }
    }

    private void CreateRandomGround(int offset, PlayerType playerId)
    {
        if (!existingWorldSegments.Contains(offset))
        {
            IReadOnlyList<Transform> worldSegments = buildings.GetWorldSegments(playerId);
            Transform prefab = worldSegments[UnityEngine.Random.Range(0, worldSegments.Count - 1)];
            Instantiate(prefab, transform.position + new Vector3(offset * 5, groundOffset, 0), Quaternion.identity, transform);

            existingWorldSegments.Add(offset);
        }
    }

    internal void Initialize(BuildingContainer buildings, float groundOffset)
    {
        this.buildings = buildings;
        this.groundOffset = groundOffset;
        existingWorldSegments = new();
    }
}