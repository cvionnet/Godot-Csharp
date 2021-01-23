using Godot;
using System;

public class Frog : KinematicBody2D
{
#region HEADER

    private AnimationPlayer _animationPlayer;
    private Vector2 _targetGlobalPosition = Utils.VECTOR_0;
    private Vector2 _velocity = Utils.VECTOR_0;
    private float _maxSpeed;
    private float _mass;
    private readonly float _fleeRadius = 20.0f;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("idle");

        _animationPlayer.PlaybackSpeed = Utils.Rnd.RandfRange(0.8f, 1.2f);
        _maxSpeed = Utils.Rnd.RandfRange(50.0f, 70.0f);
        _mass = Utils.Rnd.RandfRange(1.0f, 2.0f);

        SetPhysicsProcess(false);
    }

    //public override void _Process(float delta)
    //{}

    public override void _PhysicsProcess(float delta)
    {
        _GetDestination();
        _MoveCharacter();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS
    /// <summary>
    /// Get the target's position where the character will move
    /// </summary>
    private void _GetDestination()
    {
        _targetGlobalPosition = Utils.LeaderToFollow.GlobalPosition;
    }

    /// <summary>
    /// Calculate velocity between the character and the target (cursor), then move the character
    /// </summary>
    private void _MoveCharacter()
    {
        // Perform calcul only if the node have to move
        if (_targetGlobalPosition != GlobalPosition)
        {
            _velocity = Utils.Steering_Flee(_velocity, GlobalPosition, _targetGlobalPosition, _maxSpeed, _fleeRadius, _mass);

            // Move the character
            if (_velocity != Utils.VECTOR_0)
            {
                _velocity = MoveAndSlide(_velocity);
                _animationPlayer.Advance(0.0f);
                _animationPlayer.Play("jump", -1, Utils.Rnd.RandfRange(1.1f, 1.4f));
            }
            else
            {
                _animationPlayer.Play("idle");
            }
        }
   }

#endregion
}