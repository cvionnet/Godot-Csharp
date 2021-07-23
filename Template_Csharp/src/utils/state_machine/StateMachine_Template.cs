using Godot;
using Nucleus;
using System;

public class StateMachine_Template : StateMachine_Core
{
#region HEADER

    [Export] public NodePath InitialState;

    // A reference to the root node (=Owner) in the Scene where the State Machine node has been added (eg : Player)
    public KinematicBody2D RootNode { get; private set; }

    private string _groupName = "state_machine_template";   // or use class name :   this.GetType()

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public StateMachine_Template()
    {
        AddToGroup(_groupName);
    }

    public override void _Ready()
    {
        _SceneReady();
        _Init_StateMachine(_groupName);
    }

    public override void _Process(float delta)
    {
        base.Update(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        base.Physics_Update(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base.Input_State(@event);
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
        Node node = Nucleus_Utils.FindNode_BasedOnGroup(this, pGroupName);

        if(node != null)
        {
            Nucleus_Utils.StateMachine_Template = (StateMachine_Template)node;
            RootNode = (KinematicBody2D)Owner;

            base.Initialize(InitialState);
        }
        else
        {
            //GD.Print("ERROR (in States.cs) - State Machine node is null (Class : " + this.GetType() + ")");
            Nucleus_Utils.Error($"State Machine node is null (Groupname : {pGroupName}", new NullReferenceException(), GetType().Name, MethodBase.GetCurrentMethod().Name);

            GetTree().Quit();      // Quit game
        }
    }

#endregion
}
