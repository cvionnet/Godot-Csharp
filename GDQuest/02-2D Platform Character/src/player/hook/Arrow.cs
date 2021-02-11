using Godot;
using System;

public class Arrow : Node2D
{
#region HEADER

    public Vector2 HookPosition {
        get => _hookPosition;
        set {
            _hookPosition = value;

            // Get the destination position
            Length = Utils.GetDistanceBetween_2_Objects(GlobalPosition, _hookPosition);

            // Angle to the target position
            Rotation = Utils.GetAngleTo(GlobalPosition, _hookPosition);

            // Animate the hook to go back to the start position
            _tween.InterpolateProperty(this, "Length", _length, _startLength, 0.25f, Tween.TransitionType.Quad, Tween.EaseType.Out);
            _tween.Start();
        }
    }

    public float Length {
        get => _length;
        set {
            _length = value;

            // Change coordinates of the last point in the line array to draw a longer line
            _tail.SetPointPosition(_tail.Points.Length-1, new Vector2(_length, 0.0f));

            // Move the head position
            Head.Position = new Vector2(_tail.Points[_tail.Points.Length-1].x + _tail.Position.x , Head.Position.y);
        }
    }

    public Sprite Head;
    private Line2D _tail;
    private Tween _tween;

    private Vector2 _hookPosition;
    private float _length;
    private float _startLength;     // the distance between 0,0 and the base of the head
#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        Head = GetNode<Sprite>("Head");
        _tail = GetNode<Line2D>("Tail");
        _tween = GetNode<Tween>("Tween");

        HookPosition = Utils.VECTOR_0;
        Length = 35.0f;
        _startLength = Head.Position.x - (Head.Texture.GetWidth()/2 * Head.Scale.x) + 4.0f;    // 4.0f is a offset
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}