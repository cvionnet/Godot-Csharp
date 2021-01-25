using Godot;
using System;

[Tool]
public class Game : Node
{
#region HEADER

	[Export] public PackedScene MinionScene;
	[Export] public int MinionMaxNumber = 20;

	private Player _player;
	private Minion _minionInstance;
	private int _minionCount;
	private Vector2 _startPosition;

	// Fog of war
	//private const string Light_ImagePath = "res://assets/scenes/light_fow.png";
	//[Export] public String FOW_Light_Image;
	private Sprite _fowSprite;
	private Image _fogImage;
	private ImageTexture _fogImageTexture;
	private Image _lightImage;
	private Vector2 _lightOffset;
	[Export] public int FOW_Radius = 6;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

	// Called when the node enters the scene tree for the first time
	public override void _Ready()
	{
		Utils.Initialize_Utils(GetViewport());

		_player = GetNode<Player>("Player");
		_startPosition = GetNode<Position2D>("StartPosition").Position;

		_FOW_Initialize();

		_CreateMinion(MinionMaxNumber);
	}

	public override void _Process(float delta)
	{
		_FOW_Update(_player.Position / FOW_Radius);
	}



	// Use to add warning in the Editor   (must add the [Tool] attribute on the class)
	public override string _GetConfigurationWarning()
	{
		return (MinionScene == null) ? "'Minion Scene' property must not be empty !" : "";
	}

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

	/// <summary>
	/// Create new instance of the Minion
	/// </summary>
	/// <param name="pCount">How much Minion to create</param>
	private void _CreateMinion(int pCount)
	{
		for (int i = 0; i < pCount; i++)
		{
			if (_minionCount >= MinionMaxNumber)
				break;

			_minionInstance = (Minion)MinionScene.Instance();
			_minionInstance.Position = _startPosition;
			GetNode("Followers").AddChild(_minionInstance);
			_minionCount++;
		}
	}

	private void _FOW_Initialize()
	{
		_fowSprite = GetNode<Sprite>("Fog");
		_fowSprite.Scale *= FOW_Radius;

		_fogImageTexture = new ImageTexture();

		// Create a black image on the whole screen
		_fogImage = new Image();
		//_fogImage.Create(Mathf.FloorToInt(Utils.ScreenHeight), Mathf.FloorToInt(Utils.ScreenHeight), false, Image.Format.Rgbah);
		_fogImage.Create(Mathf.FloorToInt(Utils.ScreenWidth / FOW_Radius), Mathf.FloorToInt(Utils.ScreenHeight / FOW_Radius), false, Image.Format.Rgbah);
		_fogImage.Fill(Colors.Black);

		// Load the light image
		_lightImage = new Image();
//		_lightImage.Load(FOW_Light_Image.ResourcePath);
		_lightImage.Load(_fowSprite.Texture.ResourcePath);
		_lightOffset = new Vector2(_lightImage.GetWidth()/2, _lightImage.GetHeight()/2);	// to center the image on the destination
		_lightImage.Convert(Image.Format.Rgbah);	// apply the same format as _fogImage to allow to draw on it
	}

	private void _FOW_Update(Vector2 pNewPosition)
	{
		_fogImage.Lock();
		_lightImage.Lock();

		// Draw the light image onto the black image
		Rect2 light_rect = new Rect2(Utils.VECTOR_0, new Vector2(_lightImage.GetWidth(), _lightImage.GetHeight()));
		_fogImage.BlendRect(_lightImage, light_rect, pNewPosition - _lightOffset);

		_fogImage.Unlock();
		_lightImage.Unlock();

		_FOW_Update_ImageTexture();
	}

	private void _FOW_Update_ImageTexture()
	{
		_fogImageTexture.CreateFromImage(_fogImage);
		_fowSprite.Texture = _fogImageTexture;
	}

#endregion
}
