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
    /// In the children class : public override void Enter_State(Godot.Collections.Dictionary<string, object> pParam) {}
    /// </summary>
    /// <param name="pParam">To pass various parameters (eg : "speed", 100.0f) </param>
    public virtual void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    { }

    /// <summary>
    /// To cleanup the state before transition to another one
    /// In the children class : public override void Exit_State() {}
    /// </summary>
    public virtual void Exit_State()
    { }

    /// <summary>
    /// In the children class : public override void Update(float delta) {}
    /// </summary>
    public virtual void Update(float delta)
    { }

    /// <summary>
    /// In the children class : public override void Physics_Update(float delta) {}
    /// </summary>
    public virtual void Physics_Update(float delta)
    { }

    /// <summary>
    /// In the children class : public override void Input_State(InputEvent @event) {}
    /// </summary>
    public virtual void Input_State(InputEvent @event)
    { }

#endregion
}