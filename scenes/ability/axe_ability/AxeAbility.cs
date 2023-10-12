using DSurvivors.scenes.component;
using Godot;

namespace DSurvivors.scenes.ability.axe_ability;

public partial class AxeAbility : Node2D
{
    private const float MAX_RADIUS = 100;
    
    public HitboxComponent HitboxComponent { get; private set; }

    private Vector2 _baseRotation = Vector2.Right;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _baseRotation = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau));
        
        var tween = CreateTween();
        tween.TweenMethod(Callable.From<float>(RotateAxeTweenMethod), 0.0, 2.0, 3.0);
        tween.TweenCallback(Callable.From(QueueFree));

        HitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
    }

    private void RotateAxeTweenMethod(float rotations)
    {
        var percent = rotations / 2;
        var currentRadius = percent * MAX_RADIUS;
        var currentDirection = _baseRotation.Rotated(rotations * Mathf.Tau);

        var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (player == null)
        {
            return;
        }
        
        GlobalPosition = player.GlobalPosition + (currentDirection * currentRadius);
    }
}