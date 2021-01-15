using Godot;
using System;

[Tool]
public class ChangeSceneButton : Button
{
#region HEADER
//    [Export] public PackedScene NextScene;
    [Export(PropertyHint.File,"")] public string NextScene;

    private StateManager _stateManager;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        // To use the Autoload file named StateManager
        _stateManager = GetNode<StateManager>("/root/StateManager");
    }

    // Use to add warning in the Editor   (must add the [Tool] attribute on the class)
    public override string _GetConfigurationWarning()
    {
        return (NextScene == null) ? "Next Scene must not be empty !" : "";
    }

    // Use to detect a key not defined in the Input Manager
    // Note : it's cleaner to define key in the Input Manager and use  Input.IsActionPressed("myaction")   in  _Process
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Enter)
            {
                _on_PlayButton_button_up();
            }
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    public void _on_PlayButton_button_up()
    {
        //Unpause game
        if (GetTree().Paused)
            GetTree().Paused = false;

        _stateManager.ResetGame();
        GetTree().ChangeScene(NextScene);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}
