using Godot;
using System;

public class Player : KinematicBody2D
{
#region HEADER

    [Signal] public delegate void Minion_Move();

    private AnimatedSprite _animation;
    private Vector2 _newPosition;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _animation = GetNode<AnimatedSprite>("AnimatedSprite");
        _animation.Connect("animation_finished", this, nameof(_on_AnimationFinished));

        // Steering AI - Set the player node as leader
        Utils.LeaderToFollow = this;
    }

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("click"))
		{
			// Move the player to the mouse coordinates
			_PlayerMove("walk", GetGlobalMousePosition());
		}
	}

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// Called at every end of an animation
    /// </summary>
    public void _on_AnimationFinished()
    {
        // When the "walk" animation is play once, move the player and play "idle" animation
        if (_animation.Animation == "walk")
        {
            Position = _newPosition;
            Utils.LeaderToFollow.GlobalPosition = Position;

            _animation.Play("idle");

            // To force minions to follow the player
            EmitSignal(nameof(Minion_Move));
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// When a signal ask to change the animation
    /// </summary>
    /// <param name="pAnimation"></param>
    private void _PlayerMove(string pAnimation, Vector2 pNewPosition)
    {
        _newPosition = pNewPosition;
        _animation.Play(pAnimation);
    }

#endregion
}