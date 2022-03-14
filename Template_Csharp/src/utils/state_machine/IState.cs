using Godot;
using Godot.Collections;

/// <summary>
/// State interface to use in Hierarchical State Machines
/// </summary>
public interface IState
{
    /// <summary>
    /// To initialize the state
    /// </summary>
    /// <param name="pRootNode">The root node having a StateMachine as node</param>
    /// <param name="pParam">To pass various parameters between states (eg : "speed", 100.0f)</param>
    void Enter_State<T>(T pRootNode, Dictionary<string, object> pParam = null);

    /// <summary>
    /// To cleanup the state before transition to another one
    /// </summary>
    void Exit_State();

    /// <summary>
    /// To override Godot's methods
    /// </summary>
    void Update(float delta);
    void Physics_Update(float delta);
    void Input_State(InputEvent @event);

    /// <summary>
    /// Implementation : public string GetStateName() { return this.Name; }
    /// </summary>
    string GetStateName();
}