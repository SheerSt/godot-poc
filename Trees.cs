using Godot;
using System;

public class Trees : TileMap
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		TileMap shadowTileMap = GetNode<TileMap>("/root/Game/Shadows");

		// For each tile
		foreach (Vector2 position in GetUsedCells())
		{

			// Make a new Sprite that copies the tile texture
			int index = GetCellv(position);
			Texture texture = this.TileSet.TileGetTexture(index);
			Sprite sprite = new Sprite();
			sprite.Texture = texture;
			sprite.Position = MapToWorld(position);
			sprite.RegionEnabled = true;
			sprite.RegionRect = this.TileSet.TileGetRegion(index);

			//GD.Print(sprite.RegionRect);

			// Color sprite to be black, with slight transparency.
			//sprite.Modulate = new Color(0f, 0f, 0f, 1f);

			// Rotate 125 degrees, and squash it.
			//sprite.Rotation = Mathf.Deg2Rad(180);
			sprite.Scale = new Vector2(1f, 1f);
			sprite.FlipV = true;

			// Add the sprite as a child to Shadows TileMap
			shadowTileMap.AddChild(sprite);


		}



	}

	public override void _Process(float delta)
	{

		/*foreach (Node child in GetChildren())
		{
			this.RemoveChild(child);
			this.GetParent().AddChild(child);
			GD.Print(child.Name);
		}*/

	}
}
