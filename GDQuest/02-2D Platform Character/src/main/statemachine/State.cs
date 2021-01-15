using Godot;
using System;

/// <summary>
/// State interface to use in Hierarchical State Machines
/// </summary>
public class State : Node
{
#region HEADER

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// To initialize the state
    /// </summary>
    /// <param name="pParam">To pass various parameters (eg : "speed", 100.0f) </param>
    public virtual void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    { }

    /// <summary>
    /// To cleanup the state before transition to another one
    /// </summary>
    public virtual void Exit_State()
    { }

    public virtual void Update(float delta)
    { }

    public virtual void Physics_Update(float delta)
    { }

    public virtual void Input_State(InputEvent @event)
    { }

#endregion
}