using Godot;
using Nucleus;

/// <summary>
/// Responsible for :
/// - playing a fadein/out animation (emit a signal when finished)
/// </summary>
public class SceneTransition : Node
{
#region HEADER

    private AnimationPlayer _animation;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _animation = GetNode<AnimationPlayer>("AnimationPlayer");
        _animation.Connect("animation_finished", this, nameof(_onAnimation_Finished));
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    private void _onAnimation_Finished(string anim_name)
    {
        if (anim_name == "fadeToBlack")
        {
            // (send to SceneManager)
            Nucleus_Utils.State_Manager.EmitSignal("SceneTransition_AnimationFinished");
            _animation.Play("fadeToNormal");
        }
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// To activate the fade to black transition
    /// </summary>
    public void Transition_Scene()
        => _animation.Play("fadeToBlack");

#endregion
}