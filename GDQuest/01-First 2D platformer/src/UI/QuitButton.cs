using Godot;
using System;

public class QuitButton : Button
{
#region HEADER

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    public void _on_QuitButton_button_up()
    {
        GetTree().Quit();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}
