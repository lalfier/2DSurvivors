using DSurvivors.resources.upgrades;
using DSurvivors.scenes.autoload;
using DSurvivors.scenes.ui;
using DSurvivors.scripts;
using Godot;
using Godot.Collections;

namespace DSurvivors.scenes.manager;

public partial class UpgradeManager : Node
{
    [Export] private ExperienceManager _experienceManager;
    [Export] private PackedScene _upgradeScreenScene;
    [Export] private AbilityUpgrade _upgradeAxe;
    [Export] private AbilityUpgrade _upgradeAxeDamage;
    [Export] private AbilityUpgrade _upgradeSwordRate;
    [Export] private AbilityUpgrade _upgradeSwordDamage;
    [Export] private AbilityUpgrade _upgradePlayerSpeed;
    [Export] private AbilityUpgrade _upgradeAnvil;
    [Export] private AbilityUpgrade _upgradeAnvilCount;

    private readonly WeightedTable _upgradePool = new WeightedTable();

    private readonly Dictionary<string, Dictionary> _currentUpgrades = new Dictionary<string, Dictionary>();
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _upgradePool.AddItem(_upgradeAxe, 10);
        _upgradePool.AddItem(_upgradeAnvil, 10);
        _upgradePool.AddItem(_upgradeSwordRate, 10);
        _upgradePool.AddItem(_upgradeSwordDamage, 10);
        _upgradePool.AddItem(_upgradePlayerSpeed, 5);
        
        _experienceManager.LevelUp += OnLevelUp;
    }
    
    private void ApplyUpgrade(AbilityUpgrade upgrade)
    {
        var hasUpgrade = _currentUpgrades.ContainsKey(upgrade.Id);
        if (hasUpgrade == false)
        {
            _currentUpgrades.Add(upgrade.Id, new Dictionary{{"resource", upgrade},{"quantity", 1}});
        }
        else
        {
            _currentUpgrades[upgrade.Id]["quantity"] = (int)_currentUpgrades[upgrade.Id]["quantity"] + 1;
        }

        if (upgrade.MaxQuantity > 0)
        {
            var currentQuantity = (int)_currentUpgrades[upgrade.Id]["quantity"];
            if (currentQuantity == upgrade.MaxQuantity)
            {
                _upgradePool.RemoveItem(upgrade);
            }
        }
        
        UpdateUpgradePool(upgrade);
        GetNode<GameEvents>("/root/GameEvents").EmitAbilityUpgradeAdded(upgrade, _currentUpgrades);
    }

    private void UpdateUpgradePool(AbilityUpgrade chosenUpgrade)
    {
        if (chosenUpgrade.Id == _upgradeAxe.Id)
        {
            _upgradePool.AddItem(_upgradeAxeDamage, 10);
        }
        else if (chosenUpgrade.Id == _upgradeAnvil.Id)
        {
            _upgradePool.AddItem(_upgradeAnvilCount, 5);
        }
    }

    private Array<Resource> PickUpgrades()
    {
        var chosenUpgrades = new Array<Resource>();
        
        for (int i = 0; i < 2; i++)
        {
            if (_upgradePool.Items.Count == chosenUpgrades.Count)
            {
                break;
            }

            var chosenUpgrade = (AbilityUpgrade)_upgradePool.PickItem(chosenUpgrades);
            chosenUpgrades.Add(chosenUpgrade);
        }

        return chosenUpgrades;
    }

    private void OnUpgradeSelected(AbilityUpgrade upgrade)
    {
        ApplyUpgrade(upgrade);
    }
    
    private void OnLevelUp(int newLevel)
    {
        var upgradeScreenInstance = _upgradeScreenScene.Instantiate() as UpgradeScreen;
        if (upgradeScreenInstance == null)
        {
            return;
        }
        
        AddChild(upgradeScreenInstance);
        var chosenUpgrades = PickUpgrades();
        upgradeScreenInstance.SetAbilityUpgrades(chosenUpgrades);
        upgradeScreenInstance.UpgradeSelected += OnUpgradeSelected;
    }
}