using Godot;
using System;

public class Enemy : Actor
{
#region HEADER

    [Export] public int ScoreEnemy = 50;

    private StateManager _stateManager;
    private Vector2 _snap = new Vector2(0,1) * 50.0f;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        // To use the Autoload file named StateManager
        _stateManager = GetNode<StateManager>("/root/StateManager");

        // Disable physics. Will be reactivated by the VisibilityEnabler2D node
        SetPhysicsProcess(false);

        Velocity.x = -Speed.x;
    }

    public override void _PhysicsProcess(float delta)
    {
        // Apply gravity
        Velocity.y += Gravity;

        // Move the enemy (only use the y vector)
        //velocity.y = MoveAndSlide(velocity, FLOOR_NORMAL).y;

        // Use a vector to snap the player on slopes (https://gdquest.mavenseed.com/lessons/coding-slope-movement)
        base.Velocity.y = MoveAndSlideWithSnap(base.Velocity, _snap, base.FLOOR_NORMAL, true, 4, Mathf.Pi/3.0f).y;

        // Invert enemy movement when it hits a wall
        if (IsOnWall())
            Velocity.x *= -1.0f;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

/// <summary>
/// When an object touch the enemy   (kill it ?)
/// </summary>
/// <param name="body">The object that collides with (generally the Player) </param>
public void _on_Area2D_body_entered(PhysicsBody2D body)
{
    // Quit if the player do not hit the enemy from ABOVE the area
    if (body.GlobalPosition.y > GetNode<Area2D>("Area2D").GlobalPosition.y)
        return;

    // Desactivate collision detection to prevent false positive collisions
    GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);   // .Disabled = true;

    _Died();
}

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

/// <summary>
/// When the enemie die
/// </summary>
private void _Died()
{
    _stateManager.Score += ScoreEnemy;

    // Delete the node
    QueueFree();
}

#endregion
}
