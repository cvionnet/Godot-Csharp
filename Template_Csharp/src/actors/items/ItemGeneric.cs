using Godot;
using Nucleus;
using System;
using System.Reflection;

/// <summary>
/// Responsible for :
/// - store item properties
/// - load item sprite
/// - kill item when timer ends
/// - emitting particles before dying
/// - sending message to other scenes (characters ...)
/// </summary>
public class ItemGeneric : Area2D
{
    #region HEADER

    public CItem ItemProperties { get; private set; }

    private Timer _timerTTL;
    private Sprite _spriteGlowCircle;
    private Particles2D _particleWhenPicked;

    private AnimatedSprite instanceSprite;

    #endregion

    //*-------------------------------------------------------------------------*//

    #region GODOT METHODS

    public override void _Ready()
    {
        _timerTTL = GetNode<Timer>("TimerTTL");
        _spriteGlowCircle = GetNode<Sprite>("Glow_circle");
        _particleWhenPicked = GetNode<Particles2D>("ParticleWhenPicked");

        Connect("body_shape_entered", this, nameof(onBodyCharacterShapeEntered));
        _timerTTL.Connect("timeout", this, nameof(onDestroyItem_Timeout));

        Initialize_ItemGeneric();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    /// <summary>
    /// When a character (player/pnj) collides with an item, send information to ItemBrain
    /// </summary>
    private void onBodyCharacterShapeEntered(int body_id, Node body, int body_shape, int local_shape)
    {
        bool actionSend = false;

        if(body != null)
        {
            switch (ItemProperties.SendTo)
            {
                case StateManager.ItemsSendTo.CHARACTER:
                    // Call a method from a specific character
                    if (body.IsInGroup("Player"))
                    {
                        ((Player)body).Item_Action(ItemProperties, body.Name);
                        actionSend = true;
                    }
                    else if (body.IsInGroup("Pnj"))
                    {
                        ((Pnj)body).Item_Action(ItemProperties, body.Name);
                        actionSend = true;
                    }

                    break;
                case StateManager.ItemsSendTo.OTHER_CHARACTERS:
                case StateManager.ItemsSendTo.ALL_CHARACTERS:
                    // Call a method from all characters
                    GetTree().CallGroup("Player", "Item_Action", ItemProperties, body.Name);
                    GetTree().CallGroup("Pnj", "Item_Action", ItemProperties, body.Name);
                    actionSend = true;

                    break;
            }

            if (actionSend)
                PickedUp_Item();
        }
    }

    /// <summary>
    /// Destroy the node when time is out
    /// </summary>
    private void onDestroyItem_Timeout()
    {
        Destroy_Item();
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    private void Initialize_ItemGeneric()
    { }

    /// <summary>
    /// When the item has been picked up (hide the sprite and display the particles)
    /// </summary>
    private async void PickedUp_Item()
    {
        instanceSprite.Visible = false;
        _particleWhenPicked.Emitting = true;

        // Create a timer of 1 sec then continue to execute the code after  (process is not blocked)
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        Destroy_Item();
    }

    /// <summary>
    /// Delete the item
    /// </summary>
    private void Destroy_Item()
    {
        Nucleus_Utils.State_Manager.EmitSignal("ItemGeneric_ItemBrain_Touched", Name);    // to delete the item from the main list
        CallDeferred("queue_free");
    }

    /// <summary>
    /// Create a new item sprite instance
    /// </summary>
    private void Add_ItemSprite()
    {
        try
        {
            PackedScene scene = ResourceLoader.Load(ItemProperties.SpritePath) as PackedScene;
            if (scene != null)
            {
                instanceSprite = (AnimatedSprite)scene.Instance();
                AddChildBelowNode(_spriteGlowCircle, instanceSprite);         // to set the sprite position in the node tree (eg : to allow particles to be above the sprite)
            }
            else
            {
                throw new NullReferenceException();
            }
        }
        catch (Exception ex)
        {
            Nucleus_Utils.Error($"Error while loading Path = {ItemProperties.SpritePath}", ex, GetType().Name, MethodBase.GetCurrentMethod().Name);
        }
    }

    /// <summary>
    /// Create the time to live timer
    /// </summary>
    private void Start_TTLTimer()
    {
        _timerTTL.WaitTime = ItemProperties.TTL;
        _timerTTL.Start();
    }

    /// <summary>
    /// Initialize item properties + load sprite / start TTL timer   (called from ItemsBrain)
    /// </summary>
    public void Initialize_ItemProperties(CItem pItemProperties)
    {
        ItemProperties = pItemProperties;
        Name = "Item";  //ItemProperties.ActionName;
        AddToGroup("Items");

        Add_ItemSprite();
        Start_TTLTimer();
    }

#region ACTIONS

#endregion ACTIONS

#endregion
}