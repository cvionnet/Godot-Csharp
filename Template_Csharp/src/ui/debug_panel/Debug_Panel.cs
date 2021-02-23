using Godot;

// Doc : https://docs.godotengine.org/en/stable/classes/class_performance.html
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

    private Label _fps;
    private Label _process;
    private Label _physicsProcess;
    private Label _mem;
    private Label _objects;
    private Label _resources;
    private Label _nodes;
    private Label _drawcalls;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    // Called when the node and its children have entered the scene tree
    public override void _Ready()
    {
        _fps = GetNode<Label>("CanvasLayer/VBoxContainer/FPS");
        if (!FPS) _fps.Visible = false;
        _process = GetNode<Label>("CanvasLayer/VBoxContainer/Process");
        if (!Process) _process.Visible = false;
        _physicsProcess = GetNode<Label>("CanvasLayer/VBoxContainer/Physics_Process");
        if (!Physics_Process) _physicsProcess.Visible = false;
        _mem = GetNode<Label>("CanvasLayer/VBoxContainer/Mem");
        if (!Mem) _mem.Visible = false;
        _objects = GetNode<Label>("CanvasLayer/VBoxContainer/Objects");
        if (!Objects) _objects.Visible = false;
        _resources = GetNode<Label>("CanvasLayer/VBoxContainer/Resources");
        if (!Resources) _resources.Visible = false;
        _nodes = GetNode<Label>("CanvasLayer/VBoxContainer/Nodes");
        if (!Nodes) _nodes.Visible = false;
        _drawcalls = GetNode<Label>("CanvasLayer/VBoxContainer/Drawcalls");
        if (!Drawcalls) _drawcalls.Visible = false;
    }

    public override void _Process(float delta)
    {
        if (FPS) _fps.Text = Performance.GetMonitor(Performance.Monitor.TimeFps).ToString();
        if (Process) _process.Text = "Process:" + Mathf.RoundToInt(Performance.GetMonitor(Performance.Monitor.TimeProcess)).ToString();
        if (Physics_Process) _physicsProcess.Text = "Physics Process:" + Mathf.RoundToInt(Performance.GetMonitor(Performance.Monitor.TimePhysicsProcess)).ToString();
        if (Mem) _mem.Text = "Mem:" + Mathf.RoundToInt(Performance.GetMonitor(Performance.Monitor.MemoryStatic) / 1024).ToString();
        if (Objects) _objects.Text = "Objects:" + Performance.GetMonitor(Performance.Monitor.ObjectCount).ToString();
        if (Resources) _resources.Text = "Resources:" + Performance.GetMonitor(Performance.Monitor.ObjectResourceCount).ToString();
        if (Nodes) _nodes.Text = "Nodes:" + Performance.GetMonitor(Performance.Monitor.ObjectNodeCount).ToString();
        if (Drawcalls) _drawcalls.Text = "Drawcalls:" + Performance.GetMonitor(Performance.Monitor.Render2dDrawCallsInFrame).ToString();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}