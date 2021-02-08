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
            Vector2 toTarget = _hookPosition - GlobalPosition;
            Length = toTarget.Length();

            // Angle to the target position
            Rotation = toTarget.Angle();

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
            _head.Position = new Vector2(_tail.Points[_tail.Points.Length-1].x + _tail.Position.x , _head.Position.y);
        }
    }

    private Sprite _head;
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
        _head = GetNode<Sprite>("Head");
        _tail = GetNode<Line2D>("Tail");
        _tween = GetNode<Tween>("Tween");

        HookPosition = Utils.VECTOR_0;
        Length = 35.0f;
        _startLength = _head.Position.x - (_head.Texture.GetWidth()/2 * _head.Scale.x) + 4.0f;    // 4.0f is a offset
    }


    // Use to detect a key not defined in the Input Manager  (called only when a touch is pressed or released - not suitable for long press like run button)
    // Note : it's cleaner to define key in the Input Manager and use  Input.IsActionPressed("myaction")   in  _Process
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            // Close game if press Escape
            if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Space)
            {
                HookPosition = GetGlobalMousePosition();
            }
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}