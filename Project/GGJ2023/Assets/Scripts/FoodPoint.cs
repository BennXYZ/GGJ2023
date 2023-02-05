using UnityEngine;

public class FoodPoint : MonoBehaviour, IViewTarget
{
    public float LocalPosition => transform.localPosition.x;
    public bool IsBuilding => false;

    [SerializeField]
    string buildingName;

    [SerializeField]
    Transform worm;

    [SerializeField]
    float startSize, growthSpeed;
    float growthStartTime;

    [SerializeField]
    private float width;
    public float Width => width / 2;

    public string BuildingName => buildingName;

    void Start()
    {
        growthStartTime = Time.time;
    }

    void Update()
    {
        worm.localScale = Vector3.one * startSize + Vector3.one * ((Time.time - growthStartTime) * growthSpeed);
    }
}
