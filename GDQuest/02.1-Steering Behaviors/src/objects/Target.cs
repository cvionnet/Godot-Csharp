using Godot;
using System;

public class Target : Area2D
{
#region HEADER

    //[Export] public int alue = 0;

    //[Signal] public delegate void MySignal(bool value1, int value2);

    //Enums
    //public enum Borders { Left, Right, Top, Bottom }

    //Public
    //public int value1 = 0;

    //Private
    private AnimationPlayer _animationPlayer;
    private Area2D _target;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // A constructor replace the _init() method in GDScript ("Called when the engine creates object in memory")
    //public Target()
    //{}

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        Connect("body_entered", this, "_on_Body_Entered");
        Visible = false;
    }

    // To draw custom nodes (primitives ...). Called once, then draw commands are cached.
    // Use Update(); in _Process() to call _Draw() every frame
    //   All draw* shapes : https://docs.godotengine.org/en/stable/classes/class_canvasitem.html#class-canvasitem
    //public override void _Draw()
    //{}

    //public override void _Process(float delta)
    //{}

    public override void _PhysicsProcess(float delta)
    {
        // Make the target to follow the mouse cursor when click and drag
        if (Input.IsActionPressed("click"))
            GlobalPosition = GetGlobalMousePosition();
    }

    // Use to add warning in the Editor   (must add the [Tool] attribute on the class)
    //public override string _GetConfigurationWarning()
    //{ return "Add your warning message here"; }

    // Use to detect a key not defined in the Input Manager  (called only when a touch is pressed or released - not suitable for long press like run button)
    // Note : it's cleaner to define key in the Input Manager and use  Input.IsActionPressed("myaction")   in  _Process
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("click"))
            _animationPlayer.Play("fadein");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    public void _on_Body_Entered(PhysicsBody2D body)
    {
        _animationPlayer.Play("fadeout");
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}