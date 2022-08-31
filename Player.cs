using System;
using System.Collections.Generic;
using Godot;

// csharp game example - https://github.com/Revolutionary-Games/Thrive

public class Player : KinematicBody2D
{

	float speed;
	Vector2 direction = new Vector2();
	AudioStreamOGGVorbis bumpSound;
	Sprite sprite;
	AnimationPlayer animationPlayer;
	AudioStreamPlayer2D audioPlayer;
	CollisionShape2D collisionShape2D;

	Sprite background;
	Image bumpMap;
	int[][] bumps;

	public override void _Ready()
	{

		bumpSound = ResourceLoader.Load<AudioStreamOGGVorbis>("bump2.ogg");
		bumpSound.Loop = false;
		sprite = (Sprite)FindNode("Sprite");
		animationPlayer = (AnimationPlayer)FindNode("AnimationPlayer");
		audioPlayer = GetNode<AudioStreamPlayer2D>("/root/Game/AudioStreamPlayer2D");
        collisionShape2D = (CollisionShape2D)FindNode("CollisionShape2D");

		// TODO: move somewhere
		background = GetNode<Sprite>("/root/Game/Background");
		bumpMap = background.Texture.GetData();
		//bumpMap = ResourceLoader.Load<Texture>("new-bark-town1-bump.png").GetData();
		bumpMap.Lock();
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
						sum += Math.Abs(color.r - color.g) + Math.Abs(color.g - color.b) + Math.Abs(color.b - color.r);

					}

				}
				sum = sum / 32f;
				bumps[k][l] = (int)sum;

			}

		}

	}

    public override void _Process(float delta)
    {

		if (Input.IsKeyPressed((int)Godot.KeyList.X)) speed = 1.5f;
		else speed = 1f;

		String animationName = "walk-";
        direction = new Vector2(0, 0);

		if (Input.IsKeyPressed((int)Godot.KeyList.Up))
		{
			animationName += "up";
			direction.y -= 1;
		}
		else if (Input.IsKeyPressed((int)Godot.KeyList.Down))
		{
			animationName += "down";
			direction.y += 1;
		}
		if (Input.IsKeyPressed((int)Godot.KeyList.Right))
		{
			//sprite.FlipH = false;
			animationName += "right";
			direction.x += 1;
		}
		else if (Input.IsKeyPressed((int)Godot.KeyList.Left))
		{
			//sprite.FlipH = false;
			animationName += "left";
			direction.x -= 1;
		}


		direction = direction.Normalized();

		if (direction != Vector2.Zero)
		{

			if (!animationPlayer.CurrentAnimation.Equals(animationName))
			{

				float position = animationPlayer.CurrentAnimationPosition;
				animationPlayer.Stop(true);
				animationPlayer.Play(animationName);
				// animationPlayer.Seek(position % animationPlayer.CurrentAnimationLength); // won't work due to hflip

			}
			if (!animationPlayer.PlaybackSpeed.Equals(speed))
            {

				animationPlayer.PlaybackSpeed = speed;

			}

			Vector2 moveAmount = MoveAndSlide(direction * speed * 80);
			KinematicCollision2D collision = GetLastSlideCollision();
			if (collision != null && !audioPlayer.Playing) {

				speed = 1;
				audioPlayer.Stream = bumpSound;
				audioPlayer.Play();

			}

			Vector2 newPos = new Vector2(Position.x, Position.y) - background.Position;
			int xPos = (int)newPos.x;
			int yPos = (int)newPos.y;
			if (0 < xPos && xPos < bumps.Length && 0 < yPos && yPos < bumps[xPos].Length)
            {

				int offsetY = bumps[xPos][yPos];
				this.sprite.Position = new Vector2(0, -offsetY);

			}

		}
		else
        {

			if (animationPlayer.IsPlaying()) animationPlayer.Seek(0, true);
			animationPlayer.Stop(true);

		}

	}

	public void OnBodyEntered()
    {


    }

}
