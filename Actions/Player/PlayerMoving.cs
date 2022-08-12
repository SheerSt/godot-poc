using Godot;
using System;

public class PlayerMoving : Action
{

	// Good reference - https://cyoann.github.io/GodotSharpAPI/html/d73390a9-487c-b572-7323-a8aa485d54f1.htm
	int timer = 0;

	public WildsSprite sprite;

	public PlayerMoving()
	{

		layer = "MAP_7";

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		// Works
		Texture texture = ResourceLoader.Load<Texture>("brendan-walking.png");
		this.sprite = new WildsSprite(this, texture, 0, 0, 16, 16);

		GD.Print("PlayerMoving");

		// Nodes with higher priority value are called last.
		// This is required to process this after all child sprite components
		// have been processed (required for correct drawing).
		// TODO: could potentially be used for UI, ProcessPriority = 2 (Actions) / 3 (Sprites)
		ProcessPriority = 1;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

		game.draw(this.sprite);

		if (++this.timer > 60) {

			game.actionStack.remove(this);
			game.insertAction(new PlayerStanding());

		}
	}
}
