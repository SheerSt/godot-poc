using Godot;
using System;

public partial class Trees : TileMap {

    Node2D shadows;

    public override void _Ready()
    {
        shadows = GetNode<Node2D>("Shadows");
        Init();
    }

    public void Init()
    {
        // For each tile
        int layer = 0;
        foreach (Vector2i position in GetUsedCells(layer))
        {
            // Make a new Sprite2D that copies the tile texture
            TileData tileData = GetCellTileData(layer, position);

            TileSetAtlasSource atlasSource = (TileSetAtlasSource)this.TileSet.GetSource(layer);
            Texture2D texture = atlasSource.Texture;

            Shadow polygon2D = new Shadow();
            Rect2 regionRect = polygon2D.regionRect = atlasSource.GetTileTextureRegion(position);
            polygon2D.originalPosition = MapToLocal(position);
            polygon2D.Texture = texture;
            //
            Vector2[] uv = new Vector2[4];
            uv[0] = regionRect.Position;
            uv[1] = new Vector2(regionRect.End.x, regionRect.Position.y);
            uv[2] = regionRect.End;
            uv[3] = new Vector2(regionRect.Position.x, regionRect.End.y);
            polygon2D.Uv = uv;

            polygon2D.tileOffset = tileData.TextureOffset;
            polygon2D.tileOffset += new Vector2(3, 0);  // Looks good visually.

            // Add the sprite as a child to Shadows TileMap
            shadows.AddChild(polygon2D);
        }
    }

    public void UpdateShadows(int timeOfDay)
    {
        // The 1400 is arbitrary to sync it, idk why it's not synced by default.
        float percentThroughDay = (timeOfDay + 1400 % 720) / 720f;

        // Not sure why this +5 is needed. if maxY changes, the 5 needs to change
        float xPercent = (float)Math.Sin(percentThroughDay * Math.PI * 2);
        float yPercent = (float)Math.Cos(percentThroughDay * Math.PI * 2) + 5;

        // For each tile
        foreach (Shadow polygon2D in shadows.GetChildren())
        {
            Rect2 regionRect = polygon2D.regionRect;

            // This starts at 0,0, and defines the skew.
            // Vector2 skew = new Vector2(regionRect.Size.y / 2, regionRect.Size.y + regionRect.Size.y / 2);  // TODO: remove
            //float maxY = regionRect.Size.y + (regionRect.Size.y / 2);
            //Vector2 skew = new Vector2(regionRect.Size.y / 2, maxY * yPercent);
            float maxX = (regionRect.Size.y / 4);
            float maxY = (regionRect.Size.y / 4);
            Vector2 skew = new Vector2(maxX * xPercent, maxY * yPercent);

            Vector2[] polygon = new Vector2[4];
            Vector2 tileOffset = polygon2D.tileOffset;
            // The -12 is to look good visually.
            // There were also issues where shadows would appear above objects below.
            Vector2 ySortOffset = new Vector2(0, regionRect.Size.y - 12);
            polygon[0] = new Vector2(0, 0) + tileOffset - ySortOffset + skew;
            polygon[1] = new Vector2(regionRect.Size.x, 0) + tileOffset - ySortOffset + skew;
            polygon[2] = regionRect.Size + tileOffset - ySortOffset;
            polygon[3] = new Vector2(0, regionRect.Size.y) - ySortOffset + tileOffset;
            polygon2D.Polygon = polygon;
            polygon2D.Modulate = new Color(0f, 0f, 0f, 0.15f);

            // Try to get Y sort to work.
            polygon2D.Position = polygon2D.originalPosition + ySortOffset;

        }

    }

}
