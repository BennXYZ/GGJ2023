using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildings", menuName ="ScriptableObjects/Buildings")]
public class NewBehaviourScript : ScriptableObject
{
    [SerializeField]
    List<Building> listOfBuildingsOver;
    [SerializeField]
    List<Building> listOfBuildingsDown;
}
