using Godot;
using System;

public class Player_Skin : Node2D
{
#region HEADER

    [Signal] private delegate void AnimationFinished(string anim_name);

    private AnimationPlayer _anim;

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS

    public override void _Ready()
    {
        _anim = GetNode<AnimationPlayer>("AnimationPlayer");

        _anim.Connect("animation_finished", this, nameof(_on_AnimationFinished));
    }

#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS

    public void _on_AnimationFinished(string anim_name)
    {
        EmitSignal(nameof(AnimationFinished), anim_name);
    }

#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    public void PlayAnimation(string anim_name)
    {
        if (!Array.Exists(_anim.GetAnimationList(), p => p == anim_name))
            return;

        _anim.Play(anim_name);
    }

#endregion
}