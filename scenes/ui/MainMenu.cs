using DSurvivors.scenes.autoload;
using Godot;

namespace DSurvivors.scenes.ui;

public partial class MainMenu : CanvasLayer
{
    [Export] private PackedScene _optionsScene;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<Button>("%PlayButton").Pressed += OnPlayPressed;
        GetNode<Button>("%UpgradesButton").Pressed += OnUpgradesPressed;
        GetNode<Button>("%OptionsButton").Pressed += OnOptionsPressed;
        GetNode<Button>("%QuitButton").Pressed += OnQuitPressed;
    }

    private void OnPlayPressed()
    {
        GetNode<ScreenTransition>("/root/ScreenTransition").TransitionToScene("res://scenes/main/main.tscn");
    }
    
    private void OnUpgradesPressed()
    {
        GetNode<ScreenTransition>("/root/ScreenTransition").TransitionToScene("res://scenes/ui/meta_menu.tscn");
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
        GetTree().Quit();
    }

    private void OnOptionsClosed(Node optionsInstance)
    {
        optionsInstance.QueueFree();
    }
}