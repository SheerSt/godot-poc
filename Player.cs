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
    public int yOffset;

    float footprintTimer = 30;
    //float grassOverlayTimer = 0;

    public override void _Ready()
    {

        bumpSound = ResourceLoader.Load<AudioStreamOGGVorbis>("Sounds/bump2.ogg");
        bumpSound.Loop = false;
        walkSound = ResourceLoader.Load<AudioStreamOGGVorbis>("Sounds/walk1.ogg");
        walkSound.Loop = false;

        sprite = (Sprite)FindNode("Sprite");
        animationPlayer = (AnimationPlayer)FindNode("AnimationPlayer");
        audioPlayer = GetNode<AudioStreamPlayer2D>("/root/Game/AudioStreamPlayer2D");
        collisionShape2D = (CollisionShape2D)FindNode("CollisionShape2D");

        // TODO: move to a method
        background = GetNode<Sprite>("/root/Game/Background");
        bumpMap = background.Texture.GetData();
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

        Vector2 newPos = new Vector2(Position.x, Position.y) - background.Position;
        int xPos = (int)newPos.x;
        int yPos = (int)newPos.y;
        int offsetY = 0;
        if (0 < xPos && xPos < bumps.Length && 0 < yPos && yPos < bumps[xPos].Length)
        {

            offsetY = bumps[xPos][yPos];
            this.sprite.Position = new Vector2(0, 4 - offsetY);
            this.collisionShape2D.Position = new Vector2(0, 10 - offsetY); ;

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
                audioPlayer.Play();

            }


            // Walk sound
            AudioStreamPlayer player = (AudioStreamPlayer)FindNode("Sounds");
            if (!player.Playing)
            {

                AudioStreamOGGVorbis sound;
                if (speed > 1) sound = ResourceLoader.Load<AudioStreamOGGVorbis>("Sounds/run1.ogg");
                else sound = ResourceLoader.Load<AudioStreamOGGVorbis>("Sounds/walk2.ogg");
                sound.Loop = false;
                player.Stream = sound;
                player.Play();

            }

            // Offset sprite based on terrain.
            this.yOffset = OffsetSprite();

            // Check if it's time to have the player leave a footprint.
            // Footprint is a Node with an Animation that plays. The Animation deletes the Node when it's done.
            footprintTimer += delta * speed;
            if (footprintTimer > .3f)
            {

                footprintTimer = 0;
                PackedScene footPrints = GD.Load<PackedScene>("res://footprints.tscn");
                Node node = footPrints.Instance();
                Sprite sprite = node.GetChild<Sprite>(0);
                sprite.Position = Position.Floor() + new Vector2(0, 8 - yOffset);
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

                TileMap tileMap = (TileMap)Game.instance.FindNode("TileMap");

                Game.instance.AddChildBelowNode(tileMap, node);

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
