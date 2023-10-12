using Godot;

namespace DSurvivors.resources.upgrades;

public partial class Ability : AbilityUpgrade
{
    [Export] public PackedScene AbilityControllerScene { get; private set; }
}