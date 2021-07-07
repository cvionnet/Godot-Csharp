using Godot;
using System;

public class Menu : Node
{
#region HEADER

    [Signal] private delegate void Menu_NewGame();

	private StateManager _stateManager;

    private Button _buttonStart;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
		_stateManager = GetNode<StateManager>("/root/StateManager");

        _buttonStart = GetNode<Button>("Button");
        _buttonStart.Connect("pressed",this , nameof(_onButtonStart_Pressed));

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
        // (to SceneManager) Start a new game
        EmitSignal(nameof(Menu_NewGame));
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Menu()
    {
		_stateManager.ActiveScene = StateManager.Scene_Level.MENU;
    }

#endregion
}