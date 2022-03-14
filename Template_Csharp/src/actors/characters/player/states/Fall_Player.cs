using Godot;
using Godot.Collections;
using Nucleus;
using System;
using System.Reflection;

/// <summary>
/// Responsible for :
/// - counting time before respawning
/// - updating score (negative)
/// - positioning character on a safe platform (to respawn)
/// </summary>
public class Fall_Player : Node, IState
{
#region HEADER

    private Player _rootNode;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        Initialize_Fall();
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
        if (_rootNode == null) {
            _rootNode = pRootNode as Player;
        }

        Make_CharacterFall();
    }

    public void Exit_State() { }
    public void Update(float delta) { }
    public void Physics_Update(float delta) { }
    public void Input_State(InputEvent @event) { }
    public string GetStateName() => Name;

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// Make the character respawn
    /// </summary>
    private void onTimerFall_Timeout()
    {
        _rootNode.Position = new Vector2(100, 100);
        _rootNode.StateMachine.TransitionTo("Spawn");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Fall()
    { }

    private void Make_CharacterFall()
    {
        _rootNode.CharacterProperties.Update_Score(-5);
        _rootNode.CharacterProperties.Reset_Movement();
        _rootNode.Visible = false;
        _rootNode.Camera.Zoom_Camera(Nucleus_Utils.State_Manager.ZoomLevel_ZOOMOUT, 0.5f);

//        _rootNode.CallDeferred("queue_free");
    }

    #endregion
}
