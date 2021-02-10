using Godot;
using System;

public class Dash : Node, IState
{
#region HEADER

    [Export] private float DashSpeed = 1500.0f;

    private Timer _timer;

    private Vector2 _velocity = Utils.VECTOR_0;
    private Vector2 _direction = Utils.VECTOR_0;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State(Godot.Collections.Dictionary<string, object> pParam)
    {
        _timer.Connect("timeout", this, nameof(_on_Timer_Timeout));

        // Get the direction from Air state
        if (pParam.ContainsKey("direction"))
            _direction = (Vector2)pParam["direction"];

        _timer.Start();
    }

    public void Exit_State()
    {
        _timer.Disconnect("timeout", this, nameof(_on_Timer_Timeout));
    }

    public void Update(float delta)
    {}

    public void Physics_Update(float delta)
    {
        _velocity = Utils.StateMachine_Player.RootNode.MoveAndSlide(_direction * DashSpeed, Utils.VECTOR_FLOOR);
    }

    public void Input_State(InputEvent @event)
    {}

    public string GetStateName()
    {
        return this.Name;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    // Decrease the velocity when the time is out (eg : 0.1s). New velocity is done in the Air state
    public void _on_Timer_Timeout()
    {
        Godot.Collections.Dictionary<string,object> param = new Godot.Collections.Dictionary<string,object>();
        param.Add("velocity", _velocity/2);
        Utils.StateMachine_Player.TransitionTo("Move/Air", param);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}