using Godot;
using Nucleus;

/// <summary>
/// Extends the CCharacter class with properties and methods specific to this game
/// </summary>
public partial class CCharacter
{
#region CHARACTER

    /// <summary>
    /// Update the score and send a signal to update the UI
    /// </summary>
    /// <param name="point">Can be positif or negative</param>
    public void Update_Score(int point)
    {
        // To avoid having a negative score
        if (point < 0 && (Score + point < 0))
        {
            Score = 0;
            return;
        }

        Score += point;
        Nucleus_Utils.State_Manager.EmitSignal("Player_UIPlayer_UpdateScore", Score);
    }

#endregion

#region ACTIONS

    // Store the active item action (null if no action) and its optional value
    private StateManager.ItemsActionList? _itemActionActive = null;
    private float _itemActionOptionalValue;

    /// <summary>
    /// When an item has been touched and send an action to a character
    /// </summary>
    /// <param name="pItemProperties">All properties of the item touched</param>
    /// <param name="pItemTouchedBy">The name of the node that hits the item</param>
    /// <param name="pTimerActionDuration">The Timer used in the character scene to set how much time the action will be active</param>
    public void ActionFrom_Item(CItem pItemProperties, string pItemTouchedBy, Timer pTimerActionDuration)
    {
        bool actionToExecute = false;

        // Who to apply the item effect ?
        switch (pItemProperties.SendTo)
        {
            case StateManager.ItemsSendTo.CHARACTER:
                if (_itemActionActive == null)
                {
                    // What to apply ?
                    switch (pItemProperties.ActionName)
                    {
                        case StateManager.ItemsActionList.CHARACTER_FASTER:
                            actionToExecute = true;
                            MaxSpeed *= pItemProperties.OptionalValue;
                            break;
                    }
                }

                break;
            case StateManager.ItemsSendTo.OTHER_CHARACTERS:
                if (pItemTouchedBy != Name && _itemActionActive == null)
                {
                    // What to apply ?
                    switch (pItemProperties.ActionName)
                    {
                        case StateManager.ItemsActionList.OTHER_CHARACTERS_SLOWER:
                            actionToExecute = true;
//                            Steering.Speed = MaxSpeed.x - (MaxSpeed.x * 0.3f);
                            MaxSpeed *= pItemProperties.OptionalValue;
                            break;
                    }
                }

                break;
            case StateManager.ItemsSendTo.ALL_CHARACTERS:
                if (_itemActionActive == null)
                {
                    // What to apply ?
                    switch (pItemProperties.ActionName)
                    {
                        case StateManager.ItemsActionList.ALL_CHARACTERS_FASTER:
                            actionToExecute = true;
                            MaxSpeed *= pItemProperties.OptionalValue;
                            break;
                    }
                }

                break;
        }

        // Save active state properties and start timer if an action has been founded
        if (actionToExecute)
        {
            _itemActionActive = pItemProperties.ActionName;
            _itemActionOptionalValue = pItemProperties.OptionalValue;

            pTimerActionDuration.WaitTime = pItemProperties.ActionDuration;
            pTimerActionDuration.Start();

            //GD.Print("=> " + Name + " // ActionName: " + pItemProperties.ActionName + " // ActionDuration:" + pItemProperties.ActionDuration);
        }
    }

    /// <summary>
    /// Reset the active action (called by the ActionDuration timer on characters)
    /// </summary>
    public void ActionEnd_Item()
    {
        if (_itemActionActive != null)
        {
            switch (_itemActionActive)
            {
                case StateManager.ItemsActionList.CHARACTER_FASTER:
                case StateManager.ItemsActionList.OTHER_CHARACTERS_SLOWER:
                case StateManager.ItemsActionList.ALL_CHARACTERS_FASTER:
                    //GD.Print($"=> {Name} - End action {_itemActionActive}");
                    _itemActionActive = null;
                    MaxSpeed *= 1/_itemActionOptionalValue;
                    break;
            }
        }
    }

#endregion

#region DASH MOVEMENTS

    public float Dash_SpeedBoost { get; set; }      // default percentage to boost the character maxspeed
    public bool IsDashing { get; set; } = false;

    // Save the max speed the character can raise (a dash will overcome this value)
    public Vector2 MaxSpeed_Default {
        get => _maxSpeed_Default;
        set {
            _maxSpeed_Default = value;
            MaxSpeed = _maxSpeed_Default;
        }
    }
    private Vector2 _maxSpeed_Default;

#endregion

}