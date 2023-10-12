using DSurvivors.scenes.autoload;
using Godot;

namespace DSurvivors.scenes.ui;

public partial class PauseMenu : CanvasLayer
{
    [Export] private PackedScene _optionsScene;
    
    private PanelContainer _panelContainer;
    private bool _isClosing = false;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetTree().Paused = true;
        
        _panelContainer = GetNode<PanelContainer>("%PanelContainer");
        _panelContainer.PivotOffset = _panelContainer.Size / 2;
        
        GetNode<Button>("%ResumeButton").Pressed += OnResumePressed;
        GetNode<Button>("%OptionsButton").Pressed += OnOptionsPressed;
        GetNode<Button>("%QuitButton").Pressed += OnQuitPressed;
        
        GetNode<AnimationPlayer>("AnimationPlayer").Play("screen_in_out");

        var tween = CreateTween();
        tween.TweenProperty(_panelContainer, "scale", Vector2.Zero, 0);
        tween.TweenProperty(_panelContainer, "scale", Vector2.One, 0.3)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            Close();
            GetTree().Root.SetInputAsHandled();
        }
    }

    private async void Close()
    {
        if (_isClosing)
        {
            return;
        }

        _isClosing = true;
        GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("screen_in_out");
        
        var tween = CreateTween();
        tween.TweenProperty(_panelContainer, "scale", Vector2.One, 0);
        tween.TweenProperty(_panelContainer, "scale", Vector2.Zero, 0.3)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);
        
        await ToSignal(tween, "finished");
        
        GetTree().Paused = false;
        QueueFree();
    }
    
    private void OnResumePressed()
    {
        Close();
    }
    
    private async void OnOptionsPressed()
    {
        GetNode<ScreenTransition>("/root/ScreenTransition").Transition();
        await ToSignal(GetNode<ScreenTransition>("/root/ScreenTransition"), "TransitionedHalfway");
        
        var optionsInstance = _optionsScene.Instantiate() as OptionsMenu;
        if (optionsInstance == null)
        {
            return;
        }
        
        AddChild(optionsInstance);
        optionsInstance.BackPressed += () => OnOptionsClosed(optionsInstance);
    }
    
    private async void OnQuitPressed()
    {
        GetNode<ScreenTransition>("/root/ScreenTransition").Transition();
        await ToSignal(GetNode<ScreenTransition>("/root/ScreenTransition"), "TransitionedHalfway");
        
        Close();
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://scenes/ui/main_menu.tscn");
    }
    
    private void OnOptionsClosed(Node optionsInstance)
    {
        optionsInstance.QueueFree();
    }
}