using Godot;

namespace DSurvivors.scenes.component;

public partial class VelocityComponent : Node
{
    [Export] private int _maxSpeed = 40;
    [Export] private float _acceleration = 5;

    public int MaxSpeed
    {
        get => _maxSpeed;
        set => _maxSpeed = value;
    }

    private Vector2 _velocity = Vector2.Zero;

    public void AccelerateToPlayer()
    {
        var owner = Owner as Node2D;
        if (owner == null)
        {
            return;
        }

        var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (player == null)
        {
            return;
        }

        var direction = (player.GlobalPosition - owner.GlobalPosition).Normalized();
        AccelerateInDirection(direction);
    }

    public void AccelerateInDirection(Vector2 direction)
    {
        var desiredVelocity = direction * _maxSpeed;
        _velocity = _velocity.Lerp(desiredVelocity, 1 - (float)Mathf.Exp(-_acceleration * GetProcessDeltaTime()));
    }

    public void Decelerate()
    {
        AccelerateInDirection(Vector2.Zero);
    }

    public void Move(CharacterBody2D characterBody)
    {
        characterBody.Velocity = _velocity;
        characterBody.MoveAndSlide();
        _velocity = characterBody.Velocity;
    }
}