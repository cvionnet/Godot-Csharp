using Godot;

public class SnapDetector : Area2D
{
#region HEADER

    public HookTarget Target {
        get => _target;
        set {
            _target = value;
            _hookingHint.Visible = HasTarget();

            _hookingHint.GlobalPosition = _target != null ? _target.GlobalPosition : _hookingHint.GlobalPosition;
        }
    }

    private Position2D _hookingHint;
    private RayCast2D _raycast;

    private HookTarget _target;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _hookingHint = GetNode<Position2D>("HookingHint");
        _raycast = GetNode<RayCast2D>("RayCast2D");

        _raycast.SetAsToplevel(true);
    }

    public override void _PhysicsProcess(float delta)
    {
        Target = FindBestTarget();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    public bool HasTarget()
    {
        return Target != null;
    }

    /// <summary>
    /// Get the closest HookTarget (hook objects in the Game scene)
    /// </summary>
    /// <returns></returns>
    public HookTarget FindBestTarget()
    {
        HookTarget closestTarget = null;

        // Get a list of all targets inside the collision shape (collision mask set only on HookTargets in the Editor)
        Godot.Collections.Array targets = GetOverlappingAreas();
        foreach (HookTarget target in targets)
        {
            if (!target.isActive)
                continue;       // go directly to the next 'target' in the foreach

            // Check if there is no obstacle to the target
            _raycast.GlobalPosition = GlobalPosition;
            _raycast.CastTo = target.GlobalPosition - _raycast.GlobalPosition;
            //_raycast.ForceUpdateTransform();
            if (_raycast.IsColliding())
                continue;

            // Get the target
            closestTarget = target;
            break;
        }

        return closestTarget;
    }

#endregion
}