using Godot;
using Godot.Collections;
using System;

public class Fire : Node, IState
{
#region HEADER

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

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
    {}

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