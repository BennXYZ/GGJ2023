using UnityEngine;

public class FoodPoint : MonoBehaviour, IViewTarget
{
    public float LocalPosition => transform.localPosition.x;
    public bool IsBuilding => false;

    [SerializeField]
    private float width;
    public float Width => width / 2;
}
