using DSurvivors.scenes.autoload;
using DSurvivors.scenes.ui;
using Godot;

namespace DSurvivors.scenes.manager;

public partial class ArenaTimeManager : Node
{
    private const float DIFFICULTY_INTERVAL = 5;
    
    [Signal] public delegate void ArenaDifficultyIncreasedEventHandler(int arenaDifficulty);
    
    [Export] private PackedScene _endScreenScene;

    private Timer _timer;
    private int _arenaDifficulty = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += OnTimerTimeout;
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var nextTimeTarget = _timer.WaitTime - ((_arenaDifficulty + 1) * DIFFICULTY_INTERVAL);
        if (_timer.TimeLeft <= nextTimeTarget)
        {
            _arenaDifficulty += 1;
            EmitSignal(SignalName.ArenaDifficultyIncreased, _arenaDifficulty);
        }
    }

    public double GetTimeElapsed()
    {
        return _timer.WaitTime - _timer.TimeLeft;
    }

    private void OnTimerTimeout()
    {
        var endScreenInstance = (EndScreen)_endScreenScene.Instantiate();
        AddChild(endScreenInstance);
        endScreenInstance.PlayJingle();
        GetNode<MetaProgression>("/root/MetaProgression").SaveToFile();
    }
}