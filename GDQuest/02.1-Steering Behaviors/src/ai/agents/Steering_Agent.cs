using Godot;
using System;

public class Steering_Agent : KinematicBody2D
{
#region HEADER

    [Export] public float MaxSpeed = 500.0f;
    [Export] public float Mass = 2.0f;
    [Export] public float SlowRadius = 200.0f;          // radius the node will start to slow down
    [Export] public float Follow_Offset = 100.0f;       // to add an offset (=distance) between following nodes
    [Export] public float FleeRadius = 200.0f;          // radius the node will flee from the player

    [Export] public bool IsLeader = false;      // set the Node as the leader. All other nodes will follow the leader, keeping "Follow_Offset" distance
    [Export] public NodePath NodeToFollow;      // set the Node to follow. This node will only follow the selected node, keeping "Follow_Offset" distance (this creates a queue of nodes)
    [Export] public bool IsFlee = false;        // the node will flee (run away) from the leader
    [Export] public bool IsWander = false;      // the node will follow it's own random way

    private Sprite _sprite;
    private Vector2 _velocity = Utils.VECTOR_0;
    private Vector2 _targetGlobalPosition = Utils.VECTOR_0;
    private Node2D _nodeToFollow;

    private bool _isLeader = false;
    private bool _isNodeFollower = false;
    private bool _isFlee = false;
    private bool _isWander = false;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("Triangle");

        _Init_FollowMode();

        SetPhysicsProcess(false);       // disable call to _PhysicsProcess until the player click anywhere
    }

    public override void _PhysicsProcess(float delta)
    {
        // Make the target (the cross) following the mouse cursor when click and drag
        if (_isLeader && Input.IsActionPressed("click"))
            _GetDestination();
        // For followers nodes, recalculate the destination every frame
        else if (_isLeader == false)
            _GetDestination();

        _MoveCharacter();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("click"))
        {
            _GetDestination();
            SetPhysicsProcess(true);
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// Initialize the type of node (leader, follower ...)
    /// </summary>
    private void _Init_FollowMode()
    {
        // Set the leader node (the one to follow)
        if (IsLeader)
        {
            _isLeader = true;
            Utils.LeaderToFollow = this;
        }

        // If a dedicated node to follow has been set
        if (NodeToFollow != null)
        {
            _isNodeFollower = true;
            _nodeToFollow = GetNode<Node2D>(NodeToFollow);
        }

        if (IsFlee)
            _isFlee = true;

        if (IsWander)
        {
            // Create a timer to set a new destination every XX time
            Timer wanderTimer = new Timer();
            AddChild(wanderTimer);
            wanderTimer.WaitTime = 2.0f;
            wanderTimer.Connect("timeout", this, nameof(_Wander_GetDestination));
            wanderTimer.Start();

            _isWander = true;
        }
    }

    /// <summary>
    /// Get the target's position where the character will move
    /// </summary>
    private void _GetDestination()
    {
        // Leader node : target is the mouse cursor position
        if (_isLeader)
            _targetGlobalPosition = GetGlobalMousePosition();
        // Node follow another node : target is the node to follow and keep the distance
        else if (_isNodeFollower)
            _targetGlobalPosition = Utils.Steering_CalculateDistanceBetweenFollowers(_nodeToFollow.GlobalPosition, GlobalPosition, Follow_Offset);
        // Node flee : target to run away is the leader position
        else if (_isFlee)
            _targetGlobalPosition = Utils.LeaderToFollow.GlobalPosition;
        // Node follow leader : target is the leader position and keep the distance
        else if (!_isFlee && !_isWander)
            _targetGlobalPosition = Utils.Steering_CalculateDistanceBetweenFollowers(Utils.LeaderToFollow.GlobalPosition, GlobalPosition, Follow_Offset);
        // Stay at the same position
        else if (!_isWander)
            _targetGlobalPosition = GlobalPosition;
    }

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
            if (_isFlee)
                _velocity = Utils.Steering_Flee(_velocity, GlobalPosition, _targetGlobalPosition, MaxSpeed, FleeRadius, Mass);
            else
                _velocity = Utils.Steering_Seek(_velocity, GlobalPosition, _targetGlobalPosition, MaxSpeed, SlowRadius, Mass);

            // Move the character
            if (_velocity != Utils.VECTOR_0)
            {
                _velocity = MoveAndSlide(_velocity);
                _sprite.Rotation = _velocity.Angle();   // point the character direction towards the destination
            }
        }
   }

#endregion
}