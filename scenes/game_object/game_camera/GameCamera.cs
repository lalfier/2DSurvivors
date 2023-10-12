using Godot;

namespace DSurvivors.scenes.game_object.game_camera;

public partial class GameCamera : Camera2D
{
	private Vector2 _targetPosition = Vector2.Zero;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MakeCurrent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		AcquireTarget();
		var weightLerp = (float)(1.0 - Mathf.Exp(-delta * 20));
		GlobalPosition = GlobalPosition.Lerp(_targetPosition, weightLerp);
	}

	private void AcquireTarget()
	{
		var playerNodes = GetTree().GetNodesInGroup("player");
		if (playerNodes.Count > 0)
		{
			var player = (Node2D)playerNodes[0];
			_targetPosition = player.GlobalPosition;
		}
	}
}