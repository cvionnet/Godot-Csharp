using System;
using System.Reflection;
using Godot;
using Godot.Collections;
using Nucleus;

/// <summary>
/// Responsible for :
/// - transitioning to Idle
/// - initializing Camera default zoom
/// </summary>
public class Spawn_Player : Node, IState
{
#region HEADER

    private Player _rootNode;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _SceneReady();
        Initialize_Spawn();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State<T>(T pRootNode, Dictionary<string, object> pParam = null)
    {
        if (pRootNode == null || pRootNode.GetType() != typeof(Player))
        {
            Nucleus_Utils.Error($"State Machine root node is null or type not expected ({pRootNode.GetType()})", new NullReferenceException(), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return;
        }
        if (_rootNode == null) _rootNode = pRootNode as Player;

        Enter_CharacterEntrance();






        // TODO: ajouter une animation pour apparaitre et utiliser  _on_Spawn_AnimationFinished
        _rootNode.StateMachine.TransitionTo("Move/Idle");




        /*
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
        */
    }

    public void Exit_State() { }
    public void Update(float delta) { }
    public void Physics_Update(float delta) { }
    public void Input_State(InputEvent @event) { }
    public string GetStateName() => Name;

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    public void _on_Spawn_AnimationFinished(string anim_name)
    {
        _rootNode.StateMachine.TransitionTo("Move/Idle", null);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// Wait for the owner to be ready (owner = Node at the top of the scene), to be sure to access safely to nodes
    /// </summary>
    async private void _SceneReady() => await ToSignal(Owner, "ready");

    private void Initialize_Spawn()
    { }

    private void Enter_CharacterEntrance()
    {
        // Zoom effect when the player appears
        _rootNode.Camera.Zoom_Camera(Nucleus_Utils.State_Manager.ZoomLevel_GAME, 0.4f);
        _rootNode.Visible = true;
    }

#endregion
}