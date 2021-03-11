using System.Collections.Generic;
using Godot;

// Thanks to Gonkee : https://youtu.be/8Us2cteHbbo
// Doc : https://docs.godotengine.org/en/stable/classes/class_performance.html


/// <summary>
/// Represent object's properties to display on the debug panel
/// </summary>
public class Properties
{
    public string Name { get; set; }
    public Godot.Object Object { get; set; }
    public string Reference { get; set; }       // name of the method or the variable we want to access
    public bool isMethod { get; set; }

    /// <summary>
    /// To create a new property value / result of a method to display on the Debug panel
    /// </summary>
    /// <param name="pName">The text to display and describe the property (informative)</param>
    /// <param name="pObject">A Godot object containing the property / method</param>
    /// <param name="pReference">The name of the property / method</param>
    /// <param name="pIsMethod">Set to True if this is a method</param>
    public Properties(string pName, Godot.Object pObject, string pReference, bool pIsMethod)
    {
        Name = pName;
        Object = pObject;
        Reference = pReference;
        isMethod = pIsMethod;
    }
}

//*-------------------------------------------------------------------------*//

/// <summary>
/// Debug Panel class
/// How to display properties :
///     In the _Ready() method :
///         _debugPanel = GetNode<Debug_Panel>("Debug_Panel");
///         _player = GetNode<Player>("Player");
///         👉 adding a node property  ( Position )
///             _debugPanel.Add_Property(new Properties("Player position", _player, "position", false));
///         👉 adding a script property  ( public float Speed { get; set; } )
///             _debugPanel.Add_Property(new Properties("Player speed", _player, "Speed", false));
///         👉 adding a script method result  ( public Vector2 Get_Velocity() { return _velocity; } )
///             _debugPanel.Add_Property(new Properties("Player velocity", _player, "Get_Velocity", true));
///         
/// </summary>
public class Debug_Panel : Node2D
{
#region HEADER

    [Export] private bool FPS = true;
    [Export] private bool Drawcalls = true;
    [Export] private bool Process = true;
    [Export] private bool Physics_Process = true;
    [Export] private bool Mem = true;
    [Export] private bool Objects = true;
    [Export] private bool Resources = true;
    [Export] private bool Nodes = true;

    private Label _properties;

    private Label _fps;
    private Label _process;
    private Label _physicsProcess;
    private Label _mem;
    private Label _objects;
    private Label _resources;
    private Label _nodes;
    private Label _drawcalls;

    private List<Properties> _propertiesList = new List<Properties>();

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _properties = GetNode<Label>("CanvasLayer/Properties/PropertyLabel");

        _fps = GetNode<Label>("CanvasLayer/VBoxContainer/FPS");
        _process = GetNode<Label>("CanvasLayer/VBoxContainer/Process");
        _physicsProcess = GetNode<Label>("CanvasLayer/VBoxContainer/Physics_Process");
        _mem = GetNode<Label>("CanvasLayer/VBoxContainer/Mem");
        _objects = GetNode<Label>("CanvasLayer/VBoxContainer/Objects");
        _resources = GetNode<Label>("CanvasLayer/VBoxContainer/Resources");
        _nodes = GetNode<Label>("CanvasLayer/VBoxContainer/Nodes");
        _drawcalls = GetNode<Label>("CanvasLayer/VBoxContainer/Drawcalls");

        // Hide options not selected in the Inspector's properties
        if (!FPS) _fps.Visible = false;
        if (!Process) _process.Visible = false;
        if (!Physics_Process) _physicsProcess.Visible = false;
        if (!Mem) _mem.Visible = false;
        if (!Objects) _objects.Visible = false;
        if (!Resources) _resources.Visible = false;
        if (!Nodes) _nodes.Visible = false;
        if (!Drawcalls) _drawcalls.Visible = false;
    }

    public override void _Process(float delta)
    {
        if (FPS) _fps.Text = Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();    // Engine.GetFramesPerSecond().ToString();
        if (Process) _process.Text = "Process:" + Mathf.RoundToInt(Performance.GetMonitor(Performance.Monitor.TimeProcess)).ToString();
        if (Physics_Process) _physicsProcess.Text = "Physics Process:" + Mathf.RoundToInt(Performance.GetMonitor(Performance.Monitor.TimePhysicsProcess)).ToString();
        if (Mem) _mem.Text = "Mem:" + Mathf.RoundToInt(Performance.GetMonitor(Performance.Monitor.MemoryStatic) / 1024).ToString();     // OS.GetStaticMemoryUsage()
        if (Objects) _objects.Text = "Objects:" + Performance.GetMonitor(Performance.Monitor.ObjectCount).ToString();
        if (Resources) _resources.Text = "Resources:" + Performance.GetMonitor(Performance.Monitor.ObjectResourceCount).ToString();
        if (Nodes) _nodes.Text = "Nodes:" + Performance.GetMonitor(Performance.Monitor.ObjectNodeCount).ToString();
        if (Drawcalls) _drawcalls.Text = "Drawcalls:" + Performance.GetMonitor(Performance.Monitor.Render2dDrawCallsInFrame).ToString();

        _Update_Properties();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// To add a new property to display on the Debug panel
    /// </summary>
    /// <param name="pProperty">A Properties object</param>
    public void Add_Property(Properties pProperty)
    {
        if (pProperty == null || pProperty.Name == null || pProperty.Object == null || pProperty.Reference == null || pProperty.Name?.Length == 0 || pProperty.Reference?.Length == 0)
            return;

        _propertiesList.Add(pProperty);
    }

    /// <summary>
    /// To update properties's values
    /// </summary>
    private void _Update_Properties()
    {
        _properties.Text = "";

        foreach (Properties property in _propertiesList)
        {
            object property_result;

            // Check if a QueueFree() has not been called on the object 
            if (WeakRef(property.Object).GetRef() != null)
            {
                // for a method, need to use "Call"
                if (property.isMethod)
                    property_result = property.Object.Call(property.Reference);
                else
                    property_result = property.Object.Get(property.Reference);

                _properties.Text += property.Name + " : " + property_result + "\n";
            }
        }
    }

#endregion
}