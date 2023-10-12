using DSurvivors.scenes.manager;
using Godot;

namespace DSurvivors.scenes.ui;

public partial class ArenaTimeUI : CanvasLayer
{
	[Export] private ArenaTimeManager _arenaTimeManager;

	private Label _timeLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timeLabel = GetNode<Label>("%TimeLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_arenaTimeManager == null)
		{
			return;
		}

		var timeElapsed = _arenaTimeManager.GetTimeElapsed();
		_timeLabel.Text = FormatSecondsToString(timeElapsed);
	}

	private string FormatSecondsToString(double seconds)
	{
		var minutes = Mathf.Floor(seconds / 60);
		var remainingSeconds = seconds - (minutes * 60);
		return minutes + ":" + Mathf.Floor(remainingSeconds).ToString("00");
	}
}