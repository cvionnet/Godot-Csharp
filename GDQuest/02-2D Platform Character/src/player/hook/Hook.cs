using Godot;
using System;

public class Hook : Position2D
{
#region HEADER

    [Signal] private delegate void HookedOntoTarget(Vector2 targetGlobalPosition, float power);

    public bool isActive { get; set; } = true;

    public RayCast2D Raycast;
    public Arrow Arrow;
    public SnapDetector SnapDetector;
    public Timer CoolDownTimer;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        Raycast = GetNode<RayCast2D>("RayCast2D");
        Arrow = GetNode<Arrow>("Arrow");
        SnapDetector = GetNode<SnapDetector>("SnapDetector");
        CoolDownTimer = GetNode<Timer>("Cooldown");
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
    public bool CanHook()
    {
        return isActive && SnapDetector.HasTarget() && CoolDownTimer.IsStopped();
    }

    /// <summary>
    /// Get the direction between the hook and the mouse cursor
    /// </summary>
    /// <returns>A Vector2 to represent the direction</returns>
    public Vector2 GetAimDirection()
    {
        return Utils.GetDirectionBetween_2_Objects(GlobalPosition, GetGlobalMousePosition());
    }

#endregion
}