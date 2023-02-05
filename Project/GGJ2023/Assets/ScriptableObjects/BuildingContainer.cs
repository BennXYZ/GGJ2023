using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildings", menuName ="ScriptableObjects/Buildings")]
public class BuildingContainer : ScriptableObject
{
    [SerializeField]
    private List<Building> buildingsOver;
    [SerializeField]
    private List<Building> buildingsUnder;

    [SerializeField]
    private List<Transform> worldSegmentsOver;
    [SerializeField]
    private List<Transform> worldSegmentsUnder;

    [field: SerializeField]
    public Shader PreviewShader { get; private set; }
    [field: SerializeField]
    public Color PreviewInvalid { get; private set; }
    [field: SerializeField]
    public Color PreviewValid { get; private set; }

    public IReadOnlyList<Building> GetBuildings(WorldGameManager.PlayerType playerId)
    {
        switch (playerId)
        {
            case WorldGameManager.PlayerType.Over: return buildingsOver;
            case WorldGameManager.PlayerType.Under: return buildingsUnder;
            default: return null;
        }
    }

    public IReadOnlyList<Transform> GetWorldSegments (WorldGameManager.PlayerType playerId)
    {
        switch (playerId)
        {
            case WorldGameManager.PlayerType.Over: return worldSegmentsOver;
            case WorldGameManager.PlayerType.Under: return worldSegmentsUnder;
            default: return null;
        }
    }
}
