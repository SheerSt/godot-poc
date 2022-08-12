using Godot;
using System;

public class Action : Node2D
{

    private Game _game;
    public Game game
    {
        get
        {

            if (_game == null) _game = GetNode<Game>("/root/Game");
            return _game;

        }
        set { }
    }

    public string layer;


}
