using Godot;
using System;

public class Game : Node2D
{

	private ActionStack _actionStack;
	public ActionStack actionStack
	{
		get
		{

			if (_actionStack == null) _actionStack = GetNode<ActionStack>("/root/Game/ActionStack");
			return _actionStack;

		}
		set { }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

		if (Input.IsKeyPressed((int)KeyList.P)) {

			actionStack.PrintTreePretty();

		}

	}


	/**
	* TODO: remove. Idk why this would be needed.
	*/
	public void insertAction(string actionName)
	{

		PackedScene packedScene = ResourceLoader.Load<PackedScene>("res://Actions/Player/" + actionName + ".tscn");
		Action action = (Action)packedScene.Instance();

		Node2D layer = actionStack.GetNode<Node2D>(action.layer);
		layer.AddChild(action);

	}
	
	public void insertAction(Action action)
	{
		
		Node2D layer = actionStack.GetNode<Node2D>(action.layer);
		layer.AddChild(action);
		
	}

	/**
	* Ported method from libgdx.
	*/
	public void draw(WildsSprite sprite)
	{

		sprite.draw();

	}
}
