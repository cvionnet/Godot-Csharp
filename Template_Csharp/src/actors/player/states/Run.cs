using Godot;
using System;

public class Run : State
{
#region HEADER

    [Export] public float SpeedBoost = 200.0f;

    private Move _moveNode;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _moveNode = GetParent<Move>();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    public override void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    {
        _moveNode.Enter_State(pParam);
    }

    public override void Exit_State()
    {
        _moveNode.Exit_State();
    }

    public override void Update(float delta)
    {
        _moveNode.Update(delta);

        _Movement_Run();
    }

    public override void Physics_Update(float delta)
    {
        _moveNode.Physics_Update(delta);

        // Conditions of transition to Idle or Air states
        if (Utils.StateMachine_Node.RootNode.IsOnFloor() && !_moveNode.isMoving)
            Utils.StateMachine_Node.TransitionTo("Move/Idle", Utils.StateMachine_Node.TransitionToParam_Void);
        // _moveNode.Velocity.Length() : to deal with deceleration (force the character to stop when his velocity is close to 0)
        else if (!Utils.StateMachine_Node.RootNode.IsOnFloor() && _moveNode.Velocity.Length() < 1.0f)
            Utils.StateMachine_Node.TransitionTo("Move/Air", Utils.StateMachine_Node.TransitionToParam_Void);
    }

    public override void Input_State(InputEvent @event)
    {
        _moveNode.Input_State(@event);
    }

    /// <summary>
    /// Make the player run faster
    /// </summary>
    private void _Movement_Run()
    {
        if (Utils.StateMachine_Node.RootNode.IsOnFloor() && Input.IsActionPressed("button_X"))
            _moveNode.MaxSpeed.x = _moveNode.MaxSpeed_Default.x + SpeedBoost;
        else
            _moveNode.MaxSpeed = _moveNode.MaxSpeed_Default;
    }

#endregion
}
