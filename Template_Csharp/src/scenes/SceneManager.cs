using Godot;
using System;

[Tool]
public class SceneManager : Node
{
#region HEADER

	private StateManager _stateManager;

	[Export] private PackedScene Menu_Scene;
	[Export] private PackedScene Game_Scene;
	[Export] private PackedScene Gameover_Scene;

	private Menu _menuScene;
	private Game _gameScene;
	private Gameover _gameoverScene;

	private SceneTransition _sceneTransition;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

	public override void _Ready()
	{
		_stateManager = GetNode<StateManager>("/root/StateManager");

		_sceneTransition = GetNode<SceneTransition>("SceneTransition");
		_sceneTransition.Connect("SceneTransition_AnimationFinished", this, nameof(_onSceneTransition_Finished));

		_LoadFirstScene();
	}

	// Use to add warning in the Editor   (must add the [Tool] attribute on the class)
	public override string _GetConfigurationWarning()
	{
		string error = "";

		if (_menuScene == null || _gameScene == null || _gameoverScene == null)
			error = "All packed scenes properties must not be empty !";

		return error;
	}

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

	/// <summary>
	/// (From Menu, Game, GameOver) Change scene
	/// </summary>
	/// <returns></returns>
	private void _onCall_TransitionScene()
	{
		_sceneTransition.Transition_Scene();
	}

	/// <summary>
	/// (From SceneTransition)
	/// </summary>
	/// <returns></returns>
	private void _onSceneTransition_Finished()
	{
		switch (_stateManager.ActiveScene)
		{
			case StateManager.Scene_Level.MENU:
				_menuScene.QueueFree();

				_gameScene = (Game)Game_Scene.Instance();
				AddChild(_gameScene);
				MoveChild(_gameScene, 0);    // To display the transition scene before this scene
				_gameScene.Connect("Game_EndGame", this, nameof(_onCall_TransitionScene));		// (from Game)

				break;

			case StateManager.Scene_Level.GAME:
				_gameScene.QueueFree();

				_gameoverScene = (Gameover)Gameover_Scene.Instance();
				AddChild(_gameoverScene);
				MoveChild(_gameoverScene, 0);    // To display the transition scene before this scene
				_gameoverScene.Connect("GameOver_PlayNewGame", this, nameof(_onCall_TransitionScene));		// (from GameOver)

				break;

			case StateManager.Scene_Level.GAMEOVER:
				_gameoverScene.QueueFree();

				_menuScene = (Menu)Menu_Scene.Instance();
				AddChild(_menuScene);
				MoveChild(_menuScene, 0);    // To display the transition scene before this scene
				_menuScene.Connect("Menu_NewGame", this, nameof(_onCall_TransitionScene));		// (from Menu)
				break;
		}
	}

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

	private void _LoadFirstScene()
	{
		_menuScene = (Menu)Menu_Scene.Instance();
		AddChild(_menuScene);
		MoveChild(_menuScene, 0);    // To display the transition scene before this scene
		_menuScene.Connect("Menu_NewGame", this, nameof(_onCall_TransitionScene));		// (from Menu)
	}

#endregion
}
