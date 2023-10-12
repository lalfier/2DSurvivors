using DSurvivors.resources.meta_upgrades;
using DSurvivors.scenes.autoload;
using Godot;

namespace DSurvivors.scenes.ui;

public partial class MetaUpgradeCard : PanelContainer
{
    private Label _nameLabel;
    private Label _descriptionLabel;
    private AnimationPlayer _animationPlayer;
    private Label _progressLabel;
    private Label _countLabel;
    private ProgressBar _progressBar;
    private MetaUpgrade _upgrade;
    private Button _purchaseButton;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _nameLabel = GetNode<Label>("%NameLabel");
        _descriptionLabel = GetNode<Label>("%DescriptionLabel");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _progressLabel = GetNode<Label>("%ProgressLabel");
        _countLabel = GetNode<Label>("%CountLabel");
        _progressBar = GetNode<ProgressBar>("%ProgressBar");
        _purchaseButton = GetNode<Button>("%PurchaseButton");
        _purchaseButton.Pressed += OnPurchaseButtonPressed;
    }
    
    public void SetMetaUpgrade(MetaUpgrade upgrade)
    {
        _upgrade = upgrade;
        _nameLabel.Text = upgrade.Title;
        _descriptionLabel.Text = upgrade.Description;
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        var metaCurrency = GetNode<MetaProgression>("/root/MetaProgression").GetMetaUpgradeCurrency();
        var currentQuantity = GetNode<MetaProgression>("/root/MetaProgression").GetMetaUpgradeQuantity(_upgrade);
        var isMaxed = currentQuantity >= _upgrade.MaxQuantity;
        var percent = metaCurrency / _upgrade.ExperienceCost;
        percent = Mathf.Min(percent, 1);
        _progressBar.Value = percent;
        _purchaseButton.Disabled = percent < 1 || isMaxed;
        if (isMaxed)
        {
            _purchaseButton.Text = "Max";
        }
        _progressLabel.Text = metaCurrency.ToString("0") + "/" + _upgrade.ExperienceCost.ToString("0");
        _countLabel.Text = "x" + currentQuantity;
    }

    private void OnPurchaseButtonPressed()
    {
        if (_upgrade == null)
        {
            return;
        }
        
        GetNode<MetaProgression>("/root/MetaProgression").BuyMetaUpgrade(_upgrade);
        GetTree().CallGroup("meta_upgrade_card", "UpdateProgress");
        _animationPlayer.Play("card_selected");
    }
}