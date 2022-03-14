using Godot;
using Nucleus;

/// <summary>
/// Responsible for :
/// - loading the SceneManager
/// - initializing levels
/// - transitioning between screens (Menu, GameOver ...)
/// - checking for GameOver
/// </summary>
public class GameBrain : Node
{
#region HEADER

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        Initialize_GameBrain();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("debug_restart"))
            GetTree().ReloadCurrentScene();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    private void onLevel_Timeout()
    {
        Display_EndGame();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_GameBrain()
    {
  		Nucleus_Utils.Initialize_Utils(GetViewport());
		Nucleus_Utils.State_Manager = GetNode<StateManager>("/root/StateManager");

        // Connect SceneManager after Nucleus_Utils.State_Manager initialization
        GetParent().GetNode<SceneManager>("SceneManager").Initialize_SceneManager();
        Nucleus_Utils.State_Manager.Connect("UIPlayer_GameBrain_LevelTimeout", this, nameof(onLevel_Timeout));   // emited from Player

        Initialize_LevelsList();
    }

    /// <summary>
    /// To initialize all levels properties
    /// </summary>
    private void Initialize_LevelsList()
    {
        Nucleus_Utils.State_Manager.LevelList.Add(new CLevel() { 
            LevelId = 1, 
            RoundTime = 60, 
            PnjNumberToDisplay = 5
        });
    }

	/// <summary>
	/// Gameover screen
	/// </summary>
	private void Display_EndGame()
	{
        Nucleus_Utils.State_Manager.EmitSignal("Generic_TransitionScene", "screens/Gameover");    // (to SceneManager)
	}

#endregion
}