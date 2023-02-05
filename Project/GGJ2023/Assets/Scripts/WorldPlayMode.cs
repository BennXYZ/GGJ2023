using System;
using UnityEngine;

internal class WorldPlayMode : WorldMode
{
    Building selectedBuilding;

    public WorldPlayMode(World worldGameManager) : base(worldGameManager)
    {
    }

    public void MovePositive()
    {

    }

    public void MoveNegative()
    {

    }

    public override void Shutdown()
    {

    }

    public override void Startup()
    {
    }

    public override void Update()
    {
    }

    public override string GetBuildingTitle()
    {
        return "Bush";
    }

    public override string GetBuildingCapacity()
    {
        return "lol";
    }
}