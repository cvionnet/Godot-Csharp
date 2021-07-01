using Godot;
using Nucleus;
using System;

public class Idle_Template : Node, IState
{
#region HEADER

    private Move_Template _moveNode;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _moveNode = GetParent<Move_Template>();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    {
        _moveNode.Enter_State(pParam);

        _moveNode.MaxSpeed = _moveNode.MaxSpeed_Default;
        //_moveNode.Velocity = Utils.VECTOR_0;      // not needed with the use of decceleration in Move.cs
    }

    public void Exit_State()
    {
        _moveNode.Exit_State();
    }

    public void Update(float delta)
    {
        _moveNode.Update(delta);
    }

    public void Physics_Update(float delta)
    {
        _moveNode.Physics_Update(delta);

        // Conditions of transition to Run or Air states
        if (Nucleus_Utils.StateMachine_Template.RootNode.IsOnFloor() && _moveNode.isMoving)
            Nucleus_Utils.StateMachine_Template.TransitionTo("Move/Run", Nucleus_Utils.StateMachine_Template.TransitionToParam_Void);
        else if (!Nucleus_Utils.StateMachine_Template.RootNode.IsOnFloor())
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

#endregion
}
