using Godot;
using System;

public class Game : Node
{
#region HEADER

    private Player _player;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _player = GetNode<Player>("Player");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("debug_restart"))
            GetTree().ReloadCurrentScene();
        if (@event.IsActionPressed("debug_spawn"))
            Utils.StateMachine_Player.TransitionTo("Spawn", Utils.StateMachine_Player.TransitionToParam_Void);
        if (@event.IsActionPressed("debug_die"))
            Utils.StateMachine_Player.TransitionTo("Die", Utils.StateMachine_Player.TransitionToParam_Void);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}