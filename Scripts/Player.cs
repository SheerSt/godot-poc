using System;
using System.Collections.Generic;
using Godot;

/** 
 * References:
 * 
 * csharp game example - https://github.com/Revolutionary-Games/Thrive
 * Godot csharp documentation - https://cyoann.github.io/GodotSharpAPI/
 *  - Note: Godot csharp api is usually the exact same as gdscript, except it uses CamelCase instead of underscore_case.
 * 
 */
public partial class Player : CharacterBody2D
{

	String animationName;
	String previousDirectionName = "down";
	float speed;
	Vector2 direction = new Vector2();
	AudioStreamOggVorbis bumpSound;
	AudioStreamOggVorbis walkSound;
	Sprite2D sprite;
	AnimationPlayer animationPlayer;
	AudioStreamPlayer2D bumpSoundPlayer;
	CollisionShape2D collisionShape2D;
	Camera2D camera2D;

	Sprite2D background;
	Image bumpMap;
	int[][] bumps;
	public int yOffset;

	double footprintTimer = 30;
	bool justPressedX = true;
	//float grassOverlayTimer = 0;

	public override void _Ready()
	{

		bumpSound = ResourceLoader.Load<AudioStreamOggVorbis>("Sounds/bump2.ogg");
		bumpSound.Loop = false;
		walkSound = ResourceLoader.Load<AudioStreamOggVorbis>("Sounds/walk1.ogg");
		walkSound.Loop = false;

		sprite = (Sprite2D)FindChild("Sprite2D");
		animationPlayer = (AnimationPlayer)FindChild("AnimationPlayer");
		bumpSoundPlayer = GetNode<AudioStreamPlayer2D>("/root/Game/Map/Trees/Player/BumpSoundPlayer");
		collisionShape2D = (CollisionShape2D)FindChild("CollisionShape2D");
		camera2D = (Camera2D)FindChild("Camera2D");

		// TODO: might need to get this working for a tileset.
		// TODO: move to a method
		background = GetNode<Sprite2D>("/root/Game/Map/Background");
		bumpMap = background.Texture.GetImage();
		//bumpMap.Lock();  // TODO: test / remove
		int width = bumpMap.GetWidth();
		int height = bumpMap.GetHeight();
		bumps = new int[width][];
		// Get bump map offsets.
		float sum = 0f;
		for (int k = 0; k < width; ++k)
		{

			bumps[k] = new int[height];
			for (int l = 0; l < height; ++l)
			{

				for (int i = -4; i < 8; ++i)
				{
					int xPos = k + i;
					if (xPos < 0 || xPos >= width) continue;

					for (int j = -4; j < 8; ++j)
					{
						int yPos = l + j;
						if (yPos < 0 || yPos >= height) continue;

						Color color = bumpMap.GetPixel(xPos, yPos);
						sum += Math.Abs(color.R - color.G) + Math.Abs(color.G - color.B) + Math.Abs(color.B - color.R);

					}

				}

				// sum = sum / 32f;  // Looked good.
				// sum = sum / 64f;  // Looked good but very subtle.
				sum = sum / 52f;
				bumps[k][l] = (int)sum;

			}

		}

		// Offset sprite based on terrain.
		this.yOffset = OffsetSprite();

	}

