using Godot;
using System;

public class Fire : State
{
#region HEADER

    //[Export] private int Value = 0;

    //[Signal] private delegate void MySignal(bool value1, int value2);

    //Enums
    //public enum Borders { Left, Right, Top, Bottom }

    //Public
    //public int value1 = 0;

    //Private
    //private int _value2 = 0;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // A constructor replace the _init() method in GDScript ("Called when the engine creates object in memory")
    //public Fire()
    //{}

    // Called when the node enters the scene tree for the first time
    //public override void _Ready()
    //{}

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
    //{ return "Add your warning message here"; }

    // Use to detect a key not defined in the Input Manager  (called only when a touch is pressed or released - not suitable for long press like run button)
    // Note : it's cleaner to define key in the Input Manager and use  Input.IsActionPressed("myaction")   in  _Process
    /*public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            // Close game if press Escape
            if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape)
            {
                GetTree().Quit();
                //_sceneTree.SetInputAsHandled();   // If uncommented, all eventKey conditions below will not be tested (usefull for a Pause)
            }
        }
    }*/

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}