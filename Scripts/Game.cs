using Godot;
using System;

public partial class Game : Node2D
{

    public static Game instance;
    public bool debugMode = true;
    public static Random random = new Random();

    public override void _Ready()
    {

        if (Game.instance == null) Game.instance = this;

    }

}
