using Godot;
using System;

[Tool]
public class Game : Node
{
#region HEADER

	[Export] public PackedScene MinionScene;
	[Export] public int MinionMaxNumber = 20;

	[Signal] public delegate void Player_Move(string animation, Vector2 destination);

	private Player _player;
	private Minion _minionInstance;
	private int _minionCount;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		Utils.Rnd.Randomize();

		_player = GetNode<Player>("Player");
	}

/*		
	public override void _Process(float delta)
	{
		if (Input.IsActionJustReleased("click"))
		{
			_CreateMinion(MinionMaxNumber);
			EmitSignal(nameof(Player_Move), "walk", GetViewport().GetMousePosition());
		}
	}
*/

	//public override void _PhysicsProcess(float delta)
	//{}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("click"))
		{
			_CreateMinion(MinionMaxNumber);
			EmitSignal(nameof(Player_Move), "walk", GetViewport().GetMousePosition());
		}
	}

	// Use to add warning in the Editor   (must add the [Tool] attribute on the class)
	public override string _GetConfigurationWarning()
	{
		return (MinionScene == null) ? "'Minion Scene' property must not be empty !" : "";
	}

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

	/// <summary>
	/// Create new instance of the Minion
	/// </summary>
	/// <param name="pCount">How much Minion to create</param>
	private void _CreateMinion(int pCount)
	{
		for (int i = 0; i < pCount; i++)
		{
			if (_minionCount >= MinionMaxNumber)
				break;

			_minionInstance = (Minion)MinionScene.Instance();
			GetNode("Followers").AddChild(_minionInstance);
			_minionCount++;
		}
	}

#endregion
}
