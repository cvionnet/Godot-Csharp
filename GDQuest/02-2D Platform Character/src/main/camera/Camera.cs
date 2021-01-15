using Godot;
using System;

/// <summary>
/// To move a child camera (ShakingCamera) based on the player's input
/// </summary>
public class Camera : Position2D
{
#region HEADER

    [Export] public Vector2 Offset = new Vector2(300.0f, 300.0f);
    [Export] public Vector2 Mouse_Range = new Vector2(100.0f, 500.0f);

    //Private
    private Camera2D _shakingCamera;
    private bool _isActive = true;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _shakingCamera = GetNode<Camera2D>("ShakingCamera");
    }

    public override void _PhysicsProcess(float delta)
    {
        _Update_Position(Utils.VECTOR_0);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

/// <summary>
/// Updates the camera rig's position based on the player's state and controller position
/// </summary>
/// <param name="pVelocity"></param>
private void _Update_Position(Vector2 pVelocity)
{
    if (!_isActive)
        return;

    Vector2 mouse_position = GetLocalMousePosition();
    float distance_ratio = Mathf.Clamp(mouse_position.Length(), Mouse_Range.x, Mouse_Range.y) / Mouse_Range.y;

    _shakingCamera.Position = distance_ratio * mouse_position.Normalized() * Offset;
}

#endregion
}