	/**
	 * Offset sprite based on terrain.
	 */
	public int OffsetSprite()
	{

		Vector2 newPos = new Vector2(Position.X, Position.Y) - background.Position;
		int xPos = (int)newPos.X;
		int yPos = (int)newPos.Y;
		int offsetY = 0;
		if (0 < xPos && xPos < bumps.Length && 0 < yPos && yPos < bumps[xPos].Length)
		{

			offsetY = bumps[xPos][yPos];
			this.sprite.Position = new Vector2(0,  -2 - offsetY);
			this.collisionShape2D.Position = new Vector2(0, 3 - offsetY); ;

		}
		return offsetY;

	}

	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Godot.Key.X))
		{
			animationName = "run-";
			speed = 1.5f;

			if (Game.instance.debugMode && Input.IsKeyPressed(Godot.Key.Space)) speed = 10f;

			// Camera3D smoothing with increased catch-up time when player sprints.
			/* TODO: This eventually causes the camera to destabilize
			if (justPressedX)
			{
				justPressedX = false;
				camera2D.FollowSmoothingEnabled = true;
				camera2D.FollowSmoothingSpeed = 8f;
			}
			else if (!camera2D.GetCameraScreenCenter().IsEqualApprox(camera2D.GetCameraPosition()))
				camera2D.FollowSmoothingSpeed += 0.2f;
			else
				camera2D.FollowSmoothingEnabled = false;
				*/

		}
		else
		{
			animationName = "walk-";
			speed = 1f;

			//
			if (!justPressedX)
				justPressedX = true;
			if (!camera2D.GetScreenCenterPosition().IsEqualApprox(camera2D.GlobalPosition))
				camera2D.PositionSmoothingSpeed += 0.4f;
			else
				camera2D.PositionSmoothingEnabled = false;
		}

		//GD.Print(camera2D.FollowSmoothingEnabled);
		//GD.Print(camera2D.FollowSmoothingSpeed);
		//GD.Print(camera2D.GetCameraScreenCenter());
		//GD.Print(camera2D.GetCameraPosition());

		direction = new Vector2(0, 0);

		String directionName = "";
		if (Input.IsKeyPressed(Godot.Key.Up))
		{
			directionName += "up";
			direction.Y -= 1;
		}
		else if (Input.IsKeyPressed(Godot.Key.Down))
		{
			directionName += "down";
			direction.Y += 1;
		}
		if (Input.IsKeyPressed(Godot.Key.Right))
		{
			directionName += "right";
			direction.X += 1;
		}
		else if (Input.IsKeyPressed(Godot.Key.Left))
		{
			directionName += "left";
			direction.X -= 1;
		}

		Vector2 directionNor = direction.Normalized();

		if (direction != Vector2.Zero)
		{

			previousDirectionName = directionName;
			animationName += previousDirectionName;

			if (!animationPlayer.CurrentAnimation.Equals(animationName))
			{

				double position = animationPlayer.CurrentAnimationPosition;
				animationPlayer.Stop(true);
				animationPlayer.Play(animationName);

			}
			if (!animationPlayer.SpeedScale.Equals(speed))
			{

				animationPlayer.SpeedScale = speed;

			}

			Velocity = directionNor * speed * 65;
			MoveAndSlide();

			KinematicCollision2D collision = GetLastSlideCollision();
			if (collision != null && !bumpSoundPlayer.Playing)
			{

				speed = 1;
				bumpSoundPlayer.Stream = bumpSound;
				bumpSoundPlayer.Play();

			}


			// Walk sound
			AudioStreamPlayer player = (AudioStreamPlayer)FindChild("Sounds");
			if (!player.Playing)
			{

				AudioStreamOggVorbis sound;
				if (speed > 1) sound = ResourceLoader.Load<AudioStreamOggVorbis>("Sounds/run1.ogg");
				else sound = ResourceLoader.Load<AudioStreamOggVorbis>("Sounds/walk2.ogg");
				sound.Loop = false;
				player.Stream = sound;
				player.Play();

			}

			// Offset sprite based on terrain.
			this.yOffset = OffsetSprite();  // TODO: uncomment
			this.yOffset = 0;  // TODO: remove

			// Check if it's time to have the player leave a footprint.
			// Footprint is a Node with an Animation that plays. The Animation deletes the Node when it's done.
			footprintTimer += delta * speed;
			if (footprintTimer > .3f)
			{

				footprintTimer = 0;
				PackedScene footPrints = GD.Load<PackedScene>("res://Scenes/Footprints.tscn");
				Node node = footPrints.Instantiate();
				Sprite2D sprite = node.GetChild<Sprite2D>(0);
				sprite.Position = Position.Floor() + new Vector2(0, 2 - yOffset);
				yOffset += 2;

				if (direction == new Vector2(1, 0))
				{
					sprite.Frame = 0;
				}
				else if (direction == new Vector2(-1, 0))
				{
					sprite.Frame = 0;
					sprite.FlipH = true;
				}
				else if (direction == new Vector2(0, 1))
				{
					sprite.Frame = 1;
				}
				else if (direction == new Vector2(0, -1))
				{
					sprite.Frame = 1;
					sprite.FlipV = true;
				}
				else if (direction == new Vector2(1, -1))
				{
					sprite.Frame = 2;
				}
				else if (direction == new Vector2(-1, -1))
				{
					sprite.Frame = 2;
					sprite.FlipH = true;
				}
				else if (direction == new Vector2(1, 1))
				{
					sprite.Frame = 3;
				}
				else if (direction == new Vector2(-1, 1))
				{
					sprite.Frame = 3;
					sprite.FlipH = true;
				}

				AnimationPlayer animationPlayer = node.GetChild<AnimationPlayer>(1);

				float a = ((float)yOffset / 12f);
				// yOffset >= 4 means the player is in a grassy area.
				if (yOffset >= 4)
				{
					animationPlayer.CurrentAnimation = "footprints-fade-grass";
					a = .5f;
				}
				else animationPlayer.CurrentAnimation = "footprints-fade-sand";
				animationPlayer.Play();

				sprite.Modulate = new Color(1f, 1f, 1f, a);

				Node2D map = (Node2D)Game.instance.FindChild("Map");
				TileMap tileMap = (TileMap)map.FindChild("TileMap");
				tileMap.AddSibling(node);

			}

		}
		else
		{

			footprintTimer = 30;

			// Restart the idle animation, since player isn't moving.
			// Idle animation is just the player blinking occasionally.
			animationName = "idle-";
			animationName += previousDirectionName;
			animationPlayer.Play(animationName);

		}


	}

}
