using Godot;
using System;

public class HookTarget : Area2D
{
#region HEADER

    [Export] public bool isOneShot = false;

    [Signal] public delegate void HookedOntoFrom();

    //Enums
    //public enum Borders { Left, Right, Top, Bottom }

    //Public
    public bool isActive {
        get => _isActive;
        set {
            _isActive = value;
            activeColor = (_isActive) ? COLOR_ACTIVE : COLOR_INACTIVE;

            if (!_isActive && !isOneShot)
                _timer.Start();
        }
    }

    public Color activeColor {
        get => _activeColor;
        set {
            _activeColor = value;
            Update();
        }
    }

    //Private
    private Timer _timer;

    private Color COLOR_ACTIVE = new Color(0.9375f, 0.730906f, 0.025635f);
    private Color COLOR_INACTIVE = new Color(0.515625f, 0.484941f, 0.4552f);

    private bool _isActive = true;
    private Color _activeColor;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.Connect("timeout", this, nameof(_on_Timer_timeout));

        _activeColor = COLOR_INACTIVE;
    }

    public override void _Draw()
    {
        DrawCircle(Utils.VECTOR_0, 12.0f, activeColor);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS
    public void HookedFrom(Vector2 pHookPosition)
    {
        isActive = false;
        EmitSignal(nameof(HookedOntoFrom), pHookPosition);
    }

    private void _on_Timer_timeout()
    {
        isActive = true;
    }

#endregion
}
