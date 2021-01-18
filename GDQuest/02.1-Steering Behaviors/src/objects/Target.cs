using Godot;
using System;

public class Target : Area2D
{
#region HEADER

    private AnimationPlayer _animationPlayer;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        Connect("body_entered", this, "_on_Body_Entered");
        Visible = false;
    }

    public override void _PhysicsProcess(float delta)
    {
        // Make the target following the mouse cursor when click and drag
        if (Input.IsActionPressed("click"))
            GlobalPosition = GetGlobalMousePosition();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("click"))
            _animationPlayer.Play("fadein");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    public void _on_Body_Entered(PhysicsBody2D body)
    {
        _animationPlayer.Play("fadeout");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}