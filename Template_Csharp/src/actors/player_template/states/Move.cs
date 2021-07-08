using Godot;
using Nucleus;
using Nucleus.Physics;
using System;

public class Move_Template : Node, IState
{
#region HEADER

    [Export] public Vector2 MaxSpeed_Default = new Vector2(80.0f, 80.0f);
    [Export] public float Inertia_Start = 120.0f;
    [Export] public float Inertia_Stop = 150.0f;
    [Export] public float Gravity = 0.0f;              //for platformer=3000.0f, for top-down=0.0f
    [Export] public float Fall_Speed_Max = 0.0f;       //for platformer=1500.0f, for top-down=0.0f    // to make the character fall quicker (more mass)

    public Vector2 MaxSpeed;
    public Vector2 Acceleration;
    public Vector2 Acceleration_Default { get; private set; }
    public Vector2 Decceleration;
    public Vector2 Decceleration_Default { get; private set; }
    public Vector2 Velocity;
    public Vector2 Direction { get; private set; }

    public bool isMoving = false;

    private const bool IS_PLATFORMER = false;     //for platformer=true, for top-down=false

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        if(IS_PLATFORMER)
        {
            Acceleration_Default = new Vector2(Inertia_Start, Gravity);
            Decceleration_Default = new Vector2(Inertia_Stop, 0.0f);
        }
        else
        {
            Acceleration_Default = new Vector2(Inertia_Start, Inertia_Start);
            Decceleration_Default = new Vector2(Inertia_Stop, Inertia_Stop);
        }

        MaxSpeed = MaxSpeed_Default;
        Acceleration = Acceleration_Default;
        Decceleration = Decceleration_Default;

        Velocity = Nucleus_Utils.VECTOR_0;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    { }

    public void Exit_State()
    { }

    public void Update(float delta)
    { }

    public void Physics_Update(float delta)
    {
        _Movement_isPlayerMoving();

        if(isMoving)
            _Movement_UpdateVelocity(delta);
    }

    public void Input_State(InputEvent @event)
    {
        _Movement_Jump(@event);
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

    /// <summary>
    /// Check if the player is moving (direction (joypad) or velocity (acceleration/decceleration))
    /// </summary>
    private void _Movement_isPlayerMoving()
    {
        if(IS_PLATFORMER)
            Direction = Nucleus_Movement.GetMovingDirection("L_left", "L_right", false);
        else
            Direction = Nucleus_Movement.GetMovingDirection("L_left", "L_right", false, "L_up", "L_down");

        // Check if the player is moving : he has a direction (joypad) or a velocity (acceleration/decceleration)
        isMoving = (Velocity.x != 0.0f || Velocity.y != 0.0f || Direction.x != 0.0f || Direction.y != 0.0f) ? true : false;
    }

    /// <summary>
    /// Movements on axis
    /// </summary>
    /// <param name="delta">delta time</param>
    private void _Movement_UpdateVelocity(float delta)
    {
        // Calculate velocity and move the player
        if(IS_PLATFORMER)
        {
            Velocity = Nucleus_Movement.CalculateVelocity(Velocity, MaxSpeed, Acceleration, Decceleration, Direction, delta);
            Velocity = Nucleus_Utils.StateMachine_Template.RootNode.MoveAndSlide(Velocity, Nucleus_Utils.VECTOR_FLOOR);
        }
        else
        {
            Velocity = Nucleus_Movement.CalculateVelocity(Velocity, MaxSpeed, Acceleration, Decceleration, Direction, delta, MaxSpeed.y);
            Velocity = Nucleus_Utils.StateMachine_Template.RootNode.MoveAndSlide(Velocity);
        }
    }

    /// <summary>
    /// Make the player jump
    /// </summary>
    private void _Movement_Jump(InputEvent @event)
    {
        if (Nucleus_Utils.StateMachine_Template.RootNode.IsOnFloor() && @event.IsActionPressed("button_A"))
        {
            Godot.Collections.Dictionary<string,object> param = new Godot.Collections.Dictionary<string,object>();
            param.Add("impulse", true);

            Nucleus_Utils.StateMachine_Template.TransitionTo("Move/Air", param);
        }
    }

#endregion
}
