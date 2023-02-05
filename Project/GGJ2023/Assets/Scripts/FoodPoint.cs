using UnityEngine;

public class FoodPoint : MonoBehaviour, IViewTarget
{
    public float LocalPosition => transform.localPosition.x;
    public bool IsBuilding => false;

    [SerializeField]
    string buildingName;

    [SerializeField]
    private float width;
    public float Width => Width / 2;

    public string BuildingName => buildingName;
}
