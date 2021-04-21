using Godot;

/// <summary>
/// State interface to use in Hierarchical State Machines
/// </summary>
public interface IState
{
    /// <summary>
    /// To initialize the state
    /// </summary>
    /// <param name="pParam">To pass various parameters (eg : "speed", 100.0f) </param>
    void Enter_State(Godot.Collections.Dictionary<string, object> pParam);

    /// <summary>
    /// To cleanup the state before transition to another one
    /// </summary>
    void Exit_State();

    void Update(float delta);
    void Physics_Update(float delta);
    void Input_State(InputEvent @event);

    /// <summary>
    /// Implementation : public string GetStateName() { return this.Name; }
    /// </summary>
    string GetStateName();
}