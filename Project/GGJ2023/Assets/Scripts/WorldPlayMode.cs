using System;
using UnityEngine;

internal class WorldPlayMode : WorldMode
{
    int focussedViewTarget = 0;

    public WorldPlayMode(World worldGameManager) : base(worldGameManager)
    {
    }

    public void MovePositive()
    {
        if (!Active)
            return;

        focussedViewTarget++;
        if (focussedViewTarget >= World.ViewTargets.Count)
            focussedViewTarget = World.ViewTargets.Count - 1;

        MoveCameraToFocus();
    }

    public void MoveNegative()
    {
        if (!Active)
            return;

        focussedViewTarget--;
        if (focussedViewTarget < 0)
            focussedViewTarget = 0;

        MoveCameraToFocus();
    }

    public override void Shutdown()
    {

    }

    public override void Startup()
    {
        FocusNearestTarget();
    }

    private void FocusNearestTarget()
    {
        Vector3 cameraPosition = World.UsedCamera.transform.localPosition;

        focussedViewTarget = 0;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < World.ViewTargets.Count; i++)
        {
            float distance = Mathf.Abs(World.ViewTargets[i].LocalPosition - cameraPosition.x);
            if (closestDistance > distance)
            {
                closestDistance = distance;
                focussedViewTarget = i;
            }
        }

        MoveCameraToFocus();
    }

    private void MoveCameraToFocus()
    {
        Vector3 cameraPosition = World.UsedCamera.transform.localPosition;
        cameraPosition.x = World.ViewTargets[focussedViewTarget].LocalPosition;
        World.UsedCamera.transform.localPosition = cameraPosition;
    }

    public override void Update()
    {
    }

    public override string GetBuildingTitle()
    {
        return World.ViewTargets[focussedViewTarget].BuildingName;
    }

    public override string GetBuildingCapacity()
    {
        if(World.ViewTargets[focussedViewTarget].IsBuilding)
            return ((World.ViewTargets[focussedViewTarget] as Building).WorkPerformance * 100).ToString();
        return "";
    }
}