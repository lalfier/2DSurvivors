using System.Linq;
using DSurvivors.resources.upgrades;
using DSurvivors.scenes.ability.sword_ability;
using DSurvivors.scenes.autoload;
using Godot;
using Godot.Collections;

namespace DSurvivors.scenes.ability.sword_ability_controller;

public partial class SwordAbilityController : Node
{
	private const float MAX_RANGE = 150;
	
	[Export] private PackedScene _swordAbilityScene;

	private Timer _timer;
	private float _baseDamage = 5;
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
		
		var enemies = GetTree().GetNodesInGroup("enemy").ToList();
		var sortedEnemies = enemies.ConvertAll(e => (Node2D)e)
			.Where(e => e.GlobalPosition.DistanceSquaredTo(player.GlobalPosition) < Mathf.Pow(MAX_RANGE, 2)).ToList();
		if (sortedEnemies.Count <= 0)
		{
			return;
		}

		var closestEnemies = sortedEnemies.OrderBy(a => a.GlobalPosition.DistanceSquaredTo(player.GlobalPosition)).ToList();
		
		var swordAbilityInstance = _swordAbilityScene.Instantiate() as SwordAbility;
		if (swordAbilityInstance == null)
		{
			return;
		}
		
		var foregroundLayer = GetTree().GetFirstNodeInGroup("foreground_layer");
		foregroundLayer.AddChild(swordAbilityInstance);
		swordAbilityInstance.HitboxComponent.Damage = _baseDamage * _additionalDamagePercent;
		swordAbilityInstance.GlobalPosition = closestEnemies[0].GlobalPosition;
		swordAbilityInstance.GlobalPosition += Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau)) * 4;

		var enemyDirection = closestEnemies[0].GlobalPosition - swordAbilityInstance.GlobalPosition;
		swordAbilityInstance.Rotation = enemyDirection.Angle();
	}

	private void OnAbilityUpgradeAdded(AbilityUpgrade upgrade, Dictionary<string, Dictionary> currentUpgrades)
	{
		if (upgrade.Id == "sword_rate")
		{
			var percentReduction = (int)currentUpgrades["sword_rate"]["quantity"] * 0.1f;
			_timer.WaitTime = _baseWaitTime * (1 - percentReduction);
			_timer.Start();
		}
		else if (upgrade.Id == "sword_damage")
		{
			_additionalDamagePercent = 1 + ((int)currentUpgrades["sword_damage"]["quantity"] * 0.15f);
		}
	}
}