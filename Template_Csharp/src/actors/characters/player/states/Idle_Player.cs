using Godot;
using Godot.Collections;
using Nucleus;
using System;
using System.Reflection;

/// <summary>
/// Responsible for :
/// - 
/// </summary>
public class Idle_Player : Node, IState
{
#region HEADER

    private Player _rootNode;
    private Move_Player _moveNode;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _moveNode = GetParent<Move_Player>();

        Initialize_Idle();
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

        _rootNode.CharacterAnimation.Play("idle");

        _moveNode.Enter_State(pRootNode, pParam);
    }

    public void Exit_State() => _moveNode.Exit_State();
    public void Update(float delta) => _moveNode.Update(delta);

    public void Physics_Update(float delta)
    {
        _moveNode.Physics_Update(delta);

        if(!_rootNode.CharacterProperties.IsMoving)
            Play_Idle();

        /*
        // Conditions of transition to Run or Air states
        if (Nucleus_Utils.StateMachine_Player.RootNode.IsOnFloor() && _moveNode.isMoving)
            Nucleus_Utils.StateMachine_Player.TransitionTo("Move/Run");
        else if (!Nucleus_Utils.StateMachine_Player.RootNode.IsOnFloor())
            Nucleus_Utils.StateMachine_Player.TransitionTo("Move/Air");
        */
    }

    public void Input_State(InputEvent @event) => _moveNode.Input_State(@event);
    public string GetStateName() => Name;

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Idle()
    { }

    private void Play_Idle()
    { }

#endregion
}
