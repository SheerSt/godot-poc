using Godot;
using System;

public class Game : Node2D
{

	public static Game instance;
	public bool debugMode = true;

	public override void _Ready()
	{

		if (Game.instance == null) Game.instance = this;

	}

}
