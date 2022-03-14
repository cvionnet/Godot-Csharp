using Godot;
using Nucleus;
using System;
using System.Reflection;

public class StateMachine_Player : StateMachine_Core<Player>
{
    #region HEADER

    [Export] public NodePath InitialStateNode { get; set; }

    private bool _isReady = false;

    #endregion

    //*-------------------------------------------------------------------------*//

    #region GODOT METHODS

    public override void _Ready()
    {
        _SceneReady();
    }

    public override void _Process(float delta)
    {
        if(_isReady) base.Update(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        if(_isReady) base.Physics_Update(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(_isReady) base.Input_State(@event);
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
    async private void _SceneReady() => await ToSignal(Owner, "ready");

    /// <summary>
    /// Initialize the State Machine node reference in Utils (will be used by the States)
    /// </summary>
    /// <param name="pRootNode"></param>
    public void Init_StateMachine(Player pRootNode)
    {
        if(InitialStateNode != null)
        {
            base.Initialize_StateMachine_Core(InitialStateNode, pRootNode);
            _isReady = true;
        }
        else
        {
            Nucleus_Utils.Error($"State Machine node is null (owner : {pRootNode.Name}", new NullReferenceException(), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        }
    }

#endregion
}
