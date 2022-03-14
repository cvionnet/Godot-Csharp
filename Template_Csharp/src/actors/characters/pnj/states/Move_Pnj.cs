using System;
using System.Reflection;
using Godot;
using Godot.Collections;
using Nucleus;
using Nucleus.AI;

/// <summary>
/// Responsible for :
/// - playing the move animation
/// - moving the character using Steering
/// - transitioning to Idle
/// </summary>
public class Move_Pnj : Node, IState
{
#region HEADER

    private Pnj _rootNode;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        Initialize_Move();
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

            _rootNode.TimerScore.Connect("timeout", this, nameof(onTimerScore_Timeout));
        }
        if (_rootNode.CharacterProperties.DebugMode) _rootNode.DebugLabel.Text = _rootNode.StateMachine.ActiveState.GetStateName();

        _rootNode.CharacterProperties.IsMoving = true;
    }

    public void Exit_State() { }
    public void Update(float delta) { }

    public void Physics_Update(float delta) => Move_Character();

    public void Input_State(InputEvent @event) { }
    public string GetStateName() => Name;

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// When the character walk on a platform or on borders (two Area2D collides)
    /// </summary>
    private void onAreaShapeEntered(int area_id, Area area, int area_shape, int local_shape)
    {
        if(area != null && area.Name.StartsWith("@Platform"))
        {
            Check_UpdateScore();
        }
        else if(area != null && area.Name.StartsWith("Border"))
        {
            _rootNode.StateMachine.TransitionTo("Fall");
        }
    }

    /// <summary>
    /// Update the score
    /// </summary>
    private void onTimerScore_Timeout() => _rootNode.CharacterProperties.Update_Score(1);

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Move()
    { }

    /// <summary>
    /// Calculate velocity between the character and the target (cursor), then move the character
    /// </summary>
    private void Move_Character()
    {
        // Perform calcul only if the node have to move
        if (_rootNode.CharacterProperties.Steering.TargetGlobalPosition != _rootNode.GlobalPosition)
        {
            _rootNode.CharacterProperties.Velocity = _rootNode.CharacterProperties.Steering.Steering_Seek(_rootNode.CharacterProperties, _rootNode.GlobalPosition);

            // Move the character
            if (_rootNode.CharacterProperties.Velocity.Abs() >= Nucleus_Utils.VECTOR_1)
            {
                if (_rootNode.CharacterAnimation.CurrentAnimation != "run")
                    _rootNode.CharacterAnimation.Play("run");

                _rootNode.CharacterProperties.Velocity = _rootNode.MoveAndSlide(_rootNode.CharacterProperties.Velocity);

                // Flip sprite on left or right
                _rootNode.CharacterSprite.FlipH = _rootNode.CharacterProperties.IsOrientationHorizontalInverted;

                //_animatedSprite.Rotation = _velocity.Angle();   // point the character direction towards the destination
            }
            else
            {
                _rootNode.CharacterProperties.IsMoving = false;
                _rootNode.StateMachine.TransitionTo("Idle");
            }
        }
    }

    /// <summary>
    /// Start the timer to update the score if conditions are filled
    /// </summary>
    private void Check_UpdateScore()
    {
    }

#endregion
}