using Godot;
using System;

[Tool]
public class Portal : Area2D
{
#region HEADER

    [Export] public PackedScene NextScene;

    private AnimationPlayer _animPlayer = null;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the engine creates object in memory
    //public override void _Init()
    //{ }

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        //Preload the node
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    //public override void _Process(float delta)
    //{ }

    //public override void _PhysicsProcess(float delta)
    //{ }

    public override string _GetConfigurationWarning()
    {
        return (NextScene == null) ? "Next Scene must not be empty !" : ""; // "Add your warning message here";
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// When the player hit the portal
    /// </summary>
    /// <param name="body"></param>
    public void _on_Portal_body_entered(PhysicsBody2D body)
    {
        _Teleport();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    async private void _Teleport()
    {
        _animPlayer.Play("fadein");

        await ToSignal(_animPlayer, "animation_finished");
        GetTree().ChangeSceneTo(NextScene);
    }

#endregion
}
