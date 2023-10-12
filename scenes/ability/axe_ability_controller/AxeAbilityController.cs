using DSurvivors.resources.upgrades;
using DSurvivors.scenes.ability.axe_ability;
using DSurvivors.scenes.autoload;
using Godot;
using Godot.Collections;

namespace DSurvivors.scenes.ability.axe_ability_controller;

public partial class AxeAbilityController : Node
{
    [Export] private PackedScene _axeAbilityScene;

    private Timer _timer;
    private float _baseDamage = 10;
    private float _additionalDamagePercent = 1;
    private double _baseWaitTime;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _baseWaitTime = _timer.WaitTime;
        _timer.Timeout += OnTimerTimeout;
        GetNode<GameEvents>("/root/GameEvents").AbilityUpgradeAdded += OnAbilityUpgradeAdded;
    }
    
    public override void _ExitTree()
    {
        GetNode<GameEvents>("/root/GameEvents").AbilityUpgradeAdded -= OnAbilityUpgradeAdded;
    }
    
    private void OnTimerTimeout()
    {
        var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (player == null)
        {
            return;
        }
        
        var axeAbilityInstance = _axeAbilityScene.Instantiate() as AxeAbility;
        if (axeAbilityInstance == null)
        {
            return;
        }
        
        var foregroundLayer = GetTree().GetFirstNodeInGroup("foreground_layer");
        foregroundLayer.AddChild(axeAbilityInstance);
        axeAbilityInstance.HitboxComponent.Damage = _baseDamage * _additionalDamagePercent;
        axeAbilityInstance.GlobalPosition = player.GlobalPosition;
    }

    private void OnAbilityUpgradeAdded(AbilityUpgrade upgrade, Dictionary<string, Dictionary> currentUpgrades)
    {
        if (upgrade.Id == "axe_damage")
        {
            _additionalDamagePercent = 1 + ((int)currentUpgrades["axe_damage"]["quantity"] * 0.1f);
        }
    }
}