using Godot;
using System;

public class Aim : State
{
#region HEADER

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void Physics_Update(float delta)
    {

-> 5 min 15

        Vector2 cast = Utils.GetDirectionBetween2Objects(Utils.StateMachine_Hook.RootNode.GlobalPosition, GetViewport().GetMousePosition());
        cast *= 
    }

    public override void Input_State(InputEvent @event)
    {}

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}