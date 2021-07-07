using Godot;
using Nucleus;

public class Game : Node
{
#region HEADER

	[Signal] private delegate void Game_EndGame();

	private StateManager _stateManager;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
		_stateManager = GetNode<StateManager>("/root/StateManager");

        Initialize_Game();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Game()
    {
        Nucleus_Utils.Initialize_Utils(GetViewport());
        Nucleus_Utils.State_Manager = GetNode<StateManager>("/root/StateManager");

		_stateManager.ActiveScene = StateManager.Scene_Level.GAME;
    }

	/// <summary>
	/// Check victory or gameover conditions
	/// </summary>
	private void Check_EndGame()
	{
        // (to SceneManager)
        EmitSignal(nameof(Game_EndGame));
	}

#endregion
}