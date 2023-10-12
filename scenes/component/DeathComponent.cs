using Godot;

namespace DSurvivors.scenes.component;

public partial class DeathComponent : Node2D
{
    [Export] private HealthComponent _healthComponent;
    [Export] private Sprite2D _sprite;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<GpuParticles2D>("GPUParticles2D").Texture = _sprite.Texture;
        _healthComponent.Died += OnDied;
    }
    
    private void OnDied()
    {
        if (Owner == null || Owner is not Node2D)
        {
            return;
        }

        var spawnPosition = ((Node2D)Owner).GlobalPosition;
        
        var entities = GetTree().GetFirstNodeInGroup("entities_layer");
        GetParent().RemoveChild(this);
        entities.AddChild(this);

        GlobalPosition = spawnPosition;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("death");
        GetNode<RandomStreamPlayer2DComponent>("HitRandomAudioPlayerComponent").PlayRandom();
    }
}