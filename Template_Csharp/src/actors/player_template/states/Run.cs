using Godot;
using Nucleus;
using System;

public class Run_Template : Node, IState
{
#region HEADER

    [Export] public float SpeedBoost = 200.0f;

    private Move_Template _moveNode;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _moveNode = GetParent<Move_Template>();

        Initialize_RunPlayer();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    {
        _moveNode.Enter_State(pParam);
    }

    public void Exit_State()
    {
        _moveNode.Exit_State();
    }

    public void Update(float delta)
    {
        _moveNode.Update(delta);

        _Movement_Run();
    }

    public void Physics_Update(float delta)
    {
        _moveNode.Physics_Update(delta);

        // Conditions of transition to Idle or Air states
        if (Nucleus_Utils.StateMachine_Template.RootNode.IsOnFloor() && !_moveNode.isMoving)
            Nucleus_Utils.StateMachine_Template.TransitionTo("Move/Idle", Nucleus_Utils.StateMachine_Template.TransitionToParam_Void);
        // _moveNode.Velocity.Length() : to deal with deceleration (force the character to stop when his velocity is close to 0)
        else if (!Nucleus_Utils.StateMachine_Template.RootNode.IsOnFloor() && _moveNode.Velocity.Length() < 1.0f)
            Nucleus_Utils.StateMachine_Template.TransitionTo("Move/Air", Nucleus_Utils.StateMachine_Template.TransitionToParam_Void);
    }

    public void Input_State(InputEvent @event)
    {
        _moveNode.Input_State(@event);
    }

    public string GetStateName()
    {
        return this.Name;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_RunPlayer()
    { }

    /// <summary>
    /// Make the player run faster
    /// </summary>
    private void _Movement_Run()
    {
        if (Nucleus_Utils.StateMachine_Template.RootNode.IsOnFloor() && Input.IsActionPressed("button_X"))
            _moveNode.MaxSpeed.x = _moveNode.MaxSpeed_Default.x + SpeedBoost;
        else
            _moveNode.MaxSpeed = _moveNode.MaxSpeed_Default;
    }

#endregion
}
