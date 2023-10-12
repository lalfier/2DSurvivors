using Godot;

namespace DSurvivors.scenes.component;

public partial class HealthComponent : Node
{
	[Export] private float _maxHealth = 10;
	
	[Signal] public delegate void DiedEventHandler();
	[Signal] public delegate void HealthChangedEventHandler();
	[Signal] public delegate void HealthDecreasedEventHandler();

	private float _currentHeath;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_currentHeath = _maxHealth;
	}

	public void Damage(float damageAmount)
	{
		_currentHeath = Mathf.Clamp(_currentHeath - damageAmount, 0, _maxHealth);
		EmitSignal(SignalName.HealthChanged);
		if (damageAmount > 0)
		{
			EmitSignal(SignalName.HealthDecreased);
		}
		Callable.From(CheckDeath).CallDeferred();
	}
	
	public void Heal(float healAmount)
	{
		Damage(-healAmount);
	}

	public double GetHealthPercent()
	{
		if (_maxHealth <= 0)
		{
			return 0;
		}

		return Mathf.Min(_currentHeath / _maxHealth, 1);
	}

	private void CheckDeath()
	{
		if (_currentHeath <= 0)
		{
			_currentHeath = 0;
			EmitSignal(SignalName.Died);
			Owner.QueueFree();
		}
	}
}