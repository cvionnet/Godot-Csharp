using Godot;
using System;

public class RetryButton : Button
{
#region HEADER
    private StateManager _stateManager;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        // To use the Autoload file named StateManager
        _stateManager = GetNode<StateManager>("/root/StateManager");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    public void _on_RetryButton_button_up()
    {
        _stateManager.Score = 0;

        // Unpause the game
        GetTree().Paused = false;

        //Reload the current scene
        GetTree().ReloadCurrentScene();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}
