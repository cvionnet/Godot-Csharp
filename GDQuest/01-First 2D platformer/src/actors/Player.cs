using Godot;
using System;

public class Player : Actor
{
#region HEADER

    [Export] public float StompImpulse = 1000.0f;

    private StateManager _stateManager;
    private bool _isJumpInterrupted = false;
    private Vector2 _snap;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS


    public override void _Ready()
    {
        // To use the Autoload file named StateManager
        _stateManager = GetNode<StateManager>("/root/StateManager");
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector2 direction = _GetInputs();

        base.Velocity = _CalculateMoveVelocity(base.Velocity, base.Speed, direction);
        //base.velocity = MoveAndSlide(base.velocity, base.FLOOR_NORMAL);

        // Create a vector to snap the player on slopes (https://gdquest.mavenseed.com/lessons/coding-slope-movement)
        if (direction.y == -1.0f)
            _snap = new Vector2(0,0);   // to allow the player to jump when he is on a slope (see MoveAndSlideWithSnap doc)
        else
            _snap = new Vector2(0,1) * 50.0f;
        base.Velocity.y = MoveAndSlideWithSnap(base.Velocity, _snap, base.FLOOR_NORMAL, true, 4, Mathf.Pi/3.0f).y;



        // Limit the y velocity
        //Velocity.y = Mathf.Min(Speed.y, Velocity.y);  // = if (Speed.y > Velocity.y) { Velocity.y = Velocity.y; }

        //base._PhysicsProcess(delta);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

/// <summary>
/// To make the player rebound when he hit an enemy
/// </summary>
/// <param name="area"></param>
public void _on_EnemyDetector_area_entered(Area2D area)
{
    base.Velocity = _CalculateStompVelocity(base.Velocity, StompImpulse);
}

/// <summary>
/// When an object touch the player   (kill it ?)
/// </summary>
/// <param name="body">The object that collides with (generally the Enemy) </param>
public void _on_EnemyDetector_body_entered(PhysicsBody2D body)
{
    _Died();
}

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

/// <summary>
/// Use inputs to get player directions (left / right / jump)
/// </summary>
/// <returns>A vector2 direction</returns>
private Vector2 _GetInputs()
{
    // x axis : if player move right, x = 1.0f   and if player move left, x = -1.0f
    // y axis : if player press jump button, reverse gravity
    float x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
    float y = (Input.IsActionJustPressed("jump") && IsOnFloor() ) ? -1.0f : 0.0f;

    // True if the player release jump button while the sprite is in a middle of a jump
    _isJumpInterrupted = Input.IsActionJustReleased("jump") && Velocity.y < 0.0f;

    return new Vector2(x, y);
}

/// <summary>
/// Set the player x (left / right) and y (jump) axis values
/// </summary>
/// <param name="pVelocity"></param>
/// <param name="pSpeed"></param>
/// <param name="pDirection"></param>
/// <returns>A vector2 with the new velocity</returns>
private Vector2 _CalculateMoveVelocity(Vector2 pVelocity, Vector2 pSpeed, Vector2 pDirection)
{
    Vector2 new_velocity = pVelocity;

    // Apply gravity
    new_velocity.y += Gravity; // * GetPhysicsProcessDeltaTime();   // GetPhysicsProcessDeltaTime() = same as passing delta as parameter

    // x axis : Player left / right
    new_velocity.x = pSpeed.x * pDirection.x;

    // y axis : player jump
    if (pDirection.y == -1.0f)
        new_velocity.y = pSpeed.y * pDirection.y;

    // To adjust the jump high to the time the player press the button  (press longer = jump higher)
    if (_isJumpInterrupted)
        new_velocity.y = 0.0f;    // stop the y movement (then gravity will make the player fall)

    return new_velocity;
}

/// <summary>
/// Set the player y (rebound) axis value
/// </summary>
/// <param name="pVelocity"></param>
/// <param name="pStompImpulse"></param>
/// <returns>A vector2 with the new velocity</returns>
private Vector2 _CalculateStompVelocity(Vector2 pVelocity, float pStompImpulse)
{
    Vector2 new_velocity = pVelocity;
    new_velocity.y = -pStompImpulse;
    return new_velocity;
}

/// <summary>
/// When the player die
/// </summary>
private void _Died()
{
    _stateManager.Death++;

    // Delete the node
    QueueFree();
}

#endregion

}