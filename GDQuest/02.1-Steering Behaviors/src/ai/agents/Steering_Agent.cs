using Godot;
using System;

public class Steering_Agent : KinematicBody2D
{
#region HEADER

    [Export] public float MaxSpeed = 500.0f;
    [Export] public float Mass = 2.0f;
    [Export] public float SlowRadius = 200.0f;          // radius the node will start to slow down
    [Export] public float Follow_Offset = 100.0f;       // to add an offset (=distance) between following nodes
    [Export] public bool IsRunner = false;            // the node will flee (run away) from the player
    [Export] public float FleeRadius = 200.0f;          // radius the node will flee from the player

    //** 2 options of following
    //      OPTION 1 : check "LeaderTofollow" for a node in the Editor. All other nodes will follow the leader, keeping "Follow_Offset" distance
    //      OPTION 2 : select a "NodeTofollow" for a node in the Editor. This node will only follow the selected node, keeping "Follow_Offset" distance (this creates a queue of nodes)
    [Export] public bool LeaderToFollow;        // set the Node as the leader
    [Export] public NodePath NodeToFollow;      // set the Node to follow

    private Sprite _sprite;
    private Vector2 _velocity = Utils.VECTOR_0;
    private Vector2 _targetGlobalPosition = Utils.VECTOR_0;
    private Node2D _nodeToFollow;

    private bool _isLeader = false;
    private bool _isFollower = false;
    private bool _isRunner = false;         // run away

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
    /// Set the leader or dedicated node
    /// </summary>
    private void _Init_FollowMode()
    {
        // Set the leader node (the one to follow)
        if (LeaderToFollow)
        {
            _isLeader = true;
            Utils.LeaderToFollow = this;
        }

        // If a dedicated node to follow has been set
        if (NodeToFollow != null)
        {
            _isFollower = true;
            _nodeToFollow = GetNode<Node2D>(NodeToFollow);
        }

        if (IsRunner)
            _isRunner = true;
    }

    /// <summary>
    /// Get the target's position where the character will move
    /// </summary>
    private void _GetDestination()
    {
        // The leader node gets the mouse cursor position
        if (_isLeader)
        {
            _targetGlobalPosition = GetGlobalMousePosition();
        }
        // If this node follow another node, gets the node to follow position and keep the distance
        else if (_isFollower)   // _nodeToFollow != null)
        {
            _targetGlobalPosition = _nodeToFollow.GlobalPosition;
            _targetGlobalPosition = Utils.Steering_CalculateDistanceBetweenFollowers(_targetGlobalPosition, GlobalPosition, Follow_Offset);
        }
        // Else gets the leader position and keep the distance  (except for runners)
        else if (!_isRunner)
        {
            _targetGlobalPosition = Utils.LeaderToFollow.GlobalPosition;
            _targetGlobalPosition = Utils.Steering_CalculateDistanceBetweenFollowers(_targetGlobalPosition, GlobalPosition, Follow_Offset);
        }
        // Else if the node is a runner, gets the run away vector if it is closed to the leader
        else if (_isRunner)
        {
            _targetGlobalPosition = Utils.LeaderToFollow.GlobalPosition;
        // Stay at the same position
        }
        else
        {
            _targetGlobalPosition = GlobalPosition;
        }
    }

    /// <summary>
    /// Calculate velocity between the character and the target (cursor), then move the character
    /// </summary>
    private void _MoveCharacter()
    {
        // Perform calcul only if the node have to move
        if (_targetGlobalPosition != GlobalPosition)
        {
            if (_isRunner)
                _velocity = Utils.Steering_Flee(_velocity, GlobalPosition, _targetGlobalPosition, MaxSpeed, FleeRadius, Mass);
            else
                _velocity = Utils.Steering_Seek(_velocity, GlobalPosition, _targetGlobalPosition, MaxSpeed, SlowRadius, Mass);

            if (_velocity == Utils.VECTOR_0)
            {
                //SetPhysicsProcess(false);       // disable _PhysicsProcess if distance between target and character is too small
            }
            else
            {
                _velocity = MoveAndSlide(_velocity);
                _sprite.Rotation = _velocity.Angle();   // point the character direction towards the destination
            }
        }
   }

#endregion
}