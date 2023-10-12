using Godot;

namespace DSurvivors.scenes.ui;

public partial class FloatingText : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void StartFloatingText(string text)
    {
        GetNode<Label>("Label").Text = text;
        
        var tween = CreateTween();
        tween.SetParallel();
        
        tween.TweenProperty(this, "global_position", GlobalPosition + (Vector2.Up * 16), 0.3)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        tween.Chain();
        
        tween.TweenProperty(this, "global_position", GlobalPosition + (Vector2.Up * 48), 0.5)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(this, "scale", Vector2.Zero, 0.5)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
        tween.Chain();
        
        tween.TweenCallback(Callable.From(QueueFree));

        var scaleTween = CreateTween();
        scaleTween.TweenProperty(this, "scale", Vector2.One * 1.5f, 0.15)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        scaleTween.TweenProperty(this, "scale", Vector2.One, 0.15)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
    }
}