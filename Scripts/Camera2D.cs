using Godot;
using System;

public partial class Camera2D : Godot.Camera2D
{

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (Input.IsKeyPressed(Godot.Key.E) && this.Zoom > new Vector2(1, 1))
		{
			this.Zoom -= new Vector2(0.2f, 0.2f);
		}
		else if (Input.IsKeyPressed(Godot.Key.Q) && this.Zoom < new Vector2(5, 5))
		{
			this.Zoom += new Vector2(0.2f, 0.2f);
		}

	}

}
