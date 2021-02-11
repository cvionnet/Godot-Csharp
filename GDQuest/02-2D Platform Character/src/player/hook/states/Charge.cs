using Godot;
using System;

public class Charge : Node, IState
{
#region HEADER

    [Export] private float MaxPower = 2.0f;

    private const float STARTING_POWER = 1.0f;

    private Hook _hook;
    private Aim _aim;
    private Timer _timerCharge;

    private float _power = STARTING_POWER;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _hook = (Hook)Owner;
        _aim = (Aim)GetParent();
        _timerCharge = GetNode<Timer>(GetParent().GetNode("InputTimer").GetPath());

        _timerCharge.Connect("timeout", this, nameof(_on_TimerCharging_Timeout));
    }

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    {
        _power = STARTING_POWER;
        _timerCharge.Start();
    }

    public void Exit_State()
    {
        _hook.Arrow.Head.Modulate = Colors.White;
        _timerCharge.Stop();
    }

    public void Update(float delta)
    {}

    public void Physics_Update(float delta)
    {
        // Accumulate power to a maximum value
        _power = Mathf.Clamp(_power + delta, STARTING_POWER, MaxPower);

        // Change the color of the arrow
        float power_percent = ((_power - STARTING_POWER) / (MaxPower - STARTING_POWER));
        Color color_target = Colors.Blue;
        color_target.a = power_percent;
        _hook.Arrow.Head.Modulate = Colors.White.Blend(color_target);

        _aim.Physics_Update(delta);
    }

    public void Input_State(InputEvent @event)
    {
        if (@event.IsActionReleased("hook") && _hook.CanHook())
        {
            _Fire();
        }
        else if (@event.IsActionReleased("hook"))
        {
            Utils.StateMachine_Hook.TransitionTo("Aim", Utils.StateMachine_Hook.TransitionToParam_Void);
        }
    }

    public string GetStateName()
    {
        return this.Name;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    // Force the hook to fire after a certain amount of time
    public void _on_TimerCharging_Timeout()
    {
        _Fire();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void _Fire()
    {
        Godot.Collections.Dictionary<string,object> param = new Godot.Collections.Dictionary<string,object>();
        param.Add("velocity_multiplier", _power);
        Utils.StateMachine_Hook.TransitionTo("Aim/Fire", param);
    }

#endregion
}