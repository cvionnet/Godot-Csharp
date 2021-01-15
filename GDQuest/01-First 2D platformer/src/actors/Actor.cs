using Godot;
using System;

public class Actor : KinematicBody2D
{

#region HEADER

    [Export] public Vector2 Speed = new Vector2(800.0f, 1000.0f);
    [Export] public float Gravity = 50.0f;

    public Vector2 FLOOR_NORMAL = new Vector2(0.0f,-1.0f);    // Used for using MoveAndSlide() in platformers
    public Vector2 Velocity = new Vector2(0.0f,0.0f);

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS
    
#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS
    
#endregion
}
