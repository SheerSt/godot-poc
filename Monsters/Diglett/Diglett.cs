using Godot;
using System;

public class Diglett : Monster
{

    Timer dirtTimer;
    AudioStreamPlayer2D dirtSound;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        base._Ready();
        dirtTimer = (Timer)FindNode("DirtTimer");
        dirtSound = (AudioStreamPlayer2D)FindNode("DirtSound");

    }

    public override void _Process(float delta)
    {

        base._Process(delta);

        if (this.walkingTimer.IsStopped() && this.moveTimer.IsStopped())
            this.dirtTimer.Paused = true;

    }
    private void _on_DirtTimer_timeout()
    {

        //
        PackedScene footPrints = GD.Load<PackedScene>("res://Monsters/Dirt-Trail.tscn");
        Node node = footPrints.Instance();
        //Sprite sprite = (Sprite)node.FindNode("Sprite");
        //sprite.Position = Position.Floor() + new Vector2(0, 2);  // y = 8 - yOffset
        ((Node2D)node).Position = Position.Floor() + new Vector2(0, -5);
        AnimationPlayer animationPlayer = (AnimationPlayer)node.FindNode("AnimationPlayer");
        animationPlayer.CurrentAnimation = "footprints-fade-sand";
        animationPlayer.Play();

        //
        Node2D map = (Node2D)Game.instance.FindNode("Map");
        TileMap tileMap = (TileMap)map.FindNode("Trees");
        tileMap.AddChild(node);  // BelowNode(tileMap, 


        //
        PackedScene dirt = GD.Load<PackedScene>("res://Monsters/debris1.tscn");
        node = dirt.Instance();
        //((Node2D)node).Position = Position.Floor() + new Vector2(0, 16);  // y = 8 - yOffset
        ((Node2D)node).Position = new Vector2(0, 4);
        animationPlayer = (AnimationPlayer)node.FindNode("AnimationPlayer");
        animationPlayer.CurrentAnimation = "Anim1";
        animationPlayer.Play();
        //tileMap = (TileMap)map.FindNode("Trees");
        this.AddChild(node);

    }

    private void _on_MoveTimer_timeout()
    {

        base._on_MoveTimer_timeout();

        //
        dirtTimer.Paused = false;
        dirtTimer.Start();
        dirtSound.Play();

    }


}
