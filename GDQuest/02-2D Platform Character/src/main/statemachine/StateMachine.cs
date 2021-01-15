using Godot;
using System;

/// <summary>
/// Generic State Machine. Initializes states and delegates engine callbacks (_PhysicsProcess and _UnhandledInput) to the active state
/// </summary>
public class StateMachine : Node
{
#region HEADER

    [Export] public NodePath InitialState;

    //[Signal] public delegate void MySignal(bool value1, int value2);

    // A reference to the active State scene
    public State ActiveState {
        get => _activeState;
        set {
            _activeState = value;
            _activeStateName = _activeState.Name;
        }
    }

    // A reference to the root node (=Owner) in the Scene where the State Machine node has been added (eg : Player)
    public KinematicBody2D RootNode { get; private set; }

    // An empty dictionary to pass (as default) as 2nd parameter to TransitionTo()
    public Godot.Collections.Dictionary<string, object> TransitionToParam_Void {get; private set; }

    private State _activeState;
    private string _activeStateName;    // to store the name of the Node (used in the DebugDock node)

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // A constructor replace the _init() method in GDScript ("Called when the engine creates object in memory")
    public StateMachine()
    {
        AddToGroup("state_machine");

        TransitionToParam_Void = new Godot.Collections.Dictionary<string, object>();
        TransitionToParam_Void.Add("", 0);
    }

    public override void _Ready()
    {
        ActiveState = GetNode<State>(InitialState);
        _activeStateName = _activeState.Name;

        // Wait for the scene to be ready (to be sure to access safely to nodes)
        _SceneReady();

        _Init_StateMachine();
        RootNode = (KinematicBody2D)Owner;
        _activeState.Enter_State(TransitionToParam_Void);
    }

    public override void _Process(float delta)
    {
        _activeState.Update(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        _activeState.Physics_Update(delta);
    }

    // Use to detect a key not defined in the Input Manager
    public override void _UnhandledInput(InputEvent @event)
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
        State new_state = GetNode<State>(pTargetStatePath);
        _activeState.Exit_State();
        ActiveState = new_state;
        _activeState.Enter_State(pParam);
    }

    /// <summary>
    /// Wait for the owner to be ready (owner = Node at the top of the scene)
    /// </summary>
    async private void _SceneReady()
    {
        await ToSignal(Owner, "ready");
    }

    /// <summary>
    /// Initialize the State Machine node reference in Utils (will be used by the States)
    /// </summary>
    private void _Init_StateMachine()
    {
        Node node = Utils.FindNode_BasedOnGroup(this, "state_machine");

        if(node != null)
        {
            Utils.StateMachine_Node = (StateMachine)node;
        }
        else
        {
            GD.Print("ERROR (in States.cs) - State Machine node is null");
            GetTree().Quit();      // Quit game
        }
    }

#endregion
}
