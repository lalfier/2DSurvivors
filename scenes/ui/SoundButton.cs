using DSurvivors.scenes.component;
using Godot;

namespace DSurvivors.scenes.ui;

public partial class SoundButton : Button
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        GetNode<RandomStreamPlayerComponent>("RandomStreamPlayerComponent").PlayRandom();
    }
}