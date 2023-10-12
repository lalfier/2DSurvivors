using DSurvivors.scenes.component;
using Godot;

namespace DSurvivors.scenes.ability.sword_ability;

public partial class SwordAbility : Node2D
{
    public HitboxComponent HitboxComponent { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        HitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
    }
}