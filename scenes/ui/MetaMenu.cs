using DSurvivors.resources.meta_upgrades;
using DSurvivors.scenes.autoload;
using Godot;
using Godot.Collections;

namespace DSurvivors.scenes.ui;

public partial class MetaMenu : CanvasLayer
{
    [Export] private PackedScene _metaUpgradeCardScene;
    [Export] private Array<MetaUpgrade> _metaUpgrades = new Array<MetaUpgrade>();

    private GridContainer _gridContainer;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gridContainer = GetNode<GridContainer>("%GridContainer");
        foreach (var child in _gridContainer.GetChildren())
        {
            child.QueueFree();
        }
        
        foreach (var upgrade in _metaUpgrades)
        {
            var metaUpgradeCardInstance = _metaUpgradeCardScene.Instantiate() as MetaUpgradeCard;
            if (metaUpgradeCardInstance == null)
            {
                return;
            }
            
            _gridContainer.AddChild(metaUpgradeCardInstance);
            metaUpgradeCardInstance.SetMetaUpgrade(upgrade);
        }

        GetNode<SoundButton>("%BackButton").Pressed += OnBackButtonPressed;
    }

    private void OnBackButtonPressed()
    {
        GetNode<ScreenTransition>("/root/ScreenTransition").TransitionToScene("res://scenes/ui/main_menu.tscn");
    }
}