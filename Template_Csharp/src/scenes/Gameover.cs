using Godot;
using System;

public class Gameover : Node
{
#region HEADER

    [Signal] private delegate void GameOver_PlayNewGame();

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

        Initialize_Gameover();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// Button "Play again"
    /// </summary>
    private void _onButtonStart_Pressed()
    {
        // (to SceneManager) Restart a new game
        EmitSignal(nameof(GameOver_PlayNewGame));
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Gameover()
    {
		_stateManager.ActiveScene = StateManager.Scene_Level.GAMEOVER;
    }

#endregion
}