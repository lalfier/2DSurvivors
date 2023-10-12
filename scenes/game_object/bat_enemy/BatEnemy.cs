using DSurvivors.scenes.component;
using Godot;

namespace DSurvivors.scenes.game_object.bat_enemy;

public partial class BatEnemy : CharacterBody2D
{
    private Node2D _visuals;
    private VelocityComponent _velocityComponent;
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _visuals = GetNode<Node2D>("Visuals");
        _velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
        
        GetNode<HurtboxComponent>("HurtboxComponent").Hit += OnHit;
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _velocityComponent.AccelerateToPlayer();
        _velocityComponent.Move(this);
		
        var moveSign = Mathf.Sign(Velocity.X);
        if (moveSign != 0)
        {
            _visuals.Scale = new Vector2(moveSign, 1);
        }
    }
    
    private void OnHit()
    {
        GetNode<RandomStreamPlayer2DComponent>("HitRandomAudioPlayerComponent").PlayRandom();
    }
}