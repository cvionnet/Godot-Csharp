using Godot;
using System;

public class HUD : Control
{
#region HEADER

    [Signal] public delegate void Player_Follow();

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        GetNode("VBoxContainer/Follow").Connect("pressed", this, nameof(_onFollowPressed));
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    public void _onFollowPressed()
    {
        EmitSignal(nameof(Player_Follow));
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}