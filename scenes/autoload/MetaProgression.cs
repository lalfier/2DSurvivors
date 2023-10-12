using DSurvivors.resources.meta_upgrades;
using Godot;
using Godot.Collections;

namespace DSurvivors.scenes.autoload;

public partial class MetaProgression : Node
{
    private const string SAVE_FILE_PATH = "user://game.save";
    
    private Dictionary _saveMetaData = new Dictionary{{"metaUpgradeCurrency", 0f}, {"metaUpgrades", new Dictionary()}};
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<GameEvents>("/root/GameEvents").ExperienceVialCollected += OnExperienceVialCollected;
        LoadSavedFile();
    }
    
    public override void _ExitTree()
    {
        GetNode<GameEvents>("/root/GameEvents").ExperienceVialCollected -= OnExperienceVialCollected;
    }

    private void LoadSavedFile()
    {
        if (FileAccess.FileExists(SAVE_FILE_PATH) == false)
        {
            return;
        }

        var file = FileAccess.Open(SAVE_FILE_PATH, FileAccess.ModeFlags.Read);
        _saveMetaData = file.GetVar().AsGodotDictionary();
    }

    public void SaveToFile()
    {
        var file = FileAccess.Open(SAVE_FILE_PATH, FileAccess.ModeFlags.Write);
        file.StoreVar(_saveMetaData);
    }

    private void AddMetaUpgrade(MetaUpgrade upgrade)
    {
        var upgradeQuantity = GetMetaUpgradeQuantity(upgrade);

        var metaUpgrades = _saveMetaData["metaUpgrades"].AsGodotDictionary();
        var upgradeProperties = metaUpgrades[upgrade.Id].AsGodotDictionary();
        upgradeProperties["quantity"] = upgradeQuantity + 1;
        
        metaUpgrades[upgrade.Id] = upgradeProperties;
        _saveMetaData["metaUpgrades"] = metaUpgrades;
        
        SaveToFile();
    }

    public int GetMetaUpgradeQuantity(MetaUpgrade upgrade)
    {
        return GetMetaUpgradeQuantity(upgrade.Id);
    }
    
    public int GetMetaUpgradeQuantity(string upgradeId)
    {
        if (_saveMetaData.ContainsKey("metaUpgrades") == false)
        {
            _saveMetaData.Add("metaUpgrades", new Dictionary());
        }
        
        var metaUpgrades = _saveMetaData["metaUpgrades"].AsGodotDictionary();
        if (metaUpgrades.ContainsKey(upgradeId) == false)
        {
            metaUpgrades.Add(upgradeId, new Dictionary{{"quantity", 0}});
        }

        var upgradeProperties = metaUpgrades[upgradeId].AsGodotDictionary();
        return (int)upgradeProperties["quantity"];
    }

    private void OnExperienceVialCollected(float number)
    {
        var metaCurrency = GetMetaUpgradeCurrency();
        
        _saveMetaData["metaUpgradeCurrency"] = metaCurrency + number;
    }

    public float GetMetaUpgradeCurrency()
    {
        if (_saveMetaData.ContainsKey("metaUpgradeCurrency") == false)
        {
            _saveMetaData.Add("metaUpgradeCurrency", 0f);
        }

        return (float)_saveMetaData["metaUpgradeCurrency"];
    }

    public void BuyMetaUpgrade(MetaUpgrade upgrade)
    {
        var metaCurrency = GetMetaUpgradeCurrency();
        if (metaCurrency < upgrade.ExperienceCost)
        {
            return;
        }
        
        _saveMetaData["metaUpgradeCurrency"] = metaCurrency - upgrade.ExperienceCost;
        AddMetaUpgrade(upgrade);
    }
}