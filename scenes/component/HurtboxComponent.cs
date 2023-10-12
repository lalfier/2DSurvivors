using DSurvivors.scenes.ui;
using Godot;

namespace DSurvivors.scenes.component;

public partial class HurtboxComponent : Area2D
{
    [Export] private HealthComponent _healthComponent;
    [Export] private PackedScene _floatingTextScene;
    
    [Signal] public delegate void HitEventHandler();
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D otherArea)
    {
        if (otherArea is not HitboxComponent)
        {
            return;
        }

        if (_healthComponent == null)
        {
            return;
        }

        var hitboxComponent = otherArea as HitboxComponent;
        _healthComponent.Damage(hitboxComponent.Damage);

        var floatingTextInstance = _floatingTextScene.Instantiate() as FloatingText;
        if (floatingTextInstance == null)
        {
            return;
        }
        
        GetTree().GetFirstNodeInGroup("foreground_layer").AddChild(floatingTextInstance);
        floatingTextInstance.GlobalPosition = GlobalPosition + (Vector2.Up * 16);
        var formatString = Mathf.Round(hitboxComponent.Damage) == hitboxComponent.Damage ? "0" : "0.0";
        floatingTextInstance.StartFloatingText(hitboxComponent.Damage.ToString(formatString));

        EmitSignal(SignalName.Hit);
    }
}