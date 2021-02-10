using Godot;
using System;

public class Player : KinematicBody2D
{
#region HEADER

    public bool isActive {
        get => _isActive;
        set {
            _isActive = value;

            if (_collider == null)
                return;

            _collider.Disabled = _isActive;
            Hook.isActive = _isActive;
        }
    }

    public Player_Skin Skin;
    public Hook Hook;
    private StateMachine_Player _stateMachine;
    private CollisionShape2D _collider;

    private bool _isActive = true;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _stateMachine = GetNode<StateMachine_Player>("StateMachine");
        _collider = GetNode<CollisionShape2D>("CollisionShape2D");
        Hook = GetNode<Hook>("Hook");
        Skin = GetNode<Player_Skin>("Player_Skin");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}
