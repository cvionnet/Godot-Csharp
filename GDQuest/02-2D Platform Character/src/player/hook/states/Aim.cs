using Godot;
using Godot.Collections;
using System;

public class Aim : Node, IState
{
#region HEADER

    private Hook _hook;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _hook = (Hook)Owner;
    }

    /*
        public override void Physics_Update(float delta)
        {

    -> 5 min 15

            Vector2 cast = Utils.GetDirectionBetween2Objects(Utils.StateMachine_Hook.RootNode.GlobalPosition, GetViewport().GetMousePosition());
            cast *= 
        }
    */

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State(Dictionary<string, object> pParam)
    {}

    public void Exit_State()
    {}

    public void Input_State(InputEvent @event)
    {}

    public void Physics_Update(float delta)
    {
        GD.Print(_hook.isActive);

    }

    public void Update(float delta)
    {}

    public string GetStateName()
    {
        return this.Name;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}