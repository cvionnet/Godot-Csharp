using Godot;
using System;

public class Spawn : Node, IState
{
#region HEADER

    private Player _player;

    private Vector2 _startPosition = Utils.VECTOR_0;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        Initialize(Owner);

        _player = (Player)Owner;

        _startPosition = _player.Position;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    {
        _player.isActive = false;
        _player.Position = _startPosition;  // to respawn the character at the start of the level

        if (_player.Skin != null)
        {
            _player.Skin.PlayAnimation("spawn");
            _player.Skin.Connect("AnimationFinished", this, nameof(_on_Spawn_AnimationFinished));
        }
        else
        {
            // Force to display the Idle state
            _on_Spawn_AnimationFinished("");
        }
    }

    public void Exit_State()
    {
        _player.isActive = true;

        if (_player.Skin != null)
        {
            _player.Hook.Visible = true;
            _player.Skin.Disconnect("AnimationFinished", this, nameof(_on_Spawn_AnimationFinished));
        }
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

    public void _on_Spawn_AnimationFinished(string anim_name)
    {
        Utils.StateMachine_Player.TransitionTo("Move/Idle", Utils.StateMachine_Player.TransitionToParam_Void);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    async private void Initialize(Godot.Object pScene)
    {
        await ToSignal(pScene, "ready");
    }

#endregion
}