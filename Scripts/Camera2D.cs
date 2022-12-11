using Godot;
using System;

public class Camera2D : Godot.Camera2D
{

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

        if (Input.IsKeyPressed((int)Godot.KeyList.E) && this.Zoom > new Vector2(1, 1))
        {
            this.Zoom -= new Vector2(0.2f, 0.2f);
        }
        else if (Input.IsKeyPressed((int)Godot.KeyList.Q) && this.Zoom < new Vector2(5, 5))
        {
            this.Zoom += new Vector2(0.2f, 0.2f);
        }

    }

}
