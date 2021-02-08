using Godot;
using System;

public class StateMachine_Hook : StateMachine_Core
{
#region HEADER

    [Export] public NodePath InitialState;

    // A reference to the root node (=Owner) in the Scene where the State Machine node has been added (eg : Player)
    public Position2D RootNode { get; private set; }

    private string _groupName = "state_machine_hook";

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public StateMachine_Hook()
    {
        AddToGroup(_groupName);
    }

    public override void _Ready()
    {
        _SceneReady();
        _Init_StateMachine(_groupName);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// Wait for the owner to be ready (owner = Node at the top of the scene), to be sure to access safely to nodes
    /// </summary>
    async private void _SceneReady()
    {
        await ToSignal(Owner, "ready");
    }

    /// <summary>
    /// Initialize the State Machine node reference in Utils (will be used by the States)
    /// </summary>
    private void _Init_StateMachine(string pGroupName)
    {
        Node node = Utils.FindNode_BasedOnGroup(this, pGroupName);

        if(node != null)
        {
            Utils.StateMachine_Hook = (StateMachine_Hook)node;
            RootNode = (Position2D)Owner;
        }
        else
        {
            GD.Print("ERROR (in States.cs) - State Machine node is null (Group : " + _groupName + ")");
            GetTree().Quit();      // Quit game
        }
    }

#endregion
}
