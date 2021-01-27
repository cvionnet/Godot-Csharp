using Godot;
using System;

public class Air_Template : State
{
#region HEADER

    [Export] public float Acceleration_X = 5000.0f;
    [Export] public float JumpImpulse = 900.0f;
    [Export] public int Jump_Max_Count = 2;

    private Move_Template _moveNode;
    private int _jumpCount = 0;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _moveNode = GetParent<Move_Template>();
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

        // Just after starting a straight jump, allow the character to move on x axis in the air
        //      If 0.0f => the player jump, then cannot move on x axis until landing on the ground
        _moveNode.Acceleration.x = Acceleration_X;

        // To preserve the character inertia in the air
        if (pParam.ContainsKey("velocity"))
        {
GD.Print("AIR - Use param 'velocity' ");
            _moveNode.Velocity = (Vector2)pParam["velocity"];       // to override the default velocity
            _moveNode.MaxSpeed.x = Mathf.Max(Mathf.Abs(_moveNode.Velocity.x), _moveNode.MaxSpeed_Default.x);
        }

        // To make the player jump
        if (pParam.ContainsKey("impulse"))
        {
            //_moveNode.Velocity += Utils.CalculateJumpVelocity(_moveNode.Velocity, _moveNode.MaxSpeed, (float)pParam["impulse"]);
            _Movement_Jump();
        }
    }

    public override void Exit_State()
    {
        _moveNode.Acceleration = _moveNode.Acceleration_Default;
        _jumpCount = 0;

        _moveNode.Exit_State();
    }

    public override void Update(float delta)
    {
        _moveNode.Update(delta);
    }

    public override void Physics_Update(float delta)
    {
        _moveNode.Physics_Update(delta);

        // Conditions of transition to Idle or Run states
        if (Utils.StateMachine_Node.RootNode.IsOnFloor())
        {
            // _moveNode.Velocity.Length() : to deal with deceleration (force the character to stop when his velocity is close to 0)
            string target_state = (_moveNode.isMoving && _moveNode.Velocity.Length() < 1.0f) ? "Move/Idle" : "Move/Run";
            Utils.StateMachine_Node.TransitionTo(target_state, Utils.StateMachine_Node.TransitionToParam_Void);
        }
    }

    public override void Input_State(InputEvent @event)
    {
        _moveNode.Input_State(@event);

        if (_jumpCount < Jump_Max_Count && @event.IsActionPressed("button_A"))
            _Movement_Jump();
    }

    /// <summary>
    /// Make the player jump
    /// </summary>
    private void _Movement_Jump()
    {
        _moveNode.Velocity += Utils.CalculateJumpVelocity(_moveNode.Velocity, _moveNode.MaxSpeed, JumpImpulse);
        _jumpCount++;
    }

#endregion
}
