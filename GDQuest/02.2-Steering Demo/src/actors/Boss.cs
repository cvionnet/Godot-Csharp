using Godot;
using System;

public class Boss : KinematicBody2D
{
#region HEADER
    [Export] public float MaxSpeed = 20.0f;
    [Export] public float Mass = 10.0f;

    [Signal] public delegate void Minion_Attack();

    public bool IsMouseHover { get; private set; }

    private Vector2 _targetGlobalPosition = Utils.VECTOR_0;
    private Vector2 _velocity = Utils.VECTOR_0;
    private float _slowRadius = 40.0f;          // radius the node will start to slow down

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        GetNode<Area2D>("MouseDetection").Connect("input_event", this, "_onInputEvent");
        GetNode<Area2D>("MouseDetection").Connect("mouse_entered", this, "_onMouseEntered");
        GetNode<Area2D>("MouseDetection").Connect("mouse_exited", this, "_onMouseExited");

        // Create a timer to set a new destination every XX time
        Timer wanderTimer = new Timer();
        AddChild(wanderTimer);
        wanderTimer.WaitTime = 10.0f;
        wanderTimer.Connect("timeout", this, nameof(_Wander_GetDestination));
        wanderTimer.Start();
    }

    public override void _PhysicsProcess(float delta)
    {
        _MoveCharacter();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// When click on the boss
    /// </summary>
    public void _onInputEvent(Node viewport, InputEvent @event, int shape_idx)
    {
		if (@event.IsActionPressed("click"))
		{
            EmitSignal(nameof(Minion_Attack));
		}
    }

    public void _onMouseEntered()
    {
        IsMouseHover = true;
    }

    public void _onMouseExited()
    {
        IsMouseHover = false;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// Get the Wander node a new destination
    /// </summary>
    private void _Wander_GetDestination()
    {
        _targetGlobalPosition = new Vector2(Utils.Rnd.RandfRange(10.0f, GetViewport().Size.x), Utils.Rnd.RandfRange(10.0f, GetViewport().Size.y));
    }

    /// <summary>
    /// Calculate velocity between the character and the target (cursor), then move the character
    /// </summary>
    private void _MoveCharacter()
    {
        // Perform calcul only if the node have to move
        if (_targetGlobalPosition != GlobalPosition)
        {
            _velocity = Utils.Steering_Seek(_velocity, GlobalPosition, _targetGlobalPosition, MaxSpeed, _slowRadius, Mass);

            // Move the character
            if (_velocity != Utils.VECTOR_0)
                _velocity = MoveAndSlide(_velocity);
        }
   }

#endregion
}