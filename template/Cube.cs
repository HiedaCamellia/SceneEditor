using Godot;
using System;

public partial class Cube : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<CsgMesh3D>("bottom").Visible = GetMeta("bottom").AsBool();
        GetNode<CsgMesh3D>("top").Visible = GetMeta("top").AsBool();
        GetNode<CsgMesh3D>("north").Visible = GetMeta("north").AsBool();
        GetNode<CsgMesh3D>("south").Visible = GetMeta("south").AsBool();
        GetNode<CsgMesh3D>("west").Visible = GetMeta("west").AsBool();
        GetNode<CsgMesh3D>("east").Visible = GetMeta("east").AsBool();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
