using Godot;
using System;

public class Map : Node2D
{
    [Export] public int timeOfDay = 0;
    private int prevTimeOfDay = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (timeOfDay == prevTimeOfDay) return;

        TileMap[] tileMaps = new TileMap[]{
            (TileMap)GetNode("Grass"),
            (TileMap)GetNode("Shadows"),
            (TileMap)GetNode("Trees"),
        };

        timeOfDay = timeOfDay % 1440;  // 1440 minutes per day

        int nightTime2 = 360;   // Full night color 2 - 6 am
        int morningTime = 420;  // Full morning color - 7 am
        int dayTime = 720;      // Full day color     - 12 pm
        int dayTime2 = 1080;    // Full day color     - 6 pm
        int eveningTime = 1140; // Full evening color - 7 pm
        int nightTime = 1200;   // Full night color 1 - 8 pm

        float night2Component = 0f;
        float morningComponent = 0f;
        float dayComponent = 0f;
        float day2Component = 0f;
        float eveningComponent = 0f;
        float nightComponent = 0f;

        if (timeOfDay < nightTime2)
        {
            float distance = nightTime2 + 1440 - nightTime;
            nightComponent = 1f - (((timeOfDay + 1440) - nightTime) / distance);
            night2Component = 1f - (nightTime2 - timeOfDay) / distance;
        }
        else if (timeOfDay < morningTime)
        {
            float distance = morningTime - nightTime2;
            night2Component = 1f - (timeOfDay - nightTime2) / distance;
            morningComponent = 1f - (morningTime - timeOfDay) / distance;
        }
        else if (timeOfDay < dayTime)
        {
            float distance = dayTime - morningTime;
            morningComponent = 1f - (timeOfDay - morningTime) / distance;
            dayComponent = 1f - (dayTime - timeOfDay) / distance;
        }
        else if (timeOfDay < dayTime2)
        {
            float distance = dayTime2 - dayTime;
            dayComponent = 1f - (timeOfDay - dayTime) / distance;
            day2Component = 1f - (dayTime2 - timeOfDay) / distance;
        }
        else if (timeOfDay < eveningTime)
        {
            float distance = eveningTime - dayTime2;
            day2Component = 1f - (timeOfDay - dayTime2) / distance;
            eveningComponent = 1f - (eveningTime - timeOfDay) / distance;
        }
        else if (timeOfDay < nightTime)
        {
            float distance = nightTime - eveningTime;
            eveningComponent = 1f - (timeOfDay - eveningTime) / distance;
            nightComponent = 1f - (nightTime - timeOfDay) / distance;
        }
        else // timeOfDay < 1440
        {
            float distance = 1440 - nightTime;
            nightComponent = 1f - (timeOfDay - nightTime) / distance;
            night2Component = 1f - (1440 - timeOfDay) / distance;
        }

        Color night2Color = new Color(.5f, .5f, 1f, 1f);
        Color morningColor = new Color(.8f, .8f, 1.3f, 1f);
        Color dayColor = new Color(1f, 1f, 1f, 1f);
        Color day2Color = new Color(1f, 1f, 1f, 1f);
        Color eveningColor = new Color(1.1f, .75f, .55f, 1f);
        Color nightColor = new Color(.5f, .5f, 1f, 1f);

        // Get color based on timeOfDay
        Color color = (night2Color * night2Component) +
                      (morningColor * morningComponent) +
                      (dayColor * dayComponent) +
                      (day2Color * day2Component) +
                      (eveningColor * eveningComponent) +
                      (nightColor * nightComponent);
        color.a = 1f;  // Re-set alpha so that it's not in the range 0f - 1f.

        GD.Print(color);
        GD.Print(night2Component);
        GD.Print(morningComponent);
        GD.Print(dayComponent);
        GD.Print(day2Component);
        GD.Print(eveningComponent);
        GD.Print(nightComponent);

        foreach (TileMap tileMap in tileMaps)
        {
            // Get the tilemap
            TileSet tileSet = tileMap.TileSet;

            foreach (int tileId in tileSet.GetTilesIds())
            {
                tileSet.TileSetModulate(tileId, color);
            }

        }

        Sprite background = (Sprite)GetNode("Background");
        background.Modulate = color;

        // So that this doesn't get called each frame.
        timeOfDay = prevTimeOfDay;

    }

}