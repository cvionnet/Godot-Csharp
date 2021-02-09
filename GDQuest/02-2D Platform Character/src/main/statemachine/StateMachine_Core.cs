using Godot;

/// <summary>
/// Generic State Machine. Initializes states and delegates engine callbacks (_PhysicsProcess and _UnhandledInput) to the active state
/// </summary>
public class StateMachine_Core : Node
{
#region HEADER

    // A reference to the active State scene
    public IState ActiveState {
        get => _activeState;
        set {
            _activeState = value;
            _activeStateName = _activeState.GetStateName();
        }
    }

    // An empty dictionary to pass (as default) as 2nd parameter to TransitionTo()
    public Godot.Collections.Dictionary<string, object> TransitionToParam_Void {get; private set; }

    private IState _activeState;
    private string _activeStateName;    // to store the name of the Node (used in the DebugDock node)

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public void Initialize(NodePath pInitialState)
    {
        // Create a void transition parameter
        TransitionToParam_Void = new Godot.Collections.Dictionary<string, object>();
        TransitionToParam_Void.Add("", 0);

        // Set the initial state
        ActiveState = GetNode<IState>(pInitialState);
        _activeStateName = _activeState.GetStateName();

        _activeState.Enter_State(TransitionToParam_Void);
    }

    public void Update(float delta)
    {
        _activeState.Update(delta);
    }

    public void Physics_Update(float delta)
    {
        _activeState.Physics_Update(delta);
    }

    public void Input_State(InputEvent @event)
    {
        _activeState.Input_State(@event);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// Move from the previous state to a new state
    /// </summary>
    /// <param name="pTargetStatePath">The node path from the "StateMachine" node to the state (because we could have a hierarchie like "StateMachine" > "Move" > "Idle" </param>
    public void TransitionTo(string pTargetStatePath, Godot.Collections.Dictionary<string, object> pParam)
    {
        // Check if the path exists
        if (!HasNode(pTargetStatePath))
            return;

        // Get the new state, exit the previous one, set the new one as active and enter it
        IState new_state = GetNode<IState>(pTargetStatePath);
        _activeState.Exit_State();
        ActiveState = new_state;
        _activeState.Enter_State(pParam);
    }

#endregion
}
