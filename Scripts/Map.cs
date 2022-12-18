using Godot;
using System;

public partial class Map : Node2D
{
	[Export] public int timeOfDay = 0;
	private int prevTimeOfDay = 0;
	private float timer = 0f;

	public Color colorTint = new Color();
	Node2D sunLight;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sunLight = GetNode<Node2D>("Sun");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		//        timer += delta;
		//        if (timer < 1f) return;

		//        timer = 0f;
		//        timeOfDay += 4;
		++timeOfDay;

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

		// All child nodes get this applied.
		Modulate = color;

		// Debug: display day/night
		Sprite2D dayNightSprite = (Sprite2D)GetNode("Trees/Player/Camera2D/DayNight");
		float xPos = 0f;
		if (timeOfDay < 420)
		{
			dayNightSprite.Frame = 1;
			xPos = ((timeOfDay + 240) % 660) / 660f;
		}
		else if (timeOfDay < 1200)
		{
			dayNightSprite.Frame = 0;
			xPos = ((timeOfDay - 420) % 780) / 780f;
		}
		else
		{
			dayNightSprite.Frame = 1;
			xPos = ((timeOfDay - 1200) % 660) / 660f;
		}

		double yPos = Math.Cos(xPos * Math.PI * 2) + 1;
		yPos *= 10;

		// Goes right-to-left
		xPos *= 160;
		xPos -= 80;
		dayNightSprite.Position = new Vector2(-(int)xPos, -50 + (float)yPos);

		// So that this doesn't get called each frame.
		prevTimeOfDay = timeOfDay;

		// Update shadows based on time of day.
		Trees trees = (Trees)GetNode("Trees");
		trees.UpdateShadows(timeOfDay);

		//Adjust sun light.
		float percentThroughDay = ((timeOfDay + 1100) % 1440) / 1440f;
		xPos = (float)Math.Cos(percentThroughDay * Math.PI * 2);
		yPos = -(float)Math.Sin(percentThroughDay * Math.PI * 2);
		sunLight.GlobalPosition = new Vector2(400 * xPos, 400 * (float)yPos);

	}

}



// TODO: remove

/*
foreach (TileMap tileMap in tileMaps)
{
	// Get the tilemap
	TileSet tileSet = tileMap.TileSet;

	foreach (int tileId in tileSet.GetTilesIds())
	{
		tileSet.TileSetModulate(tileId, color);
	}

}

Sprite2D background = (Sprite2D)GetNode("Background");
background.Modulate = color;

Sprite2D playerSprite = (Sprite2D)GetNode("Trees/Player/Sprite2D");
playerSprite.Modulate = color;
*/
