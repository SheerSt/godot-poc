using ExtensionMethods;
using Godot;
using System;

public partial class Monster : CharacterBody2D
{

    public Direction direction = Direction.NONE;
    public float speed = 1f;
    public Sprite2D sprite;
    public AnimationPlayer animationPlayer;
    public Timer moveTimer;
    public Timer walkingTimer;

    public override void _Ready()
    {
        
        moveTimer = (Timer)FindChild("MoveTimer");
        walkingTimer = (Timer)FindChild("WalkingTimer");
        sprite = (Sprite2D)FindChild("Sprite2D");
        animationPlayer = (AnimationPlayer)FindChild("AnimationPlayer");

    }
    
    public override void _Process(double delta)
    {

        if (!this.walkingTimer.IsStopped())
        {

            Velocity = this.direction.getVector().Normalized() * this.speed * 36;
            MoveAndSlide();

            // Snap the sprite's position to whole numbers, otherwise it looks weird.
            // There's likely a better way to do this.
            sprite.Position = Position.Floor() - Position;

        }
        else if (this.moveTimer.IsStopped())
        {
            this.moveTimer.Start();
            animationPlayer.Stop();
        }

    }

    public void _on_MoveTimer_timeout()
    {
        walkingTimer.Start();

        // Get a random direction;
        this.direction = DirectionExtensions.all[Game.random.Next(0, DirectionExtensions.all.Length)];
        animationPlayer.CurrentAnimation = "Move";
        animationPlayer.Play();

        // Set sprite direction based this.direction
        this.sprite.FlipH = false;
        Vector2 directionVector = this.direction.getVector();
        int offsetY = 0;
        if (directionVector.x > 0)
            this.sprite.FlipH = true;
        if (directionVector.y < 0)
            offsetY = 1;
        else if (directionVector.y > 0)
            offsetY = 2;
        this.sprite.FrameCoords = new Vector2i(0, offsetY);

    }

}
