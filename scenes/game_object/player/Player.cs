using DSurvivors.resources.upgrades;
using DSurvivors.scenes.autoload;
using DSurvivors.scenes.component;
using DSurvivors.scenes.manager;
using Godot;
using Godot.Collections;

namespace DSurvivors.scenes.game_object.player;

public partial class Player : CharacterBody2D
{
	[Export] private ArenaTimeManager _arenaTimeManager;
	
	public HealthComponent HealthComponent => _healthComponent;

	private Timer _damageIntervalTimer;
	private HealthComponent _healthComponent;
	private VelocityComponent _velocityComponent;
	private Node _abilities;
	private ProgressBar _healthBar;
	private AnimationPlayer _animationPlayer;
	private Node2D _visuals;
	private int _numberCollidingBodies = 0;
	private int _baseSpeed = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_healthComponent = GetNode<HealthComponent>("HealthComponent");
		_healthComponent.HealthDecreased += OnHealthDecreased;
		_healthComponent.HealthChanged += OnHealthChanged;
		
		_damageIntervalTimer = GetNode<Timer>("DamageIntervalTimer");
		_damageIntervalTimer.Timeout += OnDamageIntervalTimerTimeout;

		_arenaTimeManager.ArenaDifficultyIncreased += OnArenaDifficultyIncreased;
		
		GetNode<GameEvents>("/root/GameEvents").AbilityUpgradeAdded += OnAbilityUpgradeAdded;

		_velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
		_baseSpeed = _velocityComponent.MaxSpeed;
		
		_healthBar = GetNode<ProgressBar>("HealthBar");
		_abilities = GetNode<Node>("Abilities");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_visuals = GetNode<Node2D>("Visuals");
		
		GetNode<Area2D>("CollisionArea2D").BodyEntered += OnBodyEntered;
		GetNode<Area2D>("CollisionArea2D").BodyExited += OnBodyExited;
		
		UpdateHealthDisplay();
	}
	
	public override void _ExitTree()
	{
		GetNode<GameEvents>("/root/GameEvents").AbilityUpgradeAdded -= OnAbilityUpgradeAdded;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var movementVector = GetMovementVector();
		var direction = movementVector.Normalized();
		_velocityComponent.AccelerateInDirection(direction);
		_velocityComponent.Move(this);

		if (movementVector.X != 0 || movementVector.Y != 0)
		{
			_animationPlayer.Play("walk");
		}
		else
		{
			_animationPlayer.Play("RESET");
		}

		var moveSign = Mathf.Sign(movementVector.X);
		if (moveSign != 0)
		{
			_visuals.Scale = new Vector2(moveSign, 1);
		}
	}

	private Vector2 GetMovementVector()
	{
		var xMovement = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		var yMovement = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

		return new Vector2(xMovement, yMovement);
	}

	private void CheckDealDamage()
	{
		if (_numberCollidingBodies <= 0 || _damageIntervalTimer.IsStopped() == false)
		{
			return;
		}

		_healthComponent.Damage(1);
		_damageIntervalTimer.Start();
	}

	private void UpdateHealthDisplay()
	{
		_healthBar.Value = _healthComponent.GetHealthPercent();
	}

	private void OnHealthDecreased()
	{
		GetNode<GameEvents>("/root/GameEvents").EmitPlayerDamaged();
		GetNode<RandomStreamPlayer2DComponent>("HitRandomAudioPlayerComponent").PlayRandom();
	}
	
	private void OnHealthChanged()
	{
		UpdateHealthDisplay();
	}

	private void OnDamageIntervalTimerTimeout()
	{
		CheckDealDamage();
	}
	
	private void OnBodyEntered(Node2D otherBody)
	{
		_numberCollidingBodies += 1;
		CheckDealDamage();
	}
	
	private void OnBodyExited(Node2D otherBody)
	{
		_numberCollidingBodies -= 1;
	}
	
	private void OnAbilityUpgradeAdded(AbilityUpgrade upgrade, Dictionary<string, Dictionary> currentUpgrades)
	{
		if (upgrade is Ability)
		{
			var ability = (Ability)upgrade;
			_abilities.AddChild(ability.AbilityControllerScene.Instantiate());
		}
		else if (upgrade.Id == "player_speed")
		{
			_velocityComponent.MaxSpeed = _baseSpeed + Mathf.RoundToInt(_baseSpeed * (int)currentUpgrades["player_speed"]["quantity"] * 0.1f);
		}
	}

	private void OnArenaDifficultyIncreased(int difficulty)
	{
		var isThirtySecondInterval = (difficulty % 6) == 0;
		if (isThirtySecondInterval)
		{
			var healthRegenUpgradeQuantity = GetNode<MetaProgression>("/root/MetaProgression").GetMetaUpgradeQuantity("health_regeneration");
			if (healthRegenUpgradeQuantity > 0)
			{
				_healthComponent.Heal(healthRegenUpgradeQuantity);
			}
		}
	}
}