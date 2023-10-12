using Godot;
using Godot.Collections;

namespace DSurvivors.scenes.component;

public partial class RandomStreamPlayerComponent : AudioStreamPlayer
{
    [Export] private Array<AudioStream> _streams = new Array<AudioStream>();
    [Export] private bool _randomizePitch = true;
    [Export] private float _minPitch = 0.9f;
    [Export] private float _maxPitch = 1.1f;

    public void PlayRandom()
    {
        if (_streams == null || _streams.Count <= 0)
        {
            return;
        }

        if (_randomizePitch)
        {
            PitchScale = (float)GD.RandRange(_minPitch, _maxPitch);
        }
        else
        {
            PitchScale = 1;
        }
        
        Stream = _streams.PickRandom();
        Play();
    }
}