using Godot;
using System;
using System.IO;
using static System.Formats.Asn1.AsnWriter;

public partial class Editor : Node3D
{
    [Export]
    public PackedScene template;

    protected RayCast3D rayCast;
    protected Label label;
    protected SpinBox x;
    protected SpinBox y;
    protected SpinBox z;
    protected bool p;
    protected bool delMode;

    protected Node3D? Collider;
    protected Node3D? Selected;

    protected Node3D scene;

    private Direction? direction;

    public bool isColliding => rayCast.IsColliding();
    public bool isSelected => Selected != null;

    public void AddTemplate()
    {
        if(template == null)
        {
            time = 3;
            saveTip = "请先导入模板";
            return;
        }

        var i = template.Instantiate<Node3D>();
        var pos = new Vector3((float)x.Value, (float)y.Value, (float)z.Value);
        i.Position = pos;
        scene.AddChild(i);
    }

    public void initFolder()
    {
        string path = "save";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path = "template";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public void Export()
    {
        var t = GetNode<LineEdit>("%Path").Text;

        if (t.Length == 0)
        {
            return;
        }
        var pk = new PackedScene();

        foreach (var item in scene.GetChildren())
        {
            item.Owner = scene;
        }

        pk.Pack(scene);

        var e = ResourceSaver.Save(pk, "res://save/" + t + ".tscn");
        time = 3;
        if (e != Error.Ok)
        {
            saveTip = "保存场景" + t + "失败";
        }
        else
        {
            saveTip = "保存场景" + t + "成功";
        }

        GetNode<LineEdit>("%Path").Text = "";
    }
    public void Import()
    {
        var t = GetNode<LineEdit>("%Path").Text;
        if (t.Length == 0)
        {
            return;
        }
        var pk = ResourceLoader.Load<PackedScene>("res://save/" + t + ".tscn");
        time = 3;
        if (pk == null)
        {
            saveTip = "加载场景" + t + "失败";
            return;
        }
        else
        {
            saveTip = "加载场景" + t + "成功";
        }
        Selected = null;
        Collider = null;
        scene.QueueFree();
        scene = pk.Instantiate<Node3D>();
        AddChild(scene);

        GetNode<LineEdit>("%Path").Text = "";
    }

    public void ImportTemplate()
    {
        var t = GetNode<LineEdit>("%Path").Text;
        if (t.Length == 0)
        {
            return;
        }
        var pk = ResourceLoader.Load<PackedScene>("res://template/" + t + ".tscn");
        time = 3;
        if (pk == null)
        {
            saveTip = "加载模板" + t + "失败";
            return;
        }
        else
        {
            saveTip = "加载模板" + t + "成功";
        }

        template = pk;

        GetNode<LineEdit>("%Path").Text = "";
    }

    public void Select()
    {
        Selected = Collider;
        var pos = Collider.Position;
        x.Value = pos.X;
        y.Value = pos.Y;
        z.Value = pos.Z;
        p = true;
    }
    public void Deselect()
    {
        Selected = null;
    }

    public void Place()
    {
        if (delMode)
        {
            if(Collider == Selected)
            {
                Selected = null;
            }
            Collider.QueueFree();
            Collider = null;
        }
        else
        {
            var i = template.Instantiate<Node3D>();
            var pos = Collider.Position;
            if(direction.HasValue)
            {
                pos += DirectionUtil.GetNormalFromFace(direction.Value);
            }
            i.Position = pos;
            scene.AddChild(i);
        }
    }

    public void changeDelMode()
    {
        delMode = !delMode;
    }

    public override void _Ready()
    {
        initFolder();
        rayCast = GetNode<RayCast3D>("CameraRay");
        label = GetNode<Label>("%name");
        scene = GetNode<Node3D>("Scene");
        var containerPos = GetNode<HBoxContainer>("%position");
        x = containerPos.GetNode<SpinBox>("X");
        y = containerPos.GetNode<SpinBox>("Y");
        z = containerPos.GetNode<SpinBox>("Z");
        p = false;
        delMode = false;
        GetNode<Button>("%AddButton").Pressed += AddTemplate;
        GetNode<Button>("%ImportScene").Pressed += Import;
        GetNode<Button>("%ImportTemplate").Pressed += ImportTemplate;
        GetNode<Button>("%ExportScene").Pressed += Export;
        AddTemplate();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (isColliding)
        {
            if (rayCast.GetCollider() is StaticBody3D staticBody)
            {
                Node3D node = staticBody.GetParent<Node3D>();
                if (node != Collider)
                {
                    Collider = node;
                    p = false;
                }
                if (!p && !isSelected)
                {
                    var pos = Collider.Position;
                    x.Value = pos.X;
                    y.Value = pos.Y;
                    z.Value = pos.Z;
                    p = true;
                }
                var collisionNormal = rayCast.GetCollisionNormal();
                direction = DirectionUtil.GetFaceFromNormal(collisionNormal);
            }
        }
        if (isSelected)
        {
            label.Text = "[已选中]"+Selected.Name;
            Selected.Position = new Vector3((float)x.Value, (float)y.Value, (float)z.Value);
        }
        else
        {
            if (p && Collider!=null)
            {
                label.Text = Collider.Name;
                Collider.Position = new Vector3((float)x.Value, (float)y.Value, (float)z.Value);
            }
            if (!isColliding)
            {
                label.Text = "";
                Collider = null;
                p = false;
            }

        }

        if (delMode)
        {
            label.Text = "[删除模式]" + label.Text;
        }

        if(time > 0)
        {
            time -= (float)delta;
            label.Text = saveTip;
        }


    }

    private float time = 0;
    private string saveTip = "";
}
