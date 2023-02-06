using AutoRogue.Controllers;
using Godot;

namespace AutoRogue.Units;

public partial class Unit : CharacterBody2D
{
    [Export] protected UnitOwnerType unitOwner = UnitOwnerType.AI_OWNED;

    public UnitOwnerType GetUnitOwner()
    {
        return unitOwner;
    }
}