using Godot;
using System;

public class Move : State
{
#region HEADER

    [Export] public Vector2 MaxSpeed_Default = new Vector2(500.0f, 900.0f);
    [Export] public float Gravity = 3000.0f;
    [Export] public float Inertia_Start = 1200.0f;
    [Export] public float Inertia_Stop = 1500.0f;
    [Export] public float Fall_Speed_Max = 1500.0f;     // to make the character fall quicker (more mass)

    public Vector2 MaxSpeed;
    public Vector2 Acceleration;
    public Vector2 Acceleration_Default { get; private set; }
    public Vector2 Decceleration;
    public Vector2 Decceleration_Default { get; private set; }
    public Vector2 Velocity;
    public Vector2 Direction { get; private set; }

    public bool isMoving = false;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        MaxSpeed = MaxSpeed_Default;

        Acceleration_Default = new Vector2(Inertia_Start, Gravity);
        Acceleration = Acceleration_Default;
        Decceleration_Default = new Vector2(Inertia_Stop, 0.0f);
        Decceleration = Decceleration_Default;

        Velocity = Utils.VECTOR_0;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    public override void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    { }

    public override void Exit_State()
    { }

    public override void Update(float delta)
    { }

    public override void Physics_Update(float delta)
    {
        _Movement_Left_Right(delta);

        //???? EmitSignal("player_moved", _root);  // for the camera ????  (see the end of https://gdquest.mavenseed.com/lessons/the-parent-move-state)
    }

    public override void Input_State(InputEvent @event)
    {
        _Movement_Jump(@event);
    }

    /// <summary>
    /// Movement on x axis
    /// </summary>
    /// <param name="delta">delta time</param>
    private void _Movement_Left_Right(float delta)
    {
        Direction = Utils.GetDirection_Platformer("L_left", "L_right", false);

        // Check if the player is moving
        isMoving = (Direction.x != 0.0f) ? true : false;

        // Move the player
        Velocity = Utils.CalculateVelocity(Velocity, MaxSpeed, Acceleration, Decceleration, Direction, delta);
        Velocity = Utils.StateMachine_Node.RootNode.MoveAndSlide(Velocity, Utils.VECTOR_FLOOR);
    }

    /// <summary>
    /// Make the player jump
    /// </summary>
    private void _Movement_Jump(InputEvent @event)
    {
        if (Utils.StateMachine_Node.RootNode.IsOnFloor() && @event.IsActionPressed("button_A"))
        {
            Godot.Collections.Dictionary<string,object> param = new Godot.Collections.Dictionary<string,object>();
            param.Add("impulse", true);

            Utils.StateMachine_Node.TransitionTo("Move/Air", param);
        }
    }
#endregion
}
