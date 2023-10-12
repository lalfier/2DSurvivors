using DSurvivors.scripts;
using Godot;

namespace DSurvivors.scenes.manager;

public partial class EnemyManager : Node
{
	private const float SPAWN_RADIUS = 375;
	
	[Export] private PackedScene _basicEnemyScene;
	[Export] private PackedScene _wizardEnemyScene;
	[Export] private PackedScene _batEnemyScene;
	[Export] private ArenaTimeManager _arenaTimeManager;

	private Timer _timer;
	private double _baseSpawnTime = 0;
	private WeightedTable _enemyTable = new WeightedTable();
	private int _numberToSpawn = 1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_enemyTable.AddItem(_basicEnemyScene, 10);
		
		_timer = GetNode<Timer>("Timer");
		_baseSpawnTime = _timer.WaitTime;
		_timer.Timeout += OnTimerTimeout;
		_arenaTimeManager.ArenaDifficultyIncreased += OnArenaDifficultyIncreased;
	}

	private Vector2 GetSpawnPosition()
	{
		var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		if (player == null)
		{
			return Vector2.Zero;
		}

		var spawnPosition = Vector2.Zero;
		var randomDirection = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
		for (int i = 0; i < 4; i++)
		{
			spawnPosition = player.GlobalPosition + (randomDirection * SPAWN_RADIUS);
			var additionalCheckOffset = randomDirection * 20;

			var queryParameters = PhysicsRayQueryParameters2D.Create(player.GlobalPosition, spawnPosition + additionalCheckOffset, 1<<0);
			var result = GetTree().Root.World2D.DirectSpaceState.IntersectRay(queryParameters);
			if (result.Count <= 0)
			{
				break;
			}
			
			randomDirection = randomDirection.Rotated(Mathf.DegToRad(90));
		}

		return spawnPosition;
	}

	private void OnTimerTimeout()
	{
		_timer.Start();
		
		var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		if (player == null)
		{
			return;
		}

		for (int i = 0; i < _numberToSpawn; i++)
		{
			var enemyScene = _enemyTable.PickItem();
			if (enemyScene == null)
			{
				return;
			}
		
			var enemy = ((PackedScene)enemyScene).Instantiate() as Node2D;
			if (enemy == null)
			{
				return;
			}

			var entitiesLayer = GetTree().GetFirstNodeInGroup("entities_layer");
			entitiesLayer.AddChild(enemy);
			enemy.GlobalPosition = GetSpawnPosition();
		}
	}

	private void OnArenaDifficultyIncreased(int arenaDifficulty)
	{
		var timeOff = (0.1 / 12) * arenaDifficulty;
		timeOff = Mathf.Min(timeOff, 0.7);
		_timer.WaitTime = _baseSpawnTime - timeOff;

		if (arenaDifficulty == 6)
		{
			_enemyTable.AddItem(_wizardEnemyScene, 15);
		}
		else if (arenaDifficulty == 18)
		{
			_enemyTable.AddItem(_batEnemyScene, 8);
		}

		if (arenaDifficulty % 6 == 0)
		{
			_numberToSpawn += 1;
		}
	}
}