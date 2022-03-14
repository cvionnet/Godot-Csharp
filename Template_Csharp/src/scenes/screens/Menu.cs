using Godot;
using Nucleus;

/// <summary>
/// Responsible for :
/// - selecting and launching the level to play
/// </summary>
public class Menu : Node
{
#region HEADER

    private Button _buttonStart;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _buttonStart = GetNode<Button>("Button");
        _buttonStart.Connect("pressed", this, nameof(_onButtonStart_Pressed));

        Initialize_Menu();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// Button "Start"
    /// </summary>
    private void _onButtonStart_Pressed()
    {
        // Go to 1st level
        Nucleus_Utils.State_Manager.LevelActive = Nucleus_Utils.State_Manager.LevelList[0];
        Nucleus_Utils.State_Manager.EmitSignal("Generic_TransitionScene", $"levels/Level{Nucleus_Utils.State_Manager.LevelActive.LevelId}");    // (to SceneManager)
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Menu()
    { }

#endregion
}