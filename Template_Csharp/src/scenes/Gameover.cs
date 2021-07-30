using Godot;
using System;

public class Gameover : Node
{
#region HEADER

    [Signal] private delegate void Generic_TransitionScene(string nextScene);

    private Button _buttonStart;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _buttonStart = GetNode<Button>("Button");
        _buttonStart.Connect("pressed",this , nameof(_onButtonStart_Pressed));

        Initialize_Gameover();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// Button "Play again"
    /// </summary>
    private void _onButtonStart_Pressed()
    {
        EmitSignal(nameof(Generic_TransitionScene), "Menu");    // (to SceneManager) Restart a new game
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_Gameover()
    {}

#endregion
}