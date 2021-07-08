using Godot;

public class SceneTransition : Node
{
#region HEADER

    [Signal] private delegate void SceneTransition_AnimationFinished();

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
            EmitSignal(nameof(SceneTransition_AnimationFinished));
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
    {
        _animation.Play("fadeToBlack");
    }

#endregion
}