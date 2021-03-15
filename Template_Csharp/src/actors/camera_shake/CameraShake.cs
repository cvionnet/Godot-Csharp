using Godot;
using System;

public class CameraShake : Camera2D
{
#region HEADER

    [Export] private OpenSimplexNoise Noise;

    private Timer _shakeLength;
    private Tween _tween;
    private ColorRect _flash;

    private Vector2 _shakeMaxOffset = new Vector2(150, 150);  	// maximum horizontal & vertical shake (in pixels)
    private float _shakeMaxRoll = 25;     // maximum rotation

    private float _speed = 150.0f;
    private float _time = 0.0f;

    private float _shakeAmplitude = 0.0f;   // current shake strength [0, 1]
    private int _shakeAmplitudePower = 2;  	// trauma exponent ([2, 3])

    private bool _isShaking = false;
    private bool _onXAxis = true;
    private bool _onYAxis = true;
    private bool _useRotation = false;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _shakeLength = GetNode<Timer>("ShakeLength");
        _tween = GetNode<Tween>("Tween");
        _flash = GetNode<ColorRect>("Flash");

        _shakeLength.Connect("timeout", this, nameof(_ShakeLength_Timeout));
    }

    public override void _Process(float delta)
    {
        if(_isShaking)
            _Shake(delta);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    // When the shake ends
    private void _ShakeLength_Timeout()
    {
        _isShaking = false;
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// Process the shake
    /// </summary>
    private void _Shake(float delta)
    {
        _time += delta;

    	float amount = Mathf.Pow(_shakeAmplitude, _shakeAmplitudePower);

        float x = _onXAxis ? Noise.GetNoise3d(_time * _speed, 0.0f, 0.0f) * _shakeMaxOffset.x * amount : 0.0f;
        float y = _onYAxis ? Noise.GetNoise3d(0.0f, _time * _speed, 0.0f) * _shakeMaxOffset.y * amount : 0.0f;
        Offset = new Vector2(x, y);

        if (_useRotation)
            Rotation = Noise.GetNoise3d(0.0f, 0.0f, _time * _speed) * _shakeMaxRoll * amount;
    }

    /// <summary>
    /// Start to shake the camera and flash the screen in white
    ///     EG : simple shake  : _camera.Start_Shake(0.5f, 600.0f, 0.3f, true, false);
    ///     EG : shake + flash : _camera.Start_Shake(0.3f, 250.0f, 0.8f, true, true, false, 0.1f, 0.5f);
    /// </summary>
    /// <param name="pDuration">Shake : how long will it lasts</param>
    /// <param name="pSpeed">Shake : how fast will it moves</param>
    /// <param name="pStrength">Shake : how far will it goes (amplitude [0, 1])</param>
    /// <param name="pXAxis">Shake : true to move on X axis</param>
    /// <param name="pYAxis">Shake : true to move on Y axis</param>
    /// <param name="pUseRotation">Shake : true to allow rotation</param>
    /// <param name="pFlashScreen_Speed">Flash : how fast will it lasts</param>
    /// <param name="pFlashScreen_Strength">Flash : alpha intensity (1.0f = no transparency)</param>
    public void Start_Shake(float pDuration, float pSpeed, float pStrength, bool pXAxis = true, bool pYAxis = true, bool pUseRotation = false, float pFlashScreen_Speed = 0.0f, float pFlashScreen_Strength = 0.0f)
    {
        // Shake
        _shakeAmplitude = pStrength;
        _speed = pSpeed;
        _onXAxis = pXAxis;
        _onYAxis = pYAxis;
        _useRotation = pUseRotation;

        _shakeLength.Start(pDuration);  // when the shake ends
        _isShaking = true;

        // Flash
        if (pFlashScreen_Speed != 0.0f)
            _Start_Flash(pFlashScreen_Speed, pFlashScreen_Strength);
    }

    /// <summary>
    /// Start to flash the screen
    /// </summary>
    /// <param name="pSpeed">How fast</param>
    /// <param name="pStrength">Alpha intensity (1.0f = no transparency)</param>
    async private void _Start_Flash(float pSpeed, float pStrength)
    {
        _tween.InterpolateProperty(_flash, "modulate:a", 0, pStrength, pSpeed, Tween.TransitionType.Sine, Tween.EaseType.Out);
        _tween.Start();

        await ToSignal(GetTree().CreateTimer(pSpeed), "timeout");
        _tween.InterpolateProperty(_flash, "modulate:a", pStrength, 0, pSpeed, Tween.TransitionType.Sine, Tween.EaseType.Out);
        _tween.Start();
    }

#endregion
}



/*
    another solution using this tutorial : https://www.youtube.com/watch?v=NivuvkbIGH4
    ATTENTION : need to add a Timer in the scene (name : ShakeRefresh)


using Godot;
using System;

public class CameraShake : Camera2D
{
#region HEADER

    private Timer _shakeLength;
    private Timer _shakeRefresh;
    private Tween _tween;
    private ColorRect _flash;

    private float _speed = 0.0f;
    private float _strength = 0.0f;
    private bool _isShaking = false;

#endregion


#region GODOT METHODS

    public override void _Ready()
    {
        _shakeLength = GetNode<Timer>("ShakeLength");
        _shakeRefresh = GetNode<Timer>("ShakeRefresh");
        _tween = GetNode<Tween>("Tween");
        _flash = GetNode<ColorRect>("Flash");

        _shakeLength.Connect("timeout", this, nameof(_ShakeLength_Timeout));
        _shakeRefresh.Connect("timeout", this, nameof(_ShakeRefresh_Timeout));
    }

#endregion


#region SIGNAL CALLBACKS

    // When the shake ends
    private void _ShakeLength_Timeout()
    {
        _isShaking = false;
        Reset_Camera();
    }

    // Generate new shake's random values
    private void _ShakeRefresh_Timeout()
    {
        if(_isShaking)
        {
            Vector2 strength = new Vector2(Utils.Rndf_AvoidZero(-_strength, _strength), Utils.Rndf_AvoidZero(-_strength, _strength));
            _tween.InterpolateProperty(this, "offset", Offset, strength, _speed, Tween.TransitionType.Sine, Tween.EaseType.Out);
            _tween.Start();
        }
    }
#endregion


#region USER METHODS

    /// <summary>
    /// Reset the camera on the center position
    /// </summary>
    private void Reset_Camera()
    {
        _tween.InterpolateProperty(this, "offset", Offset, new Vector2(0,0), _speed, Tween.TransitionType.Sine, Tween.EaseType.Out);
        _tween.Start();
    }

    /// <summary>
    /// Start to shake the camera and flash the screen in white
    ///     EG : simple shake  : _camera.Start_Shake(0.5f, 0.04f, 3.0f);
    ///     EG : shake + flash : _camera.Start_Shake(0.3f, 0.04f, 2.0f, 0.1f, 0.5f);
    /// </summary>
    /// <param name="pDuration">Shake : how long will it lasts</param>
    /// <param name="pSpeed">Shake : how fast will it moves</param>
    /// <param name="pStrength">Shake : how far will it goes (amplitude)</param>
    /// <param name="pFlashScreen_Speed">Flash : how fast will it lasts</param>
    /// <param name="pFlashScreen_Strength">Flash : alpha intensity (1.0f = no transparency)</param>
    public void Start_Shake(float pDuration, float pSpeed, float pStrength, float pFlashScreen_Speed = 0.0f, float pFlashScreen_Strength = 0.0f)
    {
        // Shake
        _isShaking = true;
        _strength = pStrength;
        _speed = pSpeed;

        _shakeLength.Start(pDuration);  // when the shake ends
        _shakeRefresh.Start(pSpeed);    // refresh shake's random values

        // Flash
        if (pFlashScreen_Speed != 0.0f)
            _Start_Flash(pFlashScreen_Speed, pFlashScreen_Strength);
    }

    /// <summary>
    /// Start to flash the screen
    /// </summary>
    /// <param name="pSpeed">How fast</param>
    /// <param name="pStrength">Alpha intensity (1.0f = no transparency)</param>
    async private void _Start_Flash(float pSpeed, float pStrength)
    {
        _tween.InterpolateProperty(_flash, "modulate:a", 0, pStrength, pSpeed, Tween.TransitionType.Sine, Tween.EaseType.Out);
        _tween.Start();

        await ToSignal(GetTree().CreateTimer(pSpeed), "timeout");
        _tween.InterpolateProperty(_flash, "modulate:a", pStrength, 0, pSpeed, Tween.TransitionType.Sine, Tween.EaseType.Out);
        _tween.Start();
    }
#endregion
}

*/

