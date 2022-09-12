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

		//TileMap shadowTileMap = GetNode<TileMap>("/root/Game/Shadows");
		TileMap shadowTileMap = GetNode<TileMap>("/root/Game/Trees");

		// For each tile
		foreach (Vector2 position in GetUsedCells())
		{

			// Make a new Sprite that copies the tile texture
			int tileId = GetCellv(position);
			Texture texture = this.TileSet.TileGetTexture(tileId);

			/* Sprite method
			Sprite sprite = new Sprite();
			sprite.Texture = texture;
			sprite.Position = MapToWorld(position);
			sprite.RegionEnabled = true;
			sprite.RegionRect = this.TileSet.TileGetRegion(index);

			// Color sprite to be black, with slight transparency.
			sprite.Modulate = new Color(0f, 0f, 0f, 1f);

			// Rotate 125 degrees, and squash it.
			//sprite.Rotation = Mathf.Deg2Rad(180);
			sprite.Scale = new Vector2(1f, 1f);
			sprite.FlipV = true;
			*/

			//GD.Print(sprite.RegionRect);

			Rect2 regionRect = this.TileSet.TileGetRegion(tileId);
			Polygon2D polygon2D = new Polygon2D();
			polygon2D.Position = MapToWorld(position);
			polygon2D.Texture = texture;
			Vector2[] uv = new Vector2[4];
			uv[0] = regionRect.Position;
			uv[1] = new Vector2(regionRect.End.x, regionRect.Position.y);
			uv[2] = regionRect.End;
			uv[3] = new Vector2(regionRect.Position.x, regionRect.End.y);
			polygon2D.Uv = uv;

			// This starts at 0,0, and defines the skew.
			//Vector2 skew = new Vector2(30, regionRect.Size.y + regionRect.Size.y / 2);
			Vector2 skew = new Vector2(regionRect.Size.y / 2, regionRect.Size.y + regionRect.Size.y / 2);

			Vector2[] polygon = new Vector2[4];
			Vector2 tileOffset = this.TileSet.TileGetTextureOffset(tileId);
			tileOffset += new Vector2(3, 0);  // Looks good visually. TODO: scale according to time of day.
			Vector2 ySortOffset = new Vector2(0, regionRect.Size.y - 8);  //-16 is to look good visually.
			polygon[0] = new Vector2(0, 0) + tileOffset - ySortOffset + skew;
			polygon[1] = new Vector2(regionRect.Size.x, 0) + tileOffset - ySortOffset + skew;
			polygon[2] = regionRect.Size + tileOffset - ySortOffset;
			polygon[3] = new Vector2(0, regionRect.Size.y) - ySortOffset + tileOffset;
			polygon2D.Polygon = polygon;
			polygon2D.Modulate = new Color(0f, 0f, 0f, 0.15f);

			// Try to get Y sort to work.
			polygon2D.Position += ySortOffset;

			// Add the sprite as a child to Shadows TileMap
			shadowTileMap.AddChild(polygon2D);


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
