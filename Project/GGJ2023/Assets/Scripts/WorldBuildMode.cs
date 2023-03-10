using System;
using System.Collections.Generic;
using UnityEngine;

internal class WorldBuildMode : WorldMode
{
    enum BuildBlocker
    {
        Price,
        Location
    }

    HashSet<BuildBlocker> blockers = new();
    Building previewBuilding = null;
    int selectedBuilding = 0;
    private IReadOnlyList<Building> buildings;

    public bool IsBlocked => blockers.Count > 0;

    public WorldBuildMode(World worldGameManager) : base(worldGameManager)
    {
        buildings = World.Buildings.GetBuildings(World.PlayerID);
        selectedBuilding = 0;
    }

    public override void Shutdown()
    {
        if (previewBuilding != null)
            GameObject.Destroy(previewBuilding.gameObject);
        previewBuilding = null;
    }

    public override void Startup()
    {
        RefreshPreview();
    }

    private void RefreshPreview()
    {
        if (previewBuilding != null)
            GameObject.Destroy(previewBuilding.gameObject);
        previewBuilding = GameObject.Instantiate(buildings[selectedBuilding], World.Spawn.position, Quaternion.identity);
        previewBuilding.transform.parent = World.Spawn;
        blockers.Clear();
        RefreshPreviewMaterial();
    }

    public void NextBuilding()
    {
        if (!Active)
            return;

        ++selectedBuilding;
        if (selectedBuilding >= buildings.Count)
            selectedBuilding = 0;
        RefreshPreview();
    }

    public void PreviousBuilding()
    {
        if (!Active)
            return;

        --selectedBuilding;
        if (selectedBuilding < 0)
            selectedBuilding = buildings.Count - 1;
        RefreshPreview();
    }

    public void Build()
    {
        if (!Active)
            return;

        if (!IsBlocked)
        {
            World.AddBuilding(buildings[selectedBuilding]);
        }
    }

    public override void Update()
    {
        Vector3 cameraMovement = new Vector3(InputManager.Instance.GetLeftJoystick((int)World.PlayerID).x, 0f);
        Camera camera = World.UsedCamera;
        camera.transform.Translate(World.CameraMoveSpeed * Time.deltaTime * cameraMovement);
        Vector3 localCameraPosition = camera.transform.localPosition;
        if (localCameraPosition.x < World.MinCameraPosition)
            localCameraPosition.x = World.MinCameraPosition;
        if (localCameraPosition.x > World.MaxCameraPosition)
            localCameraPosition.x = World.MaxCameraPosition;
        camera.transform.localPosition = localCameraPosition;

        SetBuildBlocker(BuildBlocker.Price, World.Resources < previewBuilding.Price);
        bool blocked = false;
        foreach (Building building in World.ExistingBuildings)
        {
            float distance = Mathf.Abs(building.transform.position.x - previewBuilding.transform.position.x);
            if (distance < building.Width + previewBuilding.Width)
            {
                blocked = true;
                break;
            }
        }

        if (!blocked)
        {
            float distance = Mathf.Abs(World.FoodPoint.transform.position.x - previewBuilding.transform.position.x);
            blocked = (distance < World.FoodPoint.Width + previewBuilding.Width);
        }

        SetBuildBlocker(BuildBlocker.Location, blocked);
    }

    private void SetBuildBlocker(BuildBlocker blocker, bool set)
    {
        bool hadBlockers = IsBlocked;

        if (set)
        {
            blockers.Add(blocker);
        }
        else
        {
            blockers.Remove(blocker);
        }

        if (hadBlockers != IsBlocked)
        {
            RefreshPreviewMaterial();
        }
    }

    private void RefreshPreviewMaterial()
    {
        if (previewBuilding != null)
        {
            previewBuilding.UsePreviewMaterial(World.Buildings.PreviewShader, IsBlocked ? World.Buildings.PreviewInvalid : World.Buildings.PreviewValid);
        }
    }

    public override string GetBuildingTitle()
    {
        return buildings[selectedBuilding].BuildingName;
    }

    public override string GetBuildingCapacity()
    {
        return "Price: " + buildings[selectedBuilding].Price;
    }
}