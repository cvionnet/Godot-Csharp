using Godot;
using System;

public class Minion : KinematicBody2D
{
#region HEADER

    public bool IsWaiting {
        get => _isWaiting;
        set {
            _isWaiting = value;

            if (!_isAttacking)
            {
                if (_isWaiting)
                    _sprite.Play("idle");
                else
                    _sprite.Play("walk");
            }

            SetPhysicsProcess(!_isWaiting);
        }
    }

    public bool IsAttacking {
        get => _isAttacking;
        set {
            _isAttacking = value;

            if (_isAttacking)
            {
                IsWaiting = false;
                _sprite.Play("attack");

                _audioAttack.Play();
            }
        }
    }

    private bool _isWaiting;
    private bool _isAttacking;

    private AnimatedSprite _sprite;
    private Timer _timer;
    private AudioStreamPlayer _audioAttack;

    private Boss _boss;

    private Vector2 _targetGlobalPosition = Utils.VECTOR_0;
    private Vector2 _velocity = Utils.VECTOR_0;

    private float _maxSpeed;
    private float _mass;
    private float _slowRadius;          // radius the node will start to slow down
    private readonly float _follow_Offset = 20.0f;       // to add an offset (=distance) between following nodes

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public Minion()
    {
        // Generate a Minion with random values
        InitRandomMinion();
    }

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _sprite.SpeedScale = Utils.Rnd.RandfRange(0.8f, 1.2f);  // animation speed

        _audioAttack = GetNode<AudioStreamPlayer>("Audio/Attack");

        _boss = GetParent().GetParent().GetNode<Boss>("Boss");

        // Connect to the signal send by the Player
        GetParent().GetParent().GetNode<Player>("Player").Connect("Minion_Move", this, nameof(_on_MinionMove));
        _timer.Connect("timeout", this, nameof(_on_TimerTimeout));

        // Connect to the signal send by the Boss
        GetParent().GetParent().GetNode<Boss>("Boss").Connect("Minion_Attack", this, nameof(_on_MinionAttack));

        // Connect to the signal send by the HUD
        GetParent().GetParent().GetNode<Control>("CanvasLayer/HUD").Connect("Player_Follow", this, nameof(_on_PlayerFollow));

        IsWaiting = false;
        IsAttacking = false;
    }

    public override void _Process(float delta)
    {
        if (IsAttacking)
            // Get a destination vector to the boss
            _targetGlobalPosition = Utils.Steering_CalculateDistanceBetweenFollowers(_boss.Position, GlobalPosition, _follow_Offset);
        else
            // Get a destination vector to the player
            _targetGlobalPosition = Utils.Steering_CalculateDistanceBetweenFollowers(Utils.LeaderToFollow.GlobalPosition, GlobalPosition, _follow_Offset);
    }

    public override void _PhysicsProcess(float delta)
    {
        // Perform calcul only if the node have to move  (else  _PhysicsProcess  is disabled)
        if (!IsWaiting || IsAttacking) //(_targetGlobalPosition != GlobalPosition)
        {
            // Use Steering Bahaviour to move to the player
            _velocity = Utils.Steering_Seek(_velocity, GlobalPosition, _targetGlobalPosition, _maxSpeed, _slowRadius, _mass);

            if (_velocity != Utils.VECTOR_0)
            {
                _velocity = MoveAndSlide(_velocity);

                // Check if the minion have to stop walking around the player (not aroujd the boss)
                if (!IsAttacking)
                    _StopMinionMovement();
            }
            else
            {
                if (!IsAttacking)
                    IsWaiting = true;
            }
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// Send by the player to force minions to move
    /// </summary>
    public void _on_MinionMove()
    {
        IsWaiting = false;

        // Cancel the timer to idle if the player moved
        if (_timer.TimeLeft > 0.0f)
            _timer.Stop();
    }

    /// <summary>
    /// Send by the boss to force minions to attack
    /// </summary>
    public void _on_MinionAttack()
    {
        IsAttacking = true;
    }

    /// <summary>
    /// To pass the minion as idle
    /// </summary>
    public void _on_TimerTimeout()
    {
        IsWaiting = true;
    }

    /// <summary>
    /// Send by the boss to force minions to attack
    /// </summary>
    public void _on_PlayerFollow()
    {
        if (IsAttacking)
        {
            IsAttacking = false;
            IsWaiting = false;
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// Create a minion with unique properties
    /// </summary>
    public void InitRandomMinion()
    {
        _maxSpeed = Utils.Rnd.RandfRange(70.0f, 120.0f);
        _mass = Utils.Rnd.RandfRange(1.0f, 4.0f);
        _slowRadius = Utils.Rnd.RandfRange(30.0f, 50.0f);
    }

    /// <summary>
    /// Check if the object the minion collide with is stopped. If true, also stop after a short time
    /// </summary>
    private void _StopMinionMovement()
    {
        for (int i = 0; i < GetSlideCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);

            // Check if the collider object is type of Minion
            if (((Node)collision.Collider).GetType() == typeof(Minion))
            {
                // if the collider is stopped and the timer is not already started
                if ( ((Minion)collision.Collider).IsWaiting && _timer.TimeLeft <= 0.0f )
                {
                        _timer.Start();
                        break;
                }
            }
        }
    }

#endregion
}