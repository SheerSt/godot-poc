using Godot;
using System;

public partial class GrassOverlay : Sprite2D
{

    Player player;
    Sprite2D playerSprite;
    int prevFrame;
    double timer = 0f;

    public override void _Ready()
    {
        player = (Player)GetParent();
        playerSprite = (Sprite2D)player.FindChild("Sprite2D");
    }

    public override void _Process(double delta)
    {

        if (player.yOffset < 3)
        {
            this.Visible = false;
            this.timer = 0f;
            this.prevFrame = -1;
            return;

        }

        if (this.prevFrame == playerSprite.Frame)
        {

            if (this.timer > .1f)
            {

                this.Visible = false;
                return;
            }
            this.timer += delta;

        }
        else
        {
            this.timer = 0f;
            this.prevFrame = playerSprite.Frame;
        }

        this.Visible = true;

        // Align to Player sprite.
        Position = playerSprite.Position;

        // Copy the player Sprite2D's frame number.
        Frame = playerSprite.Frame;

        // Copy the player Sprite2D's horizontal flip.
        FlipH = playerSprite.FlipH;

    }
}
