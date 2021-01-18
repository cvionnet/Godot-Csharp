using Godot;
using System;

public class CharacterFollow : KinematicBody2D
{
#region HEADER

    [Export] public float MaxSpeed = 500.0f;
    [Export] public float Mass = 2.0f;
    [Export] public float SlowRadius = 200.0f;
    [Export] public float Follow_Offset = 100.0f;        // to add an offset (=distance) between following nodes

    //** 2 options of following
    //      OPTION 1 : check "MasterTofollow" for a node in the Editor. All other nodes will follow the master, keeping "Follow_Offset" distance
    //      OPTION 2 : select a "NodeTofollow" for a node in the Editor. This node will only follow this node, keeping "Follow_Offset" distance (this creates a queue of nodes)
    [Export] public bool MasterTofollow;        // set the Node as the master
    [Export] public NodePath NodeTofollow;      // set the Node to follow

    private Sprite _sprite;
    private Vector2 _velocity = Utils.VECTOR_0;
    private Vector2 _target_global_position = Utils.VECTOR_0;
    private KinematicBody2D _nodeToFollow;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("TriangleRed");

        // Set the master node (the one to follow)
        if (MasterTofollow)
            Utils.TargetToFollow = this;

        // 
        if (NodeTofollow != null)
            _nodeToFollow = GetNode<KinematicBody2D>(NodeTofollow);

        SetPhysicsProcess(false);       // disable call to _PhysicsProcess until the player click anywhere
    }

    public override void _PhysicsProcess(float delta)
    {
        // Make the target (the cross) following the mouse cursor when click and drag
        if (MasterTofollow && Input.IsActionPressed("click"))
            _GetDestination();
        // For followers nodes, recalculate the destination every frame
        else if (MasterTofollow == false)
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
    /// Get the target's position where the character will move
    /// </summary>
    private void _GetDestination()
    {
        // The master node gets the mouse cursor position
        if (MasterTofollow)
        {
            _target_global_position = GetGlobalMousePosition();
        }
        // If this node follow another node, gets the node to follow position and keep the distance
        else if (_nodeToFollow != null)
        {
            _target_global_position = _nodeToFollow.GlobalPosition;
            _target_global_position = Utils.Steering_CalculateDistanceBetweenFollowers(_target_global_position, GlobalPosition, Follow_Offset);
        }
        // Else gets the master position and keep the distance
        else
        {
            _target_global_position = Utils.TargetToFollow.GlobalPosition;
            _target_global_position = Utils.Steering_CalculateDistanceBetweenFollowers(_target_global_position, GlobalPosition, Follow_Offset);
        }
    }

    /// <summary>
    /// Calculate velocity between the character and the target (cursor), then move the character
    /// </summary>
    private void _MoveCharacter()
    {
        // _velocity = Utils.Steering_Follow(_velocity, GlobalPosition, target_global_position, MaxSpeed);  // without slow radius
        _velocity = Utils.Steering_Follow(_velocity, GlobalPosition, _target_global_position, MaxSpeed, SlowRadius, Mass);

        if (_velocity == Utils.VECTOR_0)
        {
            SetPhysicsProcess(false);       // disable _PhysicsProcess if distance between target and character is too small
        }
        else
        {
            _velocity = MoveAndSlide(_velocity);
            _sprite.Rotation = _velocity.Angle();   // point the character direction towards the destination
        }
    }

#endregion
}