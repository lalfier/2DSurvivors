using DSurvivors.scenes.autoload;
using Godot;

namespace DSurvivors.scenes.manager;

public partial class ExperienceManager : Node
{
    private const int TARGET_EXPERIENCE_GROWTH = 5;
    
    [Signal] public delegate void ExperienceUpdatedEventHandler(float currentExperience, float targetExperience);
    [Signal] public delegate void LevelUpEventHandler(int newLevel);
    
    private float _currentExperience = 0;
    private int _currentLevel = 1;
    private float _targetExperience = 5;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<GameEvents>("/root/GameEvents").ExperienceVialCollected += OnExperienceVialCollected;
    }
    
    public override void _ExitTree()
    {
        GetNode<GameEvents>("/root/GameEvents").ExperienceVialCollected -= OnExperienceVialCollected;
    }
    
    private void OnExperienceVialCollected(float number)
    {
        IncrementExperience(number);
    }

    private void IncrementExperience(float number)
    {
        _currentExperience = Mathf.Min(_currentExperience + number, _targetExperience);
        if (_currentExperience >= _targetExperience)
        {
            _currentLevel += 1;
            _targetExperience += TARGET_EXPERIENCE_GROWTH;
            _currentExperience = 0;
            EmitSignal(SignalName.LevelUp, _currentLevel);
        }
        
        EmitSignal(SignalName.ExperienceUpdated, _currentExperience, _targetExperience);
    }
}