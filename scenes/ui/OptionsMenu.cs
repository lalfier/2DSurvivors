using DSurvivors.scenes.autoload;
using Godot;

namespace DSurvivors.scenes.ui;

public partial class OptionsMenu : CanvasLayer
{
    [Signal] public delegate void BackPressedEventHandler();
    
    private SoundButton _windowButton;
    private HSlider _sfxSlider;
    private HSlider _musicSlider;
    private SoundButton _backButton;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _windowButton = GetNode<SoundButton>("%WindowButton");
        _windowButton.Pressed += OnWindowButtonPressed;
        
        _sfxSlider = GetNode<HSlider>("%SfxSlider");
        _sfxSlider.ValueChanged += (value) => OnAudioSliderChanged(value, "Sfx");
        
        _musicSlider = GetNode<HSlider>("%MusicSlider");
        _musicSlider.ValueChanged += (value) => OnAudioSliderChanged(value, "Music");
        
        _backButton = GetNode<SoundButton>("%BackButton");
        _backButton.Pressed += OnBackButtonPressed;
        
        UpdateDisplay();
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause"))
        {
            OnBackButtonPressed();
            GetTree().Root.SetInputAsHandled();
        }
    }

    private void UpdateDisplay()
    {
        _windowButton.Text = "Windowed";
        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
        {
            _windowButton.Text = "Fullscreen";
        }

        _sfxSlider.Value = GetBusVolumePercent("Sfx");
        _musicSlider.Value = GetBusVolumePercent("Music");
    }

    private float GetBusVolumePercent(string busName)
    {
        var busIndex = AudioServer.GetBusIndex(busName);
        var volumeDb = AudioServer.GetBusVolumeDb(busIndex);
        return Mathf.DbToLinear(volumeDb);
    }
    
    private void SetBusVolumePercent(string busName, float percent)
    {
        var busIndex = AudioServer.GetBusIndex(busName);
        var volumeDb = Mathf.LinearToDb(percent);
        AudioServer.SetBusVolumeDb(busIndex, volumeDb);
    }

    private void OnWindowButtonPressed()
    {
        var mode = DisplayServer.WindowGetMode();
        if (mode != DisplayServer.WindowMode.Fullscreen)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
        else
        {
            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
        
        UpdateDisplay();
    }

    private void OnAudioSliderChanged(double value, string busName)
    {
        SetBusVolumePercent(busName, (float)value);
    }

    private async void OnBackButtonPressed()
    {
        GetNode<ScreenTransition>("/root/ScreenTransition").Transition();
        await ToSignal(GetNode<ScreenTransition>("/root/ScreenTransition"), "TransitionedHalfway");
        EmitSignal(SignalName.BackPressed);
    }
}