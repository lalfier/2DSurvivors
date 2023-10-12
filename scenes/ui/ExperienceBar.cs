using DSurvivors.scenes.manager;
using Godot;

namespace DSurvivors.scenes.ui;

public partial class ExperienceBar : CanvasLayer
{
    [Export] private ExperienceManager _experienceManager;

    private ProgressBar _progressBar;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _progressBar = GetNode<ProgressBar>("MarginContainer/ProgressBar");
        _progressBar.Value = 0;
        
        _experienceManager.ExperienceUpdated += OnExperienceUpdated;
    }

    private void OnExperienceUpdated(float currentExperience, float targetExperience)
    {
        var percent = currentExperience / targetExperience;
        _progressBar.Value = percent;
    }
}