using Godot;
using Nucleus;
using System;
using System.Reflection;

/// <summary>
/// Responsible for :
/// - loading the 1st screen
/// - transitioning to other screens
/// </summary>
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

		LoadFirstScene();
	}

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

	/// <summary>
	/// When a signal to change scene hab been emited from a scene
	/// </summary>
	private void _onCall_TransitionScene(string pnextScene)
	{
		_nextScene = pnextScene;

        // Call the animation
        _sceneTransition.Transition_Scene();
	}

	/// <summary>
	/// (From SceneTransition) When the transition animation is finished
	/// </summary>
	private void _onSceneTransition_Finished()
	{
        //PackedScene scene = GD.Load<PackedScene>($"res://src/scenes/{_nextScene}.tscn");
        PackedScene scene = ResourceLoader.Load($"res://src/scenes/{_nextScene}.tscn") as PackedScene;

        if (scene != null)
        {
            // If exists, unload the actual scene
            if (_actualScene != null)
                _actualScene.QueueFree();

            // Load the new scene + connect to its signal
            _actualScene = scene.Instance();
            AddChild(_actualScene);
            MoveChild(_actualScene, 0);    // To display the transition scene before this scene
        }
        else
        {
            //GD.PrintErr($"SceneManager._onSceneTransition_Finished - The scene {_nextScene} does not exists");
            Nucleus_Utils.Error($"The scene {_nextScene} does not exists !", new TargetException(), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        }
	}

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

	private void LoadFirstScene()
	{
        _nextScene = "screens/Menu";
        _onSceneTransition_Finished();      // to add a .Connect on the 1st scene
	}

    /// <summary>
    /// (called by GameBrain) Connect to signal after GameBrain has been initialized (because it contains the Nucleus_Utils.State_Manager initialization)
    /// </summary>
    public void Initialize_SceneManager()
    {
        Nucleus_Utils.State_Manager.Connect("Generic_TransitionScene", this, nameof(_onCall_TransitionScene));
		Nucleus_Utils.State_Manager.Connect("SceneTransition_AnimationFinished", this, nameof(_onSceneTransition_Finished));
    }

#endregion
}
