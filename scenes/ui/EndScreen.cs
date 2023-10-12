using DSurvivors.scenes.autoload;
using Godot;

namespace DSurvivors.scenes.ui;

public partial class EndScreen : CanvasLayer
{
    private PanelContainer _panelContainer;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _panelContainer = GetNode<PanelContainer>("%PanelContainer");
        _panelContainer.PivotOffset = _panelContainer.Size / 2;
        _panelContainer.Scale = Vector2.Zero;
        var tween = CreateTween();
        tween.TweenProperty(_panelContainer, "scale", Vector2.Zero, 0f);
        tween.TweenProperty(_panelContainer, "scale", Vector2.One, 0.3f)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
        
        GetTree().Paused = true;
        GetNode<Button>("%ContinueButton").Pressed += OnContinueButtonPressed;
        GetNode<Button>("%QuitButton").Pressed += OnQuitButtonPressed;
    }

    public void SetDefeat()
    {
        GetNode<Label>("%TitleLabel").Text = "Defeat";
        GetNode<Label>("%DescriptionLabel").Text = "You lost!";
        PlayJingle(true);
    }

    public void PlayJingle(bool defeat = false)
    {
        if (defeat)
        {
            GetNode<AudioStreamPlayer>("DefeatStreamPlayer").Play();
        }
        else
        {
            GetNode<AudioStreamPlayer>("VictoryStreamPlayer").Play();
        }
    }

    private void OnContinueButtonPressed()
    {
        GetNode<ScreenTransition>("/root/ScreenTransition").TransitionToScene("res://scenes/ui/meta_menu.tscn", true);
    }
    
    private void OnQuitButtonPressed()
    {
        GetNode<ScreenTransition>("/root/ScreenTransition").TransitionToScene("res://scenes/ui/main_menu.tscn", true);
    }
}