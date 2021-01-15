using Godot;
using System;

[Tool]
public class DebugPanel : Control
{
#region HEADER

    [Export] public NodePath ReferencePath;
    [Export] public string[] Properties;

    //[Signal] public delegate void MySignal(bool value1, int value2);

    //Enums
    //public enum Borders { Left, Right, Top, Bottom }
    public Node Reference { 
        get => GetNode(ReferencePath);
        set {
            _reference = value;
            if (_reference == null)
                _Setup();
            else
                _title.Text = GetClass();
        }
    }

    //Public
    //public int value1 = 0;

    //Private
    private VBoxContainer _container;
    private Label _title;
    private Node _reference;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the engine creates object in memory
    //public override void _Init()
    //{ }

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        _container = GetNode<VBoxContainer>("VBoxContainer");
        _title = GetNode<Label>("ReferenceName");
        Reference = GetNode(ReferencePath);
    }

    // To draw custom nodes (primitives ...). Called once, then draw commands are cached.
    // Use Update(); in _Process() to call _Draw() every frame
    //   All draw* shapes : https://docs.godotengine.org/en/stable/classes/class_canvasitem.html#class-canvasitem
    //public override void _Draw()
    //{}

    //public override void _Process(float delta)
    //{ }

    //public override void _PhysicsProcess(float delta)
    //{ }

    // Use to add warning in the Editor   (must add the [Tool] attribute on the class)
    //public override string _GetConfigurationWarning()
    //{ return "Add your warning message here"; }

    // Use to detect a key not defined in the Input Manager
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

    private void _Setup()
    {
        _Clear();
        _title.Text = Reference.Name;

    }

    private void _Clear()
    {
        foreach (Node property_label in _container.GetChildren())
            property_label.QueueFree();
    }

    private void _Track(string pProperty)
    {
        Label label = new Label();
        label.Autowrap = true;
        label.Name = pProperty.Capitalize();
        _container.AddChild(label);
    }

#endregion
}
