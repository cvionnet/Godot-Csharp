using Godot;
using System;

public class Idle_Template : State
{
#region HEADER

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

        _moveNode.MaxSpeed = _moveNode.MaxSpeed_Default;
        //_moveNode.Velocity = Utils.VECTOR_0;      // not needed with the use of decceleration in Move.cs
    }

    public override void Exit_State()
    {
        _moveNode.Exit_State();
    }

    public override void Update(float delta)
    {
        _moveNode.Update(delta);
    }

    public override void Physics_Update(float delta)
    {
        _moveNode.Physics_Update(delta);

        // Conditions of transition to Run or Air states
        if (Utils.StateMachine_Node.RootNode.IsOnFloor() && _moveNode.isMoving)
            Utils.StateMachine_Node.TransitionTo("Move/Run", Utils.StateMachine_Node.TransitionToParam_Void);
        else if (!Utils.StateMachine_Node.RootNode.IsOnFloor())
            Utils.StateMachine_Node.TransitionTo("Move/Air", Utils.StateMachine_Node.TransitionToParam_Void);
    }

    public override void Input_State(InputEvent @event)
    {
        _moveNode.Input_State(@event);
    }

#endregion
}
