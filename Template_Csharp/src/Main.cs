using Godot;
using Nucleus;
using System;

public class Main : Node
{
#region HEADER

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        Initialize_Main();
    }

    public override void _Notification(int what)
    {
        // When exit the game
        if (what == MainLoop.NotificationWmQuitRequest)
        {
            Nucleus_Utils.Finalize_Utils();

            GetTree().Quit(); // default behavior
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Main()
    {
        Nucleus_Utils.Initialize_Utils(GetViewport());
        Nucleus_Utils.State_Manager = GetNode<StateManager>("/root/StateManager");
    }

#endregion
}