using Godot;
using System;

public class CharacterFollow : KinematicBody2D
{
#region HEADER

    [Export] public float MaxSpeed = 500.0f;

    //[Signal] public delegate void MySignal(bool value1, int value2);

    private Sprite _sprite;
    private Vector2 _velocity = Utils.VECTOR_0;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("TriangleRed");
    }

    public override void _PhysicsProcess(float delta)
    {
        // Get mouse cursor position
        Vector2 target_global_position = GetGlobalMousePosition();

        // Calculate velocity between the character and the target (cursor), then move the character
        _velocity = Utils.Steering_Follow(_velocity, GlobalPosition, target_global_position, MaxSpeed);
        _velocity = MoveAndSlide(_velocity);

        // Point the character direction towards the destination
        _sprite.Rotation = _velocity.Angle();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}