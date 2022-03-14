using Godot;
using Nucleus;
using System;

/// <summary>
/// Responsible for :
/// - displaying score
/// </summary>
public class UI_Player : CanvasLayer
{
#region HEADER

    private Label _score;
    private Label _time;
    private Timer _timerTime;

    private int _timeLeft;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _score = GetNode<Label>("Control/Score");
        _time = GetNode<Label>("Control/Time");
        _timerTime = GetNode<Timer>("TimerTime");

        _timerTime.Connect("timeout", this, nameof(onTimerTime_Timeout));
        Nucleus_Utils.State_Manager.Connect("Player_UIPlayer_UpdateScore", this, nameof(onPlayer_UpdateScore));              // emited from Move_Player

        Initialize_UI_Player();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// When the Game send a signal to update the score
    /// </summary>
    /// <param name="pScore">The score to display</param>
    private void onPlayer_UpdateScore(int pScore) => _score.Text = $"Score : {pScore}";

    private void onTimerTime_Timeout()
    {
        if (_timeLeft > 0)
        {
            _timeLeft--;
            _time.Text = _timeLeft.ToString();
        }
        else
        {
            _timerTime.Stop();
            Nucleus_Utils.State_Manager.EmitSignal("UIPlayer_GameBrain_LevelTimeout");
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_UI_Player()
    {
        _timeLeft = Nucleus_Utils.State_Manager.LevelActive.RoundTime;
        _time.Text = _timeLeft.ToString();

        _timerTime.Start();
    }

#endregion
}
