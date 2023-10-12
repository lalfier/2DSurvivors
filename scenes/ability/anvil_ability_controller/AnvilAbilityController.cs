using DSurvivors.resources.upgrades;
using DSurvivors.scenes.ability.anvil_ability;
using DSurvivors.scenes.autoload;
using Godot;
using Godot.Collections;

namespace DSurvivors.scenes.ability.anvil_ability_controller;

public partial class AnvilAbilityController : Node
{
    private const float BASE_RANGE = 100;
    
    [Export] private PackedScene _anvilAbilityScene;
    
    private Timer _timer;
    private float _baseDamage = 15;
    private float _additionalDamagePercent = 1;
    private double _baseWaitTime;
    private int _anvilCount = 0;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _baseWaitTime = _timer.WaitTime;
        _timer.Timeout += OnTimerTimeout;
        GetNode<GameEvents>("/root/GameEvents").AbilityUpgradeAdded += OnAbilityUpgradeAdded;
    }
    
    private void OnTimerTimeout()
    {
        var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (player == null)
        {
            return;
        }
        
        var direction = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
        var additionalRotationDegrees = 360.0 / (_anvilCount + 1);
        var anvilDistance = (float)GD.RandRange(0, BASE_RANGE);
        for (int i = 0; i < _anvilCount + 1; i++)
        {
            var adjustedDirection = direction.Rotated((float)Mathf.DegToRad(i * additionalRotationDegrees));
            var spawnPosition = player.GlobalPosition + (adjustedDirection * anvilDistance);
        
            var queryParameters = PhysicsRayQueryParameters2D.Create(player.GlobalPosition, spawnPosition, 1<<0);
            var result = GetTree().Root.World2D.DirectSpaceState.IntersectRay(queryParameters);
            if (result.Count > 0)
            {
                spawnPosition = (Vector2)result["position"];
            }

            var anvilAbilityInstance = _anvilAbilityScene.Instantiate() as AnvilAbility;
            if (anvilAbilityInstance == null)
            {
                return;
            }
        
            GetTree().GetFirstNodeInGroup("foreground_layer").AddChild(anvilAbilityInstance);
            anvilAbilityInstance.GlobalPosition = spawnPosition;
            anvilAbilityInstance.HitboxComponent.Damage = _baseDamage * _additionalDamagePercent;
        }
    }
    
    private void OnAbilityUpgradeAdded(AbilityUpgrade upgrade, Dictionary<string, Dictionary> currentUpgrades)
    {
        if (upgrade.Id == "anvil_count")
        {
            _anvilCount = (int)currentUpgrades["anvil_count"]["quantity"];
        }
    }
}