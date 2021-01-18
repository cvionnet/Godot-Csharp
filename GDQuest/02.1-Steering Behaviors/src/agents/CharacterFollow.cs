using Godot;
using System;

public class CharacterFollow : KinematicBody2D
{
#region HEADER

    [Export] public float MaxSpeed = 500.0f;
    [Export] public float SlowRadius = 200.0f;

    //[Signal] public delegate void MySignal(bool value1, int value2);

    private Sprite _sprite;
    private Vector2 _velocity = Utils.VECTOR_0;
    private Vector2 _target_global_position = Utils.VECTOR_0;
#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("TriangleRed");

        SetPhysicsProcess(false);       // disable call to _PhysicsProcess until the player click anywhere
    }

    public override void _PhysicsProcess(float delta)
    {
        // Make the target (the cross) to follow the mouse cursor when click and drag
        if (Input.IsActionPressed("click"))
            _target_global_position = GetGlobalMousePosition();      // get mouse cursor position

        // Calculate velocity between the character and the target (cursor), then move the character
        // _velocity = Utils.Steering_Follow(_velocity, GlobalPosition, target_global_position, MaxSpeed);  // without slow radius
        _velocity = Utils.Steering_Follow(_velocity, GlobalPosition, _target_global_position, MaxSpeed, SlowRadius);
        if (_velocity == Utils.VECTOR_0)
        {
            SetPhysicsProcess(false);       // disable _PhysicsProcess if distance between target and character is too small
        }
        else
        {
            _velocity = MoveAndSlide(_velocity);

            // Point the character direction towards the destination
            _sprite.Rotation = _velocity.Angle();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("click"))
        {
            _target_global_position = GetGlobalMousePosition();      // get mouse cursor position
            SetPhysicsProcess(true);
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}