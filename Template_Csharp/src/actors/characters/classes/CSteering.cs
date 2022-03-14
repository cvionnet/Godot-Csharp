
using Godot;
using Nucleus;
/// <summary>
/// Properties used to move the character using steering methods from Nucleus.Steering (eg : for PNJ's statemachine)
/// </summary>
public class CSteering
{
    public float Mass { get; }
    public float Slow_Radius { get; }       // radius the node will start to slow down

    public float Speed { get; set; }

    public Vector2 TargetGlobalPosition { get; set; }

    public CSteering(float mass = 10.0f, float slow_radius = 80.0f)
    {
        Mass = mass;
        Slow_Radius = slow_radius;

        TargetGlobalPosition = Nucleus_Utils.VECTOR_0;
//        Speed = 0.0f;
    }
}