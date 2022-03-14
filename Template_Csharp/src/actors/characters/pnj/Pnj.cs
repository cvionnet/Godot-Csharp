using Godot;
using Nucleus;
using Nucleus.AI;

/// <summary>
/// Responsible for :
/// - displaying a random character
/// - initializing the character properties
/// - initializing the StateMachine
/// - setting new player's random direction (transition to Move)
/// - receiving actions from Items
/// </summary>
public class Pnj : KinematicBody2D
{
#region HEADER

    public CCharacter CharacterProperties { get; private set; }

    public StateMachine_Pnj StateMachine { get; private set; }
    public Label DebugLabel { get; private set; }
    public Label DebugLabel2 { get; private set; }
    public Timer TimerScore { get; private set; }
    public AnimationPlayer CharacterAnimation { get; private set; }
    public Sprite CharacterSprite { get; private set; }

    private Timer _timerItemActionDuration;
    private Timer _timerChangeDestination;

    private readonly float _minTimerNewDestination = 5.0f;
    private readonly float _maxTimerNewDestination = 10.0f;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        StateMachine = GetNode<StateMachine_Pnj>("StateMachine");
        DebugLabel = GetNode<Label>("DebugLabel");
        DebugLabel2 = GetNode<Label>("DebugLabel2");
        CharacterAnimation = GetNode<AnimationPlayer>("CharacterAnimation");
        CharacterSprite = GetNode<Sprite>("CharacterSprite");

        TimerScore = GetNode<Timer>("CharacterTimers/TimerScore");
        _timerItemActionDuration = GetNode<Timer>("CharacterTimers/TimerItemActionDuration");
        _timerChangeDestination = GetNode<Timer>("CharacterTimers/TimerNewDestination");

        _timerItemActionDuration.Connect("timeout", this, nameof(onItemActionDurationTimer_Timeout));
        _timerChangeDestination.Connect("timeout", this, nameof(onChangeDestinationTimer_Timeout));

        Initialize_Pnj();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// When action time of an item finish
    /// </summary>
    private void onItemActionDurationTimer_Timeout() => CharacterProperties.ActionEnd_Item();

    /// <summary>
    /// Generate a new destination for the character
    /// </summary>
    private void onChangeDestinationTimer_Timeout()
    {
        Set_NewDestination();
        Initialize_TimerNewDestination();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Pnj()
    {
        Name = "PNJ";       // prefix name of nodes

        //Initialize_CharacterSprite();
        Initialize_Properties();
        Initialize_TimerNewDestination();

        // Initialize the StateMachine
        StateMachine.Init_StateMachine(this);
    }

    /// <summary>
    /// Generate a random character by adding a CustomCharacter node
    /// </summary>
    private void Initialize_CharacterSprite()
    {
        //PackedScene sceneCharacter = GD.Load<PackedScene>("res://src/actors/characters/customCharacter/CustomCharacter.tscn");
        //_spriteCharacter = (CustomCharacter)sceneCharacter.Instance();
        //AddChild(_spriteCharacter);
        //MoveChild(_spriteCharacter, 0);

        //CharacterAnimation = _spriteCharacter.GetNode<AnimationPlayer>("AnimationPlayer");
    }

    /// <summary>
    /// Initialize character properties
    /// </summary>
    private void Initialize_Properties()
    {
        CharacterProperties = new CCharacter(IsPlateformer:false, Name);

        CharacterProperties.IsControlledByPlayer = false;

        CharacterProperties.Steering.LeaderToFollow = this;     // Steering AI - Set the player node as leader
        CharacterProperties.MaxSpeed = new Vector2(Nucleus_Maths.Rnd.RandfRange(300.0f, 500.0f), Nucleus_Maths.Rnd.RandfRange(300.0f, 500.0f));
        CharacterProperties.Steering.TargetGlobalPosition = GlobalPosition;
//        CharacterProperties.Steering.Speed = CharacterProperties.MaxSpeed.x;
    }

    /// <summary>
    /// Send the character to a new destination
    /// </summary>
    private void Set_NewDestination()
    {
        // TODO: use "pPlatformProperties.PlatformNextGroupToOpen" to not send the player on the next open platform

        // Move the player to another place
        CharacterProperties.Steering.TargetGlobalPosition = new Vector2(Nucleus_Maths.Rnd.RandfRange(10.0f, Nucleus_Utils.ScreenWidth-10.0f), Nucleus_Maths.Rnd.RandfRange(10.0f, Nucleus_Utils.ScreenHeight-10.0f));
        if (CharacterProperties.DebugMode) DebugLabel2.Text = Mathf.Floor(CharacterProperties.Steering.TargetGlobalPosition.x) + "/" + Mathf.Floor(CharacterProperties.Steering.TargetGlobalPosition.y);

        StateMachine.TransitionTo("Move");
    }

    /// <summary>
    /// Random timing before the character change destination
    /// </summary>
    private void Initialize_TimerNewDestination()
    {
        _timerChangeDestination.WaitTime = Nucleus_Maths.Rnd.RandfRange(_minTimerNewDestination, _maxTimerNewDestination);
        _timerChangeDestination.Start();
    }

#region ACTIONS

    /// <summary>
    /// When an item has been touched (called by ItemGeneric)
    /// </summary>
    /// <param name="pItemProperties">All properties of the item touched</param>
    /// <param name="pItemTouchedBy">The name of the node that hits the item</param>
    public void Item_Action(CItem pItemProperties, string pItemTouchedBy)
    {
        // Call the generic method
        CharacterProperties.ActionFrom_Item(pItemProperties, pItemTouchedBy, _timerItemActionDuration);
    }

#endregion ACTIONS

#endregion
}