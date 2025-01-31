using Godot;
using System;

public partial class Stair : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<Node3D>("bottom").Visible = GetMeta("bottom").AsBool();
        GetNode<Node3D>("top").Visible = GetMeta("top").AsBool();
        GetNode<Node3D>("north").Visible = GetMeta("north").AsBool();
        GetNode<Node3D>("south").Visible = GetMeta("south").AsBool();
        GetNode<Node3D>("west").Visible = GetMeta("west").AsBool();
        GetNode<Node3D>("east").Visible = GetMeta("east").AsBool();
        var nav = GetNode<Node3D>("Nav");
        if (!GetMeta("LkNorth").AsVector3().Equals(Vector3.Zero))
        {
            var node = nav.GetNode<NavigationLink3D>("LinkNorth");
            node.Enabled = true;
            node.Visible = true;
            node.EndPosition = GetMeta("LkNorth").AsVector3();
            NavigationServer3D.LinkSetMap(node.GetRid(), GetWorld3D().NavigationMap);
        }
        if (!GetMeta("LkSouth").AsVector3().Equals(Vector3.Zero))
        {
            var node = nav.GetNode<NavigationLink3D>("LinkSouth");
            node.Enabled = true;
            node.Visible = true;
            node.EndPosition = GetMeta("LkSouth").AsVector3();
            NavigationServer3D.LinkSetMap(node.GetRid(), GetWorld3D().NavigationMap);
        }
        if (!GetMeta("LkWest").AsVector3().Equals(Vector3.Zero))
        {
            var node = nav.GetNode<NavigationLink3D>("LinkWest");
            node.Enabled = true;
            node.Visible = true;
            node.EndPosition = GetMeta("LkWest").AsVector3();
            NavigationServer3D.LinkSetMap(node.GetRid(), GetWorld3D().NavigationMap);
        }
        if (!GetMeta("LkEast").AsVector3().Equals(Vector3.Zero))
        {
            var node = nav.GetNode<NavigationLink3D>("LinkEast");
            node.Enabled = true;
            node.Visible = true;
            node.EndPosition = GetMeta("LkEast").AsVector3();
            NavigationServer3D.LinkSetMap(node.GetRid(), GetWorld3D().NavigationMap);
        }
        if (!GetMeta("LkTop").AsVector3().Equals(Vector3.Zero))
        {
            var node = nav.GetNode<NavigationLink3D>("LinkTop");
            node.Enabled = true;
            node.Visible = true;
            node.EndPosition = GetMeta("LkTop").AsVector3();
            NavigationServer3D.LinkSetMap(node.GetRid(), GetWorld3D().NavigationMap);
        }
        if (!GetMeta("LkBottom").AsVector3().Equals(Vector3.Zero))
        {
            var node = nav.GetNode<NavigationLink3D>("LinkBottom");
            node.Enabled = true;
            node.Visible = true;
            node.EndPosition = GetMeta("LkBottom").AsVector3();
            NavigationServer3D.LinkSetMap(node.GetRid(), GetWorld3D().NavigationMap);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
