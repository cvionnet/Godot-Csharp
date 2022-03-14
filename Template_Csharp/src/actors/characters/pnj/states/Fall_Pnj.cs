using Godot;
using Godot.Collections;
using Nucleus;
using System;
using System.Reflection;

/// <summary>
/// Responsible for :
/// - deleting the character
/// </summary>
public class Fall_Pnj : Node, IState
{
#region HEADER

    private Pnj _rootNode;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        Initialize_Fall();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region INTERFACE IMPLEMENTATION

    public void Enter_State<T>(T pRootNode, Dictionary<string, object> pParam = null)
    {
        if (pRootNode == null || pRootNode.GetType() != typeof(Pnj))
        {
            Nucleus_Utils.Error($"State Machine root node is null or type not expected ({pRootNode.GetType()})", new NullReferenceException(), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            return;
        }
        if (_rootNode == null) {
            _rootNode = pRootNode as Pnj;
        }
        if (_rootNode.CharacterProperties.DebugMode) _rootNode.DebugLabel.Text = _rootNode.StateMachine.ActiveState.GetStateName();

        Make_CharacterFall();
    }

    public void Exit_State() { }
    public void Update(float delta) { }
    public void Physics_Update(float delta) { }
    public void Input_State(InputEvent @event) { }
    public string GetStateName() => Name;

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// Make the character respawn
    /// </summary>
    private void onTimerFall_Timeout()
    {
        _rootNode.Position = new Vector2(100, 100);
        _rootNode.StateMachine.TransitionTo("Spawn");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Fall()
    { }

    private void Make_CharacterFall()
    {
        _rootNode.CharacterProperties.Update_Score(-5);
        _rootNode.CharacterProperties.Reset_Movement();
        _rootNode.Visible = false;
        _rootNode.Position = Nucleus_Utils.VECTOR_0;

//        _rootNode.CallDeferred("queue_free");
    }

    #endregion
}
