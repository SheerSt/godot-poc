using Godot;
using System;

public partial class Camera2D : Godot.Camera2D
{

	public override void _Process(double delta)
	{

		if (Input.IsKeyPressed(Godot.Key.E) && this.Zoom < new Vector2(1, 1))
		{
			this.Zoom += new Vector2(0.1f, 0.1f);
		}
		else if (Input.IsKeyPressed(Godot.Key.Q) && this.Zoom > new Vector2(0.5f, 0.5f))
		{
			this.Zoom -= new Vector2(0.1f, 0.1f);
		}

	}

}
