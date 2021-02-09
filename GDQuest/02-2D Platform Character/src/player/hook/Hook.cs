using Godot;
using System;

public class Hook : Position2D
{
#region HEADER

//    public bool isActive = true;
    public bool isActive { get; set; } = true;

    private RayCast2D _raycast;
    private Node2D _arrow;
    private Area2D _snapDetector;
    private Timer _coolDownTimer;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _raycast = GetNode<RayCast2D>("RayCast2D");
        _arrow = GetNode<Node2D>("Arrow");
        _snapDetector = GetNode<Area2D>("SnapDetector");
        _coolDownTimer = GetNode<Timer>("Cooldown");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// Get a True if the hook is active
    /// </summary>
    /// <returns>A boolean</returns>
/*
    public bool CanHook()
    {
        return isActive && _snapDetector.HasTarget() && _coolDownTimer.IsStopped();
    }
*/
    /// <summary>
    /// Get the direction between 2 objects (eg : pActualPosition can be the player)
    /// </summary>
    /// <returns>A Vector2 to represent the direction</returns>
    public Vector2 GetAimDirection()
    {
        return Utils.GetDirectionBetween2Objects(GlobalPosition, GetGlobalMousePosition());
    }

#endregion
}