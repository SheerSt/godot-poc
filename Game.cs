using Godot;
using System;

public class Game : Node2D
{

	public static Game instance;

	public override void _Ready()
	{

		if (Game.instance == null) Game.instance = this;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

		if (Input.IsKeyPressed((int)KeyList.P)) {

			//actionStack.PrintTreePretty();

		}

	}

	/**
	* Ported method from libgdx.
	*/
	public void draw(WildsSprite sprite)
	{

		sprite.draw();

	}
}
