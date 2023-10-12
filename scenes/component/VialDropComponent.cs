using DSurvivors.scenes.autoload;
using Godot;

namespace DSurvivors.scenes.component;

public partial class VialDropComponent : Node
{
    [Export(PropertyHint.Range, "0,1")] private float _dropPercent = 0.5f;
    [Export] private HealthComponent _healthComponent;
    [Export] private PackedScene _vialScene;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _healthComponent.Died += OnDied;
    }

    private void OnDied()
    {
        var adjustedDropPercent = _dropPercent;
        var experienceGainUpgradeQuantity = GetNode<MetaProgression>("/root/MetaProgression").GetMetaUpgradeQuantity("experience_gain");
        if (experienceGainUpgradeQuantity > 0)
        {
            adjustedDropPercent += experienceGainUpgradeQuantity * 0.1f;
        }
        
        if (GD.Randf() > adjustedDropPercent)
        {
            return;
        }
        
        if (_vialScene == null)
        {
            return;
        }

        if (Owner is not Node2D)
        {
            return;
        }

        var spawnPosition = ((Node2D)Owner).GlobalPosition;
        var vialInstance = _vialScene.Instantiate() as Node2D;
        if (vialInstance == null)
        {
            return;
        }
        
        var entitiesLayer = GetTree().GetFirstNodeInGroup("entities_layer");
        entitiesLayer.AddChild(vialInstance);
        vialInstance.GlobalPosition = spawnPosition;
    }
}