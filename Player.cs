using System;
using System.Collections.Generic;
using Godot;

// csharp game example - https://github.com/Revolutionary-Games/Thrive

public class Player : KinematicBody2D
{

	String animationName;
	String previousDirectionName = "down";
	float speed;
	Vector2 direction = new Vector2();
	AudioStreamOGGVorbis bumpSound;
	AudioStreamOGGVorbis walkSound;
	Sprite sprite;
	AnimationPlayer animationPlayer;
	AudioStreamPlayer2D audioPlayer;
	CollisionShape2D collisionShape2D;

	Sprite background;
	Image bumpMap;
	int[][] bumps;

	float footprintTimer = 30;

	public override void _Ready()
	{

		bumpSound = ResourceLoader.Load<AudioStreamOGGVorbis>("bump2.ogg");
		bumpSound.Loop = false;
		walkSound = ResourceLoader.Load<AudioStreamOGGVorbis>("walk1.ogg");
		walkSound.Loop = false;

		sprite = (Sprite)FindNode("Sprite");
		animationPlayer = (AnimationPlayer)FindNode("AnimationPlayer");
		audioPlayer = GetNode<AudioStreamPlayer2D>("/root/Game/AudioStreamPlayer2D");
        //collisionShape2D = (CollisionShape2D)FindNode("CollisionShape2D");

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

		// Offset sprite based on terrain.
		OffsetSprite();

	}

    /**
	 * Offset sprite based on terrain.
	 */
    public int OffsetSprite()
    {

		Vector2 newPos = new Vector2(Position.x, Position.y) - background.Position;
		int xPos = (int)newPos.x;
		int yPos = (int)newPos.y;
		int offsetY = 0;
		if (0 < xPos && xPos < bumps.Length && 0 < yPos && yPos < bumps[xPos].Length)
		{

			offsetY = bumps[xPos][yPos];
			this.sprite.Position = new Vector2(0, 4 - offsetY);

		}
		return offsetY;

	}

    public override void _Process(float delta)
    {
		if (Input.IsKeyPressed((int)Godot.KeyList.X))
		{

			animationName = "run-";
			speed = 1.5f;

		}
		else
		{
			animationName = "walk-";
			speed = 1f;
		}

        direction = new Vector2(0, 0);

		String directionName = "";
		if (Input.IsKeyPressed((int)Godot.KeyList.Up))
		{
			directionName += "up";
			direction.y -= 1;
		}
		else if (Input.IsKeyPressed((int)Godot.KeyList.Down))
		{
			directionName += "down";
			direction.y += 1;
		}
		if (Input.IsKeyPressed((int)Godot.KeyList.Right))
		{
			directionName += "right";
			direction.x += 1;
		}
		else if (Input.IsKeyPressed((int)Godot.KeyList.Left))
		{
			directionName += "left";
			direction.x -= 1;
		}

		Vector2 directionNor = direction.Normalized();

		if (direction != Vector2.Zero)
		{

			previousDirectionName = directionName;
			animationName += previousDirectionName;

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

			Vector2 moveAmount = MoveAndSlide(directionNor * speed * 65);

			KinematicCollision2D collision = GetLastSlideCollision();
            if (collision != null && !audioPlayer.Playing)
            {

                speed = 1;
                audioPlayer.Stream = bumpSound;
                //audioPlayer.VolumeDb = 0.5f;
                audioPlayer.Play();

            }


			// Walk sound
			AudioStreamPlayer player = (AudioStreamPlayer)FindNode("Sounds");
			if (!player.Playing)
			{

				//audioPlayer.Stream = walkSound;
				//audioPlayer.Play();

				AudioStreamOGGVorbis sound;
				if (speed > 1) sound = ResourceLoader.Load<AudioStreamOGGVorbis>("run1.ogg");
				else sound = ResourceLoader.Load<AudioStreamOGGVorbis>("walk2.ogg");
				sound.Loop = false;
				player.Stream = sound;
				player.Play();


			}

			// Offset sprite based on terrain.
			int yOffset = OffsetSprite();

			footprintTimer += delta * speed;
			if (footprintTimer > .3f)
            {

				footprintTimer = 0;
				PackedScene footPrints = GD.Load<PackedScene>("res://footprints.tscn");
				Node node = footPrints.Instance();
				Sprite sprite = node.GetChild<Sprite>(0);
				sprite.Position = Position.Floor() + new Vector2(0, 8 - yOffset);
				yOffset += 2;
				float a = ((float)yOffset / 12f);
				sprite.Modulate = new Color(1f, 1f, 1f, a);

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
				animationPlayer.CurrentAnimation = "footprints-fade";
				animationPlayer.Play();

				TileMap tileMap = (TileMap)Game.instance.FindNode("TileMap");

				Game.instance.AddChildBelowNode(tileMap, node);
				//Game.instance.AddChildBelowNode(this.GetParent(), node);
				//Game.instance.MoveChild(node, 0);

			}

		}
		else
		{

			// Doesn't work.
			// Round sprite position to nearest pixel (prevents camera weirdness).
			//Vector2 offset = (Position - Position.Floor());
			//sprite.Position= new Vector2(-offset.x, -offset.y + 4);

			footprintTimer = 30;

			// Test
			animationName = "idle-";
			animationName += previousDirectionName;
			animationPlayer.Play(animationName);

			//
			//if (animationPlayer.IsPlaying()) animationPlayer.Seek(0, true);
			//animationPlayer.Stop(true);

		}


	}

	public void OnBodyEntered()
    {


    }

}
