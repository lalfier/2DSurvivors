using Godot;

namespace DSurvivors.scenes.component;

public partial class HitFlashComponent : Node
{
    [Export] private HealthComponent _healthComponent;
    [Export] private Sprite2D _sprite;
    [Export] private ShaderMaterial _flashMaterial;

    private Tween _hitFlashTween;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _healthComponent.HealthDecreased += OnHealthDecreased;
        _sprite.Material = _flashMaterial;
    }

    private void OnHealthDecreased()
    {
        if (_hitFlashTween != null && _hitFlashTween.IsValid())
        {
            _hitFlashTween.Kill();
        }

        ((ShaderMaterial)_sprite.Material).SetShaderParameter("lerpPercent", 1.0);
        _hitFlashTween = CreateTween();
        _hitFlashTween.TweenProperty(_sprite.Material, "shader_parameter/lerpPercent", 0.0, 0.25)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
    }
}