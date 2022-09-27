using Godot;
using System;

public class GrassOverlay : Sprite
{

	Player player;
	Sprite playerSprite;
	int prevFrame;
	float timer = 0f;

	public override void _Ready()
	{
		player = (Player)GetParent();
		playerSprite = (Sprite)player.FindNode("Sprite");
	}

	public override void _Process(float delta)
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

		// Copy the player Sprite's frame number.
		Frame = playerSprite.Frame;

		// Copy the player Sprite's horizontal flip.
		FlipH = playerSprite.FlipH;

	}
}
