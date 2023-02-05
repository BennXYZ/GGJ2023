public interface IViewTarget
{
    float LocalPosition { get; }

    float Width { get; }

    bool IsBuilding { get; }

    string BuildingName { get; }
}