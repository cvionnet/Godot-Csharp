using Godot;
using System;

public class UI : Control
{
#region HEADER

    public bool isPaused {
        get => _isPaused;
        set {
            _isPaused = value;
            _sceneTree.Paused = value;      // Pause, unpause the game
            _pauseOverlay.Visible = value;
        }
    }

    private StateManager _stateManager;
    private bool _isPaused = false;
    private SceneTree _sceneTree;
    private ColorRect _pauseOverlay;
    private Label _scoreLabel;
    private Label _titleLabel;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        // Optimisation to get the scenetree only at startup
        _sceneTree = GetTree();
        _pauseOverlay = GetNode<ColorRect>("PauseOverlay");
        _scoreLabel = GetNode<Label>("Score");
        _titleLabel = GetNode<Label>("PauseOverlay/Title");

        // Connect to signals of the StateManager
        _stateManager = GetNode<StateManager>("/root/StateManager");
        _stateManager.Connect("ScoreUpdated", this, nameof(_Update_UI));
        _stateManager.Connect("PlayerDied", this, nameof(_Update_PlayerDeath));

        // Hide the pause overlay
        _pauseOverlay.Visible = false;

        _Update_UI();
    }

    // Use to detect a key not defined in the Input Manager
    // Note : it's cleaner to define key in the Input Manager and use  Input.IsActionPressed("myaction")   in  _Process
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.IsActionPressed("pause"))
            {
                isPaused = !isPaused;
                _sceneTree.SetInputAsHandled();
            }
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

private void _Update_UI()
{
    _scoreLabel.Text = $"Score {_stateManager.Score}";
}

private void _Update_PlayerDeath()
{
    isPaused = true;
    _titleLabel.Text = "You die   :(";
}

#endregion
}
