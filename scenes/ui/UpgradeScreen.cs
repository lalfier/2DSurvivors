using DSurvivors.resources.upgrades;
using Godot;
using Godot.Collections;

namespace DSurvivors.scenes.ui;

public partial class UpgradeScreen : CanvasLayer
{
    [Export] private PackedScene _upgradeCardScene;
    
    [Signal] public delegate void UpgradeSelectedEventHandler(AbilityUpgrade upgrade);

    private HBoxContainer _cardContainer;
    private AnimationPlayer _animationPlayer;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _cardContainer = GetNode<HBoxContainer>("%CardContainer");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        GetTree().Paused = true;
    }
    
    public void SetAbilityUpgrades(Array<Resource> upgrades)
    {
        var delay = 0f;
        foreach (var upgrade in upgrades)
        {
            var cardInstance = _upgradeCardScene.Instantiate() as AbilityUpgradeCard;
            if (cardInstance == null)
            {
                return;
            }
            
            _cardContainer.AddChild(cardInstance);
            cardInstance.SetAbilityUpgrade((AbilityUpgrade)upgrade);
            cardInstance.PlayCardIn(delay);
            cardInstance.AbilitySelected += () => OnAbilitySelected((AbilityUpgrade)upgrade);
            delay += 0.2f;
        }
    }

    private async void OnAbilitySelected(AbilityUpgrade upgrade)
    {
        EmitSignal(SignalName.UpgradeSelected, upgrade);
        _animationPlayer.Play("screen_out");
        await ToSignal(_animationPlayer, "animation_finished");
        GetTree().Paused = false;
        QueueFree();
    }
}