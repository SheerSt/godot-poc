using ExtensionMethods;
using Godot;
using System;

public class Monster : KinematicBody2D
{

	Direction direction = Direction.NONE;
	Timer moveTimer;
	Timer dirtTimer;
	float speed = 1f;
	float walkingTimer = 0f;
	Sprite sprite;
	AnimationPlayer animationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		moveTimer = (Timer)FindNode("MoveTimer");
		dirtTimer = (Timer)FindNode("DirtTimer");
		sprite = (Sprite)FindNode("Sprite");
		animationPlayer = (AnimationPlayer)FindNode("AnimationPlayer");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
			this.dirtTimer.Paused = true;
		}

	}

	private void _on_DirtTimer_timeout()
	{

		//
		PackedScene footPrints = GD.Load<PackedScene>("res://Monsters/Dirt-Trail.tscn");
		Node node = footPrints.Instance();
		Sprite sprite = (Sprite)node.FindNode("Sprite");
		sprite.Position = Position.Floor() + new Vector2(0, 2);  // y = 8 - yOffset
		AnimationPlayer animationPlayer = (AnimationPlayer)node.FindNode("AnimationPlayer");
		animationPlayer.CurrentAnimation = "footprints-fade-sand";
		animationPlayer.Play();

		//
		Node2D map = (Node2D)Game.instance.FindNode("Map");
		TileMap tileMap = (TileMap)map.FindNode("TileMap");
		map.AddChildBelowNode(tileMap, node);


		//
		PackedScene dirt = GD.Load<PackedScene>("res://Monsters/debris1.tscn");
		node = dirt.Instance();
		//((Node2D)node).Position = Position.Floor() + new Vector2(0, 16);  // y = 8 - yOffset
		((Node2D)node).Position = new Vector2(0, 4);
		animationPlayer = (AnimationPlayer)node.FindNode("AnimationPlayer");
		animationPlayer.CurrentAnimation = "Anim1";
		animationPlayer.Play();
		//tileMap = (TileMap)map.FindNode("Trees");
		this.AddChild(node);

	}

	private void _on_MoveTimer_timeout()
	{

		//
		dirtTimer.Paused = false;
		dirtTimer.Start();

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
