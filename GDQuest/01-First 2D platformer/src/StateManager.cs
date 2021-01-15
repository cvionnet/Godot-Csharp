using System;
using Godot;

public class StateManager : Node
{
#region HEADER
    [Signal] public delegate void ScoreUpdated();
    [Signal] public delegate void PlayerDied();

    public int Score {
        get => _score;
        set {
            _score = value;
            EmitSignal(nameof(ScoreUpdated));
        }
    }

    public int Death {
        get => _death;
        set {
            _death = value;
            EmitSignal(nameof(PlayerDied));
        }
    }

    private int _score = 0;
    private int _death = 0;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

public void ResetGame()
{
    _score = 0;
    _death = 0;
}

#endregion
}