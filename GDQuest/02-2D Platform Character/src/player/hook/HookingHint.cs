using Godot;
using System;

[Tool]
public class HookingHint : Position2D
{
#region HEADER

    [Export] private Color InnerColor = Colors.Red;
    [Export] private float Radius = 10.0f;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        SetAsToplevel(true);    // Set the node as the 1st one
        Update();   // force to redraw the scene
    }

    public override void _Draw()
    {
        DrawCircle(Utils.VECTOR_0, Radius, InnerColor);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}