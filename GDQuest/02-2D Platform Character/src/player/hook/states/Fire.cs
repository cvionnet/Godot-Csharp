using Godot;
using Godot.Collections;
using System;

public class Fire : Node, IState
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
    {
        float power = 1.0f;

        _hook.CoolDownTimer.Connect("timeout", this, nameof(_onCooldownTimeout));
        _hook.CoolDownTimer.Start();

        HookTarget target = _hook.SnapDetector.Target;
        if (target != null)
        {
            _hook.Arrow.HookPosition = target.GlobalPosition;
            target.HookedFrom(_hook.GlobalPosition);

            // (from Charge state)
            if (pParam.ContainsKey("velocity_multiplier"))
                power = (float)pParam["velocity_multiplier"];

            _hook.EmitSignal("HookedOntoTarget", target.GlobalPosition, power);
        }
    }

    public void Exit_State()
    {
        _hook.CoolDownTimer.Disconnect("timeout", this, nameof(_onCooldownTimeout));
    }

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

    /// <summary>
    /// (from Hook) When the cooldown timer is out, switch to the Aim state
    /// </summary>
    public void _onCooldownTimeout()
    {
        Utils.StateMachine_Hook.TransitionTo("Aim", Utils.StateMachine_Hook.TransitionToParam_Void);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}