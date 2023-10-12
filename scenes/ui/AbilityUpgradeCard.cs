using DSurvivors.resources.upgrades;
using Godot;

namespace DSurvivors.scenes.ui;

public partial class AbilityUpgradeCard : PanelContainer
{
    [Signal] public delegate void AbilitySelectedEventHandler();
    
    private Label _nameLabel;
    private Label _descriptionLabel;
    private AnimationPlayer _animationPlayer;

    private bool _disabled = false;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _nameLabel = GetNode<Label>("%NameLabel");
        _descriptionLabel = GetNode<Label>("%DescriptionLabel");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        GuiInput += OnGuiInput;
        MouseEntered += OnMouseEntered;
    }

    public async void PlayCardIn(float delay = 0)
    {
        Modulate = Colors.Transparent;
        
        var timer = GetTree().CreateTimer(delay);
        await ToSignal(timer, "timeout");
        
        _animationPlayer.Play("card_in");
    }

    private void PlayCardDiscarded()
    {
        _animationPlayer.Play("card_discarded");
    }
    
    public void SetAbilityUpgrade(AbilityUpgrade upgrade)
    {
        _nameLabel.Text = upgrade.Name;
        _descriptionLabel.Text = upgrade.Description;
    }

    private async void SelectCard()
    {
        _disabled = true;
        _animationPlayer.Play("card_selected");

        foreach (var otherCard in GetTree().GetNodesInGroup("upgrade_card"))
        {
            if (otherCard == this)
            {
                continue;
            }
            
            ((AbilityUpgradeCard)otherCard).PlayCardDiscarded();
        }
        
        await ToSignal(_animationPlayer, "animation_finished");
        EmitSignal(SignalName.AbilitySelected);
    }

    private void OnGuiInput(InputEvent inputEvent)
    {
        if (_disabled)
        {
            return;
        }
        
        if (inputEvent.IsActionPressed("left_click"))
        {
            SelectCard();
        }
    }

    private void OnMouseEntered()
    {
        if (_disabled)
        {
            return;
        }
        
        GetNode<AnimationPlayer>("HoverAnimationPlayer").Play("card_hover");
    }
}