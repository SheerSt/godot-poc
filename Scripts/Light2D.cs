using Godot;
using System;

public class Light2D : Godot.Light2D
{

    float timer = 0f;
    int region = 0;

    public override void _Ready()
    {
        (Texture as AtlasTexture).Region = new Rect2(0, 0, 16, 16);
    }

    public override void _Process(float delta)
    {
        timer += delta;

        if (timer >= 1)
        {
            // There is a Godot bug preventing this from working.
            // https://github.com/godotengine/godot/issues/43268#issuecomment-720571636
            timer -= 1f;
            region = (region + 1) % 2;
            (Texture as AtlasTexture).Region = new Rect2(region * 16, 0, 16, 16);
            //GD.Print((Texture as AtlasTexture).Region);

        }

    }

}
