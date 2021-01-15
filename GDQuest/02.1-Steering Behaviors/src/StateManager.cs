using System;
using Godot;

/// <summary>
/// HOW TO :
///     In Godot, add this file as Autoload :  Project > Project Settings > AutoLoad    (Node name = StateManager)
/// IN THE CALLING SCENE :
///     StateManager _stateManager = GetNode<StateManager>("/root/StateManager");
///     _stateManager.Connect("ScoreUpdated", this, nameof(ActionMethod));
/// </summary>
public class StateManager : Node
{
#region HEADER

    //[Signal] public delegate void ScoreUpdated();

    /*
    public int Score {
        get => _score;
        set {
            _score = value;
            EmitSignal(nameof(ScoreUpdated));
        }
    }

    private int _score = 0;
    */
#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

/*
public void ResetGame()
{
    _score = 0;
}
*/

#endregion
}