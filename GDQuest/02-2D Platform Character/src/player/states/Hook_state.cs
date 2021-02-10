using Godot;
using System;

public class Hook_state : Node, IState
{
#region HEADER

    [Export] private float HookMaxSpeed = 1500.0f;
    [Export] private float PushWhenArrive = 900.0f;

    private Vector2 _target_GlobalPosition = Utils.VECTOR_INF;
    private Vector2 _velocity = Utils.VECTOR_0;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    {
        if (pParam.ContainsKey("target_global_position"))
            _target_GlobalPosition = (Vector2)pParam["target_global_position"];

        if (pParam.ContainsKey("velocity"))
            _velocity = (Vector2)pParam["velocity"];
    }

    public void Exit_State()
    {
        _target_GlobalPosition = Utils.VECTOR_INF;
        _velocity = Utils.VECTOR_0;
    }

    public void Update(float delta)
    {
    }

    public void Physics_Update(float delta)
    {
        // Use Steering Behaviour to "move" the player to the hook, adding a pushing force when he arrive on it
        _velocity = Utils.Steering_Seek(_velocity, Utils.StateMachine_Player.RootNode.GlobalPosition, _target_GlobalPosition, HookMaxSpeed);
        _velocity = (_velocity.Length() > PushWhenArrive) ? _velocity : _velocity.Normalized() * PushWhenArrive;

        _velocity = Utils.StateMachine_Player.RootNode.MoveAndSlide(_velocity, Utils.VECTOR_FLOOR);

        // Get distance to the target
        float distance = Utils.GetDistanceBetween_2_Objects(Utils.StateMachine_Player.RootNode.GlobalPosition, _target_GlobalPosition);

        // Check if we are less that 1 frame (_velocity.Length() * delta) from the target to apply the push and change state
        if (distance < (_velocity.Length() * delta))
        {
            _velocity = _velocity.Normalized() * PushWhenArrive;

	        Godot.Collections.Dictionary<string,object> param = new Godot.Collections.Dictionary<string,object>();
            param.Add("velocity", _velocity);
            Utils.StateMachine_Player.TransitionTo("Move/Air", param);
        }

        if (Utils.StateMachine_Player.RootNode.IsOnFloor())
            Utils.StateMachine_Player.TransitionTo("Move/Run", Utils.StateMachine_Player.TransitionToParam_Void);
    }

    public void Input_State(InputEvent @event)
    {
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