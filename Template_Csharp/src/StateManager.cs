using System.Collections.Generic;
using Godot;
using Nucleus;

/// <summary>
/// HOW TO :
///     In Godot, add this file as Autoload :  Project > Project Settings > AutoLoad    (Node name = StateManager)
/// IN THE CALLING SCENE :
///     StateManager _stateManager = GetNode<StateManager>("/root/StateManager");
/// </summary>
public class StateManager : Node
{
#region HEADER
    // ITEM - To store all items actions
    public enum ItemsActionList { CHARACTER_FASTER, OTHER_CHARACTERS_SLOWER, ALL_CHARACTERS_FASTER }
    public enum ItemsSendTo { CHARACTER, OTHER_CHARACTERS, ALL_CHARACTERS }

    // PLAYER
    public float ZoomLevel_GAME { get; } = 0.5f;
    public float ZoomLevel_ZOOMOUT { get; } = 1.0f;

    // GAME
    public List<CLevel> LevelList { get; set; } = new List<CLevel>();
    public CLevel LevelActive { get; set; }

#endregion

#region SIGNALS DECLARATION

    [Signal] public delegate void Generic_TransitionScene(string nextScene);
    [Signal] public delegate void SceneTransition_AnimationFinished();
    [Signal] public delegate void UIPlayer_GameBrain_LevelTimeout();

    [Signal] public delegate void Player_UIPlayer_UpdateScore(int score);

    [Signal] public delegate void ItemGeneric_ItemBrain_Touched(string ItemName);

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

#endregion
}