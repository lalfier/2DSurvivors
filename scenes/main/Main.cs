using DSurvivors.scenes.autoload;
using DSurvivors.scenes.game_object.player;
using DSurvivors.scenes.ui;
using Godot;

namespace DSurvivors.scenes.main;

public partial class Main : Node
{
    [Export] private PackedScene _endScreenScene;
    [Export] private PackedScene _pauseMenuScene;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<Player>("%Player").HealthComponent.Died += OnPlayerDied;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            AddChild(_pauseMenuScene.Instantiate());
            GetTree().Root.SetInputAsHandled();
        }
    }

    private void OnPlayerDied()
    {
        var endScreenInstance = _endScreenScene.Instantiate() as EndScreen;
        if (endScreenInstance == null)
        {
            return;
        }
        
        AddChild(endScreenInstance);
        endScreenInstance.SetDefeat();
        GetNode<MetaProgression>("/root/MetaProgression").SaveToFile();
    }
}