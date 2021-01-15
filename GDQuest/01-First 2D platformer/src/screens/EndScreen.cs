using Godot;
using System;

public class EndScreen : Control
{
#region HEADER

    private StateManager _stateManager;
    private Label _resultLabel;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _resultLabel = GetNode<Label>("Label");
        _stateManager = GetNode<StateManager>("/root/StateManager");

        _resultLabel.Text = $"Your final score is {_stateManager.Score}\nYou died {_stateManager.Death} times";
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}
