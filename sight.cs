using Godot;
using System;

public partial class sight : Sprite2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var s = GetViewport().GetWindow().Size;
        Position = new Vector2(s.X / 2, s.Y / 2);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
