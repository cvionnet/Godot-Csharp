using Godot;
using System;

public class Coin : Area2D
{
#region HEADER

    [Export] public int ScoreCoin = 10;

    private StateManager _stateManager;
    private AnimationPlayer _animPlayer = null;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS
    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        // To use the Autoload file named StateManager
        _stateManager = GetNode<StateManager>("/root/StateManager");

        //Preload the node
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

/// <summary>
/// When the player hit the coin, launch an animation and queuefree (called in the animation)
/// </summary>
/// <param name="body"></param>
public void _on_Coin_body_entered(PhysicsBody2D body)
{
    _animPlayer.Play("fadeout");
    _stateManager.Score += ScoreCoin;
}

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}
