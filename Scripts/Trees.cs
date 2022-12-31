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
            int tileId = GetCellSourceId(layer, position);

            TileSetAtlasSource atlasSource = (TileSetAtlasSource)TileSet.GetSource(tileId);

            Shadow polygon2D = new Shadow();
            // TODO: tile atlas is likely set up wrong, every unique tile id is at position 0,0 and
            // is in a different atlas.
            // Vector2i regionSize = atlasSource.GetTileTextureRegion(new Vector2i(0, 0)).Size;
            // Rect2 regionRect = polygon2D.regionRect = new Rect2(0, 0, regionSize.x, regionSize.y);
            Rect2 regionRect = polygon2D.regionRect = atlasSource.GetTileTextureRegion(new Vector2i(0 ,0));
            polygon2D.originalPosition = ToGlobal(MapToLocal(position));
            polygon2D.Texture = atlasSource.Texture;
            //
            Vector2[] uv = new Vector2[4];
            uv[0] = regionRect.Position;
            uv[1] = new Vector2(regionRect.End.x, regionRect.Position.y);
            uv[2] = regionRect.End;
            uv[3] = new Vector2(regionRect.Position.x, regionRect.End.y);
            polygon2D.Uv = uv;

            // GD.Print(tileData.TextureOffset); -> they are all 0,0 now.
            // Keeping this incase there's still a use-case for it.
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

        // Used by light angle shader.
        float light_angle = (((timeOfDay + 1080) % 1440) / 1440f) * (float)Math.PI * 2.0f;

        float lightFade = (((timeOfDay + 1080) % 1440) / 1440f);
        lightFade = (float)Math.Sin(lightFade * Math.PI * 2) + 1f;
        lightFade = Math.Min((lightFade), 1.0f);
        lightFade = Math.Max(lightFade, .3f);

        // TODO: I feel like the tile atlases are set up wrong, this shouldn't be necessary?
        // If the material was shared, maybe it would update, not sure.
        // Regular trees.
        TileSetAtlasSource atlasSource = (TileSetAtlasSource)this.TileSet.GetSource(1);
        TileData tileData = atlasSource.GetTileData(new Vector2i(0, 0), 0);
        (tileData.Material as ShaderMaterial).SetShaderParameter("light_angle", light_angle);
        (tileData.Material as ShaderMaterial).SetShaderParameter("light_color", new Vector3(1f, 1f, 1f) * lightFade);
        // Large trees.
        atlasSource = (TileSetAtlasSource)this.TileSet.GetSource(5);
        tileData = atlasSource.GetTileData(new Vector2i(0, 0), 0);
        (tileData.Material as ShaderMaterial).SetShaderParameter("light_angle", light_angle);
        (tileData.Material as ShaderMaterial).SetShaderParameter("light_color", new Vector3(1f, 1f, 1f) * lightFade);  // TODO: uncomment

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
