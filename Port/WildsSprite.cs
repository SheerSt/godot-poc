using Godot;
using System;

public class WildsSprite : Sprite
{


    public WildsSprite(Node action, Texture texture)
    {
        this.Visible = false;
        this.RegionEnabled = true;
        this.Texture = texture;
        this.Name = texture.ResourcePath;

        // Puts this sprite component in the ActionStack.
        action.AddChild(this);

    }


    public WildsSprite(Node action, Texture texture, int x, int y, int width, int height) : this(action, texture)
    {

        Rect2 rectangle = new Rect2(x, y, width, height);
        this.RegionRect = rectangle;

    }

    public override void _Process(float delta)
    {
        // Set this to inactive each frame (wait for draw())
        this.Visible = false;

    }

    /**
    * Ported method from libgdx.
    */
    public void draw()
    {

        this.Visible = true;

    }


}
