using AutoRogue.Controllers;
using AutoRogue.Input;
using AutoRogue.Units;
using Godot;
using Godot.Collections;

namespace AutoRogue.PlayerController.DragController;

public partial class DragController : Node2D
{
    private Unit currentlyHoveredUnit;
    private Unit currentlySelectedUnit;

    private Vector2 selectionDragOffset;
    private UnitInputMouseState mouseInputState = UnitInputMouseState.NONE;

    private void SelectHoveredUnit()
    {
        if (currentlySelectedUnit != null)
        {
            currentlySelectedUnit.Modulate = Colors.Orange;
            currentlySelectedUnit.ZIndex = 10;
        }
    }

    private void DeselectCurrentlySelectedUnit()
    {
        if (currentlySelectedUnit != null)
        {
            currentlySelectedUnit.Modulate = Colors.White;
            currentlySelectedUnit.ZIndex = 0;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (mouseInputState != UnitInputMouseState.DRAGGING)
        {
            UpdateUnitHovering();
        }

        UpdateUnitSelection();
        UpdateDraggedUnit();
    }

    private void UpdateDraggedUnit()
    {
        if (currentlySelectedUnit != null && mouseInputState == UnitInputMouseState.DRAGGING)
        {
            Vector2 globalPosition = GetLocalMousePosition();
            currentlySelectedUnit.GlobalPosition = globalPosition;
        }
    }

    private void UpdateUnitSelection()
    {
        if (Godot.Input.IsActionJustPressed("Select") && mouseInputState == UnitInputMouseState.NONE)
        {
            // Select
            if (currentlyHoveredUnit != currentlySelectedUnit)
            {
                DeselectCurrentlySelectedUnit();
                currentlySelectedUnit = currentlyHoveredUnit;
                SelectHoveredUnit();
            }

            // Start Dragging
            if (currentlySelectedUnit != null)
            {
                mouseInputState = UnitInputMouseState.DRAGGING;
            }
        }
        else if (Godot.Input.IsActionJustReleased("Select") && mouseInputState == UnitInputMouseState.DRAGGING)
        {
            // Stop Dragging
            mouseInputState = UnitInputMouseState.NONE;
        }
    }

    private void UpdateUnitHovering()
    {
        Unit hoveredUnit = PerformRaycast();
        if (hoveredUnit != currentlyHoveredUnit)
        {
            StopHoveringCurrentUnit();
            currentlyHoveredUnit = hoveredUnit;
            StartHoveringCurrentUnit();
        }
    }

    private Unit PerformRaycast()
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        PhysicsPointQueryParameters2D query = new PhysicsPointQueryParameters2D();
        query.Position = GetGlobalMousePosition();

        Array<Dictionary> results = spaceState.IntersectPoint(query);
        if (results.Count > 0)
        {
            return ProcessHits(results);
        }

        return null;
    }

    private Unit ProcessHits(Array<Dictionary> hitResults)
    {
        for (int i = 0; i < hitResults.Count; i++)
        {
            GodotObject obj = (GodotObject)hitResults[i]["collider"];
            if (obj is Unit s && s.GetUnitOwner() == UnitOwnerType.PLAYER_OWNED)
            {
                return s;
            }
        }

        return null;
    }

    private void StopHoveringCurrentUnit()
    {
        if (currentlyHoveredUnit != null && currentlyHoveredUnit != currentlySelectedUnit)
        {
            currentlyHoveredUnit.Modulate = Colors.White;
        }
    }

    private void StartHoveringCurrentUnit()
    {
        if (currentlyHoveredUnit != null && currentlyHoveredUnit != currentlySelectedUnit)
        {
            currentlyHoveredUnit.Modulate = Colors.Green;
        }
    }
}