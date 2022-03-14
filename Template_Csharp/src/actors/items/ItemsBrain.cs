using Godot;
using Nucleus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

/// <summary>
/// Responsible for :
/// - initialize properties for a new random item
/// - adding random items to the scene
/// - timing to create a new item
/// </summary>
public class ItemsBrain : Node2D
{
#region HEADER

    //! How to add a new item : see file "README.drawio"  (How to)

    [Export] private readonly PackedScene ItemGenericScene;
    [Export] private readonly float MinTimerNewItem = 30.0f;
    [Export] private readonly float MaxTimerNewItem = 60.0f;

    private Spawn_Factory _spawnItems;
    private Timer _timerNewItem;

    private List<ItemGeneric> _listItems = new List<ItemGeneric>();     // TODO : performance => use an ARRAY ?  (et ajouter l'id dans les propriétés du ItemGeneric)
    private List<CItem> _listUniqueItems = new List<CItem>();           // items that appear only once during the game
    private ItemGeneric _itemTouched = new ItemGeneric();

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _spawnItems = GetNode<Spawn_Factory>("Spawn_Items");
        _timerNewItem = GetNode<Timer>("TimerNewItem");

        _timerNewItem.Connect("timeout", this, nameof(onNewItemTimer_Timeout));

        Nucleus_Utils.State_Manager.Connect("ItemGeneric_ItemBrain_Touched", this, nameof(onItem_Touched));

        Initialize_ItemsBrain();
    }

    // Use to add warning in the Editor   (must add the [Tool] attribute on the class)
    public override string _GetConfigurationWarning()
    {
        return (ItemGenericScene == null) ? "The object ItemGenericScene must not be empty !" : "";
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// Time to create a new item and start a new timer
    /// </summary>
    private void onNewItemTimer_Timeout()
    {
        Create_Item();
        Initialize_TimerNewItem();
    }

    /// <summary>
    /// When a player / pnj has collided with an item, send information to final receiver
    /// </summary>
    /// <param name="ItemName">The name of the item node</param>
    private void onItem_Touched(string ItemName)
    {
        // Get the touched item properties
        _itemTouched = _listItems.Find(i => i.Name == ItemName);

        _listItems.Remove(_itemTouched);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_ItemsBrain()
    {
        _spawnItems.Load_NewScene(ItemGenericScene.ResourcePath);

        Initialize_TimerNewItem();
    }

    /// <summary>
    /// Create a new item instance
    /// </summary>
    private void Create_Item()
    {
        // Select a random item
        CItem itemProperties = new CItem();
        var random_item = (StateManager.ItemsActionList) Nucleus_Maths.Rnd.RandiRange(0, Enum.GetNames(typeof(StateManager.ItemsActionList)).Length-1);

        // Apply item properties
        switch (random_item)
        {
            case StateManager.ItemsActionList.CHARACTER_FASTER:
                itemProperties.SpritePath = "res://src/actors/items/itemSprites/ItemSprite_Player_GainSpeed.tscn";
                itemProperties.SendTo = StateManager.ItemsSendTo.CHARACTER;
                itemProperties.ActionName = random_item;
                itemProperties.ActionDuration = 3.0f;
                itemProperties.MaxVisibleInstance = 2;
                itemProperties.TTL = 10.0f;
                itemProperties.OptionalValue = 1.5f;    // the percent to apply on MaxSpeed's character
                break;
            case StateManager.ItemsActionList.OTHER_CHARACTERS_SLOWER:
                itemProperties.SpritePath = "res://src/actors/items/itemSprites/ItemSprite_OtherPlayers_LooseSpeed.tscn";
                itemProperties.SendTo = StateManager.ItemsSendTo.OTHER_CHARACTERS;
                itemProperties.ActionName = random_item;
                itemProperties.ActionDuration = 5.0f;
                itemProperties.MaxVisibleInstance = 2;
                itemProperties.TTL = 10.0f;
                itemProperties.OptionalValue = 0.5f;    // the percent to apply on MaxSpeed's characters
                break;
            case StateManager.ItemsActionList.ALL_CHARACTERS_FASTER:
                itemProperties.SpritePath = "res://src/actors/items/itemSprites/ItemSprite_AllPlayers_GainSpeed.tscn";
                itemProperties.SendTo = StateManager.ItemsSendTo.ALL_CHARACTERS;
                itemProperties.ActionName = random_item;
                itemProperties.ActionDuration = 5.0f;
                itemProperties.MaxVisibleInstance = 2;
                itemProperties.TTL = 10.0f;
                itemProperties.OptionalValue = 1.5f;    // the percent to apply on MaxSpeed's characters
                break;
        }

        // Only load the item if it is unique and already in the game AND its max instance number is not reached
        if (!_listUniqueItems.Any(i => i.ActionName == itemProperties.ActionName)
            && _listItems.Count(i => i.ItemProperties.ActionName == random_item) < itemProperties.MaxVisibleInstance)
        {
            // Initialize the item
            if (itemProperties.SpritePath != null)
            {
                Vector2 newPosition = new Vector2(Nucleus_Maths.Rnd.RandfRange(40.0f, Nucleus_Utils.ScreenWidth-40.0f), Nucleus_Maths.Rnd.RandfRange(40.0f, Nucleus_Utils.ScreenHeight-40.0f));
                //ItemGeneric instance = new ItemGeneric();
                ItemGeneric instance = _spawnItems.Add_Instance<ItemGeneric>(null, newPosition);
                _listItems.Add(instance);
                if (itemProperties.UniqueItem)
                    _listUniqueItems.Add(itemProperties);

                instance.Initialize_ItemProperties(itemProperties);
            }
            else
            {
                Nucleus_Utils.Error($"Error while loading Item Action '{random_item}' (path is null))", new NullReferenceException(), GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
        }
        else
        {
            itemProperties = null;
            //GD.Print($"{random_item} max number");
        }
    }

    private void Initialize_TimerNewItem()
    {
        _timerNewItem.WaitTime = Nucleus_Maths.Rnd.RandfRange(MinTimerNewItem, MaxTimerNewItem);
        _timerNewItem.Start();
    }

#endregion
}