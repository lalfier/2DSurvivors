using Godot;

namespace DSurvivors.scenes.component;

public partial class HitboxComponent : Area2D
{
    public float Damage { get; set; } = 0;
}