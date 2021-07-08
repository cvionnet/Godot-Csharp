using Godot;
using Nucleus;
using System;

public class Game : Node
{
#region HEADER

    [Signal] private delegate void Generic_TransitionScene(string nextScene);

	//private StateManager _stateManager;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // A constructor replace the _init() method in GDScript ("Called when the engine creates object in memory")
    //public Game()
    //{}

    // Called when the node enters the scene tree and before children
    //public override void _EnterTree()
    //{}

    public override void _Ready()
    {
		//_stateManager = GetNode<StateManager>("/root/StateManager");

        Initialize_Game();
    }

    // To draw custom nodes (primitives ...). Called once, then draw commands are cached.
    // Use Update(); in _Process() to call _Draw() every frame
    //   All draw* shapes : https://docs.godotengine.org/en/stable/classes/class_canvasitem.html#class-canvasitem
    //public override void _Draw()
    //{}

    //public override void _Process(float delta)
    //{}

    //public override void _PhysicsProcess(float delta)
    //{}

    // Use to add warning in the Editor   (must add the [Tool] attribute on the class)
    //public override string _GetConfigurationWarning()
    //{ return (MyObject == null) ? "The object XXXX must not be empty !" : ""; }    

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("debug_restart"))
            GetTree().ReloadCurrentScene();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Game()
    {}

	/// <summary>
	/// Check victory or gameover conditions
	/// </summary>
	private void Check_EndGame()
	{
        EmitSignal(nameof(Generic_TransitionScene), "Gameover");    // (to SceneManager)
	}

#endregion
}