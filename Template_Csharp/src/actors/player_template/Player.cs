using Godot;
using System;

public class Player_Template : KinematicBody2D
{
#region HEADER

    public bool isActive {
        get => _isActive;
        set {
            _isActive = value;
            _collider.Disabled = _isActive;
        }
    }

    private StateMachine_Template _stateMachine;
    private CollisionShape2D _collider;

    private bool _isActive = true;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _stateMachine = GetNode<StateMachine_Template>("StateMachine");
        _collider = GetNode<CollisionShape2D>("CollisionShape2D");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}
