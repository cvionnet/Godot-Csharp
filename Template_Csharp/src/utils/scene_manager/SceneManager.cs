using Godot;
using System;

[Tool]
public class SceneManager : Node
{
#region HEADER

	private SceneTransition _sceneTransition;

    private Node _actualScene;
    private string _nextScene;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

	public override void _Ready()
	{
		_sceneTransition = GetNode<SceneTransition>("SceneTransition");
		_sceneTransition.Connect("SceneTransition_AnimationFinished", this, nameof(_onSceneTransition_Finished));

		_LoadFirstScene();
	}

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

	/// <summary>
	/// (From scenes) Change scene
	/// </summary>
	/// <returns></returns>
	private void _onCall_TransitionScene(string pnextScene)
	{
		_nextScene = pnextScene;
        _sceneTransition.Transition_Scene();
	}

	/// <summary>
	/// (From SceneTransition)
	/// </summary>
	/// <returns></returns>
	private void _onSceneTransition_Finished()
	{
        PackedScene scene = GD.Load<PackedScene>($"res://src/scenes/{_nextScene}.tscn");

        if (scene != null)
        {
            // If exists, unload the actual scene
            if (_actualScene != null)
                _actualScene.QueueFree();

            // Load the new scene + connect to its signal
            _actualScene = scene.Instance();
            AddChild(_actualScene);
            MoveChild(_actualScene, 0);    // To display the transition scene before this scene
            _actualScene.Connect("Generic_TransitionScene", this, nameof(_onCall_TransitionScene));
        }
        else
        {
            //GD.PrintErr($"SceneManager._onSceneTransition_Finished - The scene {_nextScene} does not exists");
            Nucleus_Utils.Error($"The scene {_nextScene} does not exists !", new TargetException(), GetType().Name, MethodBase.GetCurrentMethod().Name);
        }
	}

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

	private void _LoadFirstScene()
	{
        _nextScene = "Menu";
        _onSceneTransition_Finished();
	}

#endregion
}
