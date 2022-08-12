using Godot;
using System;

public class ActionStack : Node2D
{

	public void remove(Action action)
	{

		Node2D layer = GetNode<Node2D>(action.layer);
		layer.RemoveChild(action);

	}

}
