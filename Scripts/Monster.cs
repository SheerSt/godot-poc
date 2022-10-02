using ExtensionMethods;
using Godot;
using System;

public class Monster : KinematicBody2D
{

	public Direction direction = Direction.NONE;
	public Timer moveTimer;
	public float speed = 1f;
	public float walkingTimer = 0f;
	public Sprite sprite;
	public AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		
		moveTimer = (Timer)FindNode("MoveTimer");
		sprite = (Sprite)FindNode("Sprite");
		animationPlayer = (AnimationPlayer)FindNode("AnimationPlayer");

	}
	
	public override void _Process(float delta)
	{

		if (walkingTimer > 0)
		{
			walkingTimer -= delta;
			Vector2 moveAmount = MoveAndSlide(this.direction.getVector().Normalized() * this.speed * 36);

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

		walkingTimer = 1f;

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
		this.sprite.FrameCoords = new Vector2(0, offsetY);

	}

}
