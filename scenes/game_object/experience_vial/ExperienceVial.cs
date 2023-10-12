using DSurvivors.scenes.autoload;
using DSurvivors.scenes.component;
using Godot;

namespace DSurvivors.scenes.game_object.experience_vial;

public partial class ExperienceVial : Node2D
{
	private CollisionShape2D _collisionShape;
	private Sprite2D _sprite;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
		_sprite = GetNode<Sprite2D>("Sprite2D");
		GetNode<Area2D>("Area2D").AreaEntered += OnAreaEntered;
	}

	private void CollectTweenMethod(float percent, Vector2 startPosition)
	{
		var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		if (player == null)
		{
			return;
		}

		GlobalPosition = startPosition.Lerp(player.GlobalPosition, percent);
		var directionFromStart = player.GlobalPosition - startPosition;
		
		var targetRotation = directionFromStart.Angle() + Mathf.DegToRad(90);
		Rotation = Mathf.LerpAngle(Rotation, targetRotation, 1 - (float)Mathf.Exp(-2 * GetProcessDeltaTime()));
	}

	private void VialCollected()
	{
		GetNode<GameEvents>("/root/GameEvents").EmitExperienceVialCollected(1);
		QueueFree();
	}

	private void DisableVialCollision()
	{
		_collisionShape.Disabled = true;
	}
	
	private void OnAreaEntered(Area2D otherArea)
	{
		Callable.From(DisableVialCollision).CallDeferred();

		var startPosition = GlobalPosition;
		
		var tween = CreateTween();
		tween.SetParallel();
		tween.TweenMethod(Callable.From<float>(percent => CollectTweenMethod(percent, startPosition)), 0.0, 1.0, 0.5)
			.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
		tween.TweenProperty(_sprite, "scale", Vector2.Zero, 0.1).SetDelay(0.4);
		tween.Chain();
		tween.TweenCallback(Callable.From(VialCollected));
		
		GetNode<RandomStreamPlayer2DComponent>("RandomStreamPlayer2DComponent").PlayRandom();
	}
}