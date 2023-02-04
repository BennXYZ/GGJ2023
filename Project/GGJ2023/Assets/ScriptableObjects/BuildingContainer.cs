using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildings", menuName ="ScriptableObjects/Buildings")]
public class BuildingContainer : ScriptableObject
{
    [SerializeField]
    public List<Building> listOfBuildingsOver;
    [SerializeField]
    public List<Building> listOfBuildingsDown;

    public IReadOnlyList<Building> GetBuildingsByPlayerId(int playerId)
    {
        switch (playerId)
        {
            case 1: return listOfBuildingsOver;
            case 2: return listOfBuildingsDown;
            default: return null;
        }
    }
}
