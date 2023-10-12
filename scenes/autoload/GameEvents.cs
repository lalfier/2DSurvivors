using DSurvivors.resources.upgrades;
using Godot;
using Godot.Collections;

namespace DSurvivors.scenes.autoload;

public partial class GameEvents : Node
{
    [Signal] public delegate void ExperienceVialCollectedEventHandler(float number);
    [Signal] public delegate void AbilityUpgradeAddedEventHandler(AbilityUpgrade upgrade, Dictionary<string, Dictionary> currentUpgrades);
    [Signal] public delegate void PlayerDamagedEventHandler();

    public void EmitExperienceVialCollected(float number)
    {
        EmitSignal(SignalName.ExperienceVialCollected, number);
    }
    
    public void EmitAbilityUpgradeAdded(AbilityUpgrade upgrade, Dictionary<string, Dictionary> currentUpgrades)
    {
        EmitSignal(SignalName.AbilityUpgradeAdded, upgrade, currentUpgrades);
    }

    public void EmitPlayerDamaged()
    {
        EmitSignal(SignalName.PlayerDamaged);
    }
}