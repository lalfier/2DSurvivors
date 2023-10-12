using Godot;

namespace DSurvivors.scenes.autoload;

public partial class ScreenTransition : CanvasLayer
{
    [Signal] public delegate void TransitionedHalfwayEventHandler();

    private bool _skipEmit = false;

    public async void Transition()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("fade_in_out");
        await ToSignal(this, "TransitionedHalfway");
        _skipEmit = true;
        GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("fade_in_out");
    }

    public async void TransitionToScene(string scenePath, bool unPause = false)
    {
        Transition();
        await ToSignal(this, "TransitionedHalfway");
        if (unPause)
        {
            GetTree().Paused = false;
        }
        GetTree().ChangeSceneToFile(scenePath);
    }

    private void EmitTransitionedHalfway()
    {
        if (_skipEmit)
        {
            _skipEmit = false;
            return;
        }
        
        EmitSignal(SignalName.TransitionedHalfway);
    }
}