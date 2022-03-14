using Godot;
using Nucleus;
using Nucleus.AI;

/// <summary>
/// Extends the Character class with properties about character's movements (max speed, acceleration ...)
/// </summary>
public partial class CCharacter
{
    public Vector2 MaxSpeed { get; set; }

    // To move the character using steering methods from Nucleus.Steering (eg : for PNJ)
    public Nucleus_Steering Steering { get; set; } = new Nucleus_Steering();

    public bool IsMoving { get; set; } = false;
    public bool IsOrientationHorizontalInverted { get; private set; } = false;      // Left or Right
    public bool IsOrientationVerticalInverted { get; private set; } = false;        // Up or Down

    public Vector2 Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            // Check if orientation is inverted (horizontal / vertical  -  to flip Sprite when moving)
            IsOrientationHorizontalInverted = DetectOrientation(_direction.x, IsOrientationHorizontalInverted);
            IsOrientationVerticalInverted = DetectOrientation(_direction.y, IsOrientationVerticalInverted);
        }
    }
    private Vector2 _direction;
    
    public Vector2 Acceleration { get; private set; }
    public Vector2 Deceleration { get; private set; }

    public Vector2 Velocity
    {
        get => _velocity;
        set
        {
            _velocity = value;
            // Check if orientation is inverted (horizontal / vertical  -  to flip Sprite when moving)
            IsOrientationHorizontalInverted = DetectOrientation(_velocity.x, IsOrientationHorizontalInverted);
            IsOrientationVerticalInverted = DetectOrientation(_velocity.y, IsOrientationVerticalInverted);
        }
    }
    private Vector2 _velocity;
    
    // (optional) To add some inertia
    public float Inertia_Start {
        get => _inertia_Start;
        set {
            _inertia_Start = value;
            Acceleration = _isPlateformer ? new Vector2(Inertia_Start, Gravity) : new Vector2(Inertia_Start, Inertia_Start);
        }
    }
    private float _inertia_Start;

    public float Inertia_Stop {
        get => _inertia_Stop;
        set {
            _inertia_Stop = value;
            Deceleration = _isPlateformer ? new Vector2(Inertia_Stop, 0.0f) : new Vector2(Inertia_Stop, Inertia_Stop) ;
        }
    }
    private float _inertia_Stop;    
    
#region PLATEFORMER
    public float MaxFall_Speed { get; private set; }        // to make the character fall quicker (add more mass)

    public float Gravity {
        get => _gravity;
        set {
            _gravity = value;
            Acceleration = _isPlateformer ? new Vector2(Inertia_Start, Gravity) : new Vector2(Inertia_Start, Inertia_Start);
        }
    }
    private float _gravity;
#endregion

    //*-------------------------------------------------------------------------*//

    /// <summary>
    /// Reset all movements to zero
    /// </summary>
    public void Reset_Movement()
    {
        Direction = Nucleus_Utils.VECTOR_0;
        Velocity = Nucleus_Utils.VECTOR_0;
    }

    /// <summary>
    /// Check if orientation must be inverted (horizontal or vertical)
    /// Used to flip Sprite in the correct orientation when they are moving
    /// </summary>
    /// <param name="direction">The direction of the character</param>
    /// <param name="previousOrientationStatus">The previous </param>
    /// <returns>True if the orientation must be inverted</returns>
    private bool DetectOrientation(float direction, bool previousOrientationStatus)
    {
        bool newOrientationStatus = previousOrientationStatus;      // to keep the same orientation if the character is not moving 

        if (direction > 0 && previousOrientationStatus)
            newOrientationStatus = false;
        else if (direction < 0 && !previousOrientationStatus)
            newOrientationStatus = true;

        return newOrientationStatus;
    }    
}