using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Shakes the screen when is_shaking is set to true
///     To make it react to events happening in the game world, use the Events signal routing singleton
/// </summary>

public class ShackingCamera : Camera2D
{
#region HEADER

	//[Export] public float DAMP_EASING = 1.0f;
	[Export] public float Amplitude = 4.0f;
	[Export] public int Mouse_Smoothing_Speed_Default = 3;
	[Export] public int Gamepad_Smoothing_Speed_Default = 1;

	[Export]
	public float Duration {
		get => _duration;
		set {
			_duration = value;
			// Set the timer value
			if (_timer.TimeLeft <= 0.0f)
				_timer.WaitTime = _duration;
		}
	}

	[Export]
	public bool isShaking {
		get => _isShaking;
		set {
			_isShaking = value;
			if (_isShaking)
				_ChangeState(CamStates.Shacking);
			else
				_ChangeState(CamStates.Idle);
		}
	}

	public enum CamStates { Idle, Shacking }

	private Timer _timer;

	private float _duration;
	private bool _isShaking;
	private CamStates _camState = CamStates.Idle;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");

		Duration = 0.3f;
		isShaking = false;

		_timer.Connect("timeout", this, nameof(_on_ShakeTimer_timeout));
		_ResetSmoothingSpeed();
		SetProcess(false);
	}

	public override void _Process(float delta)
	{
		//var damping = ease(timer.time_left / timer.wait_time, DAMP_EASING)
		Offset = new Vector2(
			(float)GD.RandRange(Amplitude, -Amplitude),
			(float)GD.RandRange(Amplitude, -Amplitude)
		);
	}

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

	private void _on_ShakeTimer_timeout()
	{
		isShaking = false;
	}

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

	private void _ResetSmoothingSpeed()
	{
		SmoothingSpeed = Mouse_Smoothing_Speed_Default;
	}

	private void _ChangeState(CamStates pNewState)
	{
		switch (pNewState)
		{
			case CamStates.Idle:
				Offset = Utils.VECTOR_1;
				SetProcess(false);      // Block the call to the _Process method
				break;

			case CamStates.Shacking:
				SetProcess(true);
				_timer.Start();
				break;

			default:
				break;
		}
		_camState = pNewState;
	}

#endregion
}
