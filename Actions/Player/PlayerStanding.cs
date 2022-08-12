using Godot;
using System;

public class PlayerStanding : Action
{

	// Good reference - https://cyoann.github.io/GodotSharpAPI/html/d73390a9-487c-b572-7323-a8aa485d54f1.htm
	int timer = 0;

	public PlayerStanding()
	{

		layer = "MAP_7";

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		
		GD.Print("PlayerStanding");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

		if (++this.timer > 60) {

			//actionStack.RemoveChild(this);
			//PackedScene packedScene = ResourceLoader.Load<PackedScene>("res://Actions/Player/PlayerMoving.tscn");
			//actionStack.AddChild(packedScene.Instance());

			game.actionStack.remove(this);
			game.insertAction(new PlayerMoving());
		}
	}
}
