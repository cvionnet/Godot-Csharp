using Godot;
using System;

public class Die : Node, IState
{
#region HEADER

    private Player _player;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _player = (Player)Owner;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    {
        _player.Hook.Visible = false;
        _player.Skin.PlayAnimation("die");
        _player.Skin.Connect("AnimationFinished", this, nameof(_on_Die_AnimationFinished));
    }

    public void Exit_State()
    {
        _player.Skin.Disconnect("AnimationFinished", this, nameof(_on_Die_AnimationFinished));
    }

    public void Update(float delta)
    {}

    public void Physics_Update(float delta)
    {}

    public void Input_State(InputEvent @event)
    {}

    public string GetStateName()
    {
        return this.Name;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    public void _on_Die_AnimationFinished(string anim_name)
    {
        Utils.StateMachine_Player.TransitionTo("Spawn", Utils.StateMachine_Player.TransitionToParam_Void);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}