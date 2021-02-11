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

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State(Dictionary<string, object> pParam)
    {}

    public void Exit_State()
    {}

    public void Input_State(InputEvent @event)
    {
        if (@event.IsActionPressed("hook") && _hook.CanHook())
        {
            //Utils.StateMachine_Hook.TransitionTo("Aim/Fire", Utils.StateMachine_Hook.TransitionToParam_Void);
            Utils.StateMachine_Hook.TransitionTo("Aim/Charge", Utils.StateMachine_Hook.TransitionToParam_Void);
        }
    }

    public void Physics_Update(float delta)
    {
        // Project a raycast to see if there is an obstacle (eg : a wall) between the player and the point to hook
        Vector2 cast = _hook.GetAimDirection() * _hook.Raycast.CastTo.Length();

        // Update the angle towards the target
        float angle = cast.Angle();

        // Set the new direction and rotate the arrow towards the target
        _hook.Raycast.CastTo = cast;
        _hook.Arrow.Rotation = angle;
        _hook.SnapDetector.Rotation = angle;

        _hook.Raycast.ForceRaycastUpdate();
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