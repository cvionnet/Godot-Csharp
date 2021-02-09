using Godot;
using System;

public class Idle_Template : Node, IState
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
        if (Utils.StateMachine_Player.RootNode.IsOnFloor() && _moveNode.isMoving)
            Utils.StateMachine_Player.TransitionTo("Move/Run", Utils.StateMachine_Player.TransitionToParam_Void);
        else if (!Utils.StateMachine_Player.RootNode.IsOnFloor())
            Utils.StateMachine_Player.TransitionTo("Move/Air", Utils.StateMachine_Player.TransitionToParam_Void);
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
