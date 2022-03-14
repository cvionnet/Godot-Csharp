using Godot;
using Godot.Collections;
using Nucleus;
using System;
using System.Reflection;

/// <summary>
/// Responsible for :
/// - setting the maxspeed and velocity to a boost value (within a limited timing)
/// </summary>
public class Dash_Player : Node, IState
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

        Initialize_Dash();
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
        if (_rootNode == null)
        {
            _rootNode = pRootNode as Player;
            _rootNode.TimerDashDuration.Connect("timeout", this, nameof(onTimerDash_Timeout));
        }

        _moveNode.Enter_State(pRootNode, pParam);

        Play_Dash();
    }

    public void Exit_State() => _moveNode.Exit_State();
    public void Update(float delta) => _moveNode.Update(delta);
    public void Physics_Update(float delta) => _moveNode.Physics_Update(delta);
    public void Input_State(InputEvent @event) => _moveNode.Input_State(@event);
    public string GetStateName() => Name;

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    // Reset values after a dash
    public void onTimerDash_Timeout()
    {
        _rootNode.CharacterProperties.MaxSpeed *= 1/_rootNode.CharacterProperties.Dash_SpeedBoost;
        _rootNode.CharacterProperties.Velocity *= 1/_rootNode.CharacterProperties.Dash_SpeedBoost;
        _rootNode.CharacterProperties.IsDashing = false;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Dash()
    { }

    /// <summary>
    /// Make the player run faster
    /// </summary>
    private void Play_Dash()
    {
        _rootNode.CharacterProperties.IsDashing = true;
        _rootNode.TimerDashDuration.Start();

        //TODO:   play animation

        _rootNode.CharacterProperties.MaxSpeed *= _rootNode.CharacterProperties.Dash_SpeedBoost;
        _rootNode.CharacterProperties.Velocity *= _rootNode.CharacterProperties.Dash_SpeedBoost;

        /*
        if (Nucleus_Utils.StateMachine_Player.RootNode.IsOnFloor() && Input.IsActionPressed("button_X"))
            _moveNode.MaxSpeed.x = _moveNode.MaxSpeed_Default.x + SpeedBoost;
        else
            _moveNode.MaxSpeed = _moveNode.MaxSpeed_Default;
        */
    }

    #endregion
}
