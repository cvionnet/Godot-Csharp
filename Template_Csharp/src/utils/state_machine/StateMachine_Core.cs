using Godot;
using Godot.Collections;

/// <summary>
/// Responsible for :
/// - storing the active state
/// - calling Updates() + Input() of the active state
/// - transitioning to a new state
/// </summary>
public class StateMachine_Core<T> : Node
{
#region HEADER

    public IState ActiveState { get; set; }

    private T _rootNode;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public void Initialize_StateMachine_Core(NodePath pInitialState, T pRootNode)
    {
        _rootNode = pRootNode;

        // Set the initial state
        ActiveState = GetNode<IState>(pInitialState);
        ActiveState.Enter_State<T>(pRootNode);
    }

    public void Update(float delta)             => ActiveState.Update(delta);
    public void Physics_Update(float delta)     => ActiveState.Physics_Update(delta);
    public void Input_State(InputEvent @event)  => ActiveState.Input_State(@event);

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// Move from the previous state to a new state
    /// </summary>
    /// <param name="pTargetStatePath">The node path from the "StateMachine" node to the state (because we could have a hierarchie like "StateMachine" > "Move" > "Idle" </param>
    /// <param name="pParam">A dictionnary to pass parameters from a state to the next one </param>
    public void TransitionTo(string pTargetStatePath, Dictionary<string, object> pParam = null)
    {
        // Check if the path exists
        if (!HasNode(pTargetStatePath))
            return;

        // Get the new state, exit the previous one, set the new one as active and enter it
        IState new_state = GetNode<IState>(pTargetStatePath);
        ActiveState.Exit_State();
        ActiveState = new_state;
        ActiveState.Enter_State<T>(_rootNode, pParam);
    }

#endregion
}
