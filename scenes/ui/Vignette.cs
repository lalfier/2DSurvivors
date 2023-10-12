using DSurvivors.scenes.autoload;
using Godot;

namespace DSurvivors.scenes.ui;

public partial class Vignette : CanvasLayer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("RESET");
        GetNode<GameEvents>("/root/GameEvents").PlayerDamaged += OnPlayerDamaged;
    }
    
    public override void _ExitTree()
    {
        GetNode<GameEvents>("/root/GameEvents").PlayerDamaged -= OnPlayerDamaged;
    }

    private void OnPlayerDamaged()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("hit");
    }
}