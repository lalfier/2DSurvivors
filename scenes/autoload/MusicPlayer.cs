using Godot;

namespace DSurvivors.scenes.autoload;

public partial class MusicPlayer : AudioStreamPlayer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Finished += OnFinished;
        GetNode<Timer>("Timer").Timeout += OnTimerTimeout;
    }

    private void OnFinished()
    {
        GetNode<Timer>("Timer").Start();
    }

    private void OnTimerTimeout()
    {
        Play();
    }
}