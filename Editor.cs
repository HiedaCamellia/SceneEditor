using Godot; using System; using System.IO;  public partial class Editor : Node3D {     [Export]     public PackedScene template;      protected RayCast3D rayCast;     protected Label label;     protected SpinBox xPosition;     protected SpinBox yPosition;     protected SpinBox zPosition;     protected SpinBox xRotation;     protected SpinBox yRotation;     protected SpinBox zRotation;      protected CheckBox top;     protected CheckBox bottom;     protected CheckBox east;     protected CheckBox west;     protected CheckBox south;     protected CheckBox north;

    protected CheckBox navtop;     protected CheckBox navbottom;     protected CheckBox naveast;     protected CheckBox navwest;     protected CheckBox navsouth;     protected CheckBox navnorth;      protected Button nav;     protected HBoxContainer navContainer;     protected HBoxContainer navVsiContainer;      protected bool p;     protected bool delMode;     protected bool navMode;     protected bool conMode;      protected Node3D? Collider;     protected Node3D? Selected;      protected Node3D scene;      private Direction? direction;      public bool isColliding => rayCast.IsColliding();     public bool isSelected => Selected != null;      public void AddTemplate()     {         if(template == null)         {             time = 3;             saveTip = "请先导入模板";             return;         } 
        var i = template.Instantiate<Node3D>(PackedScene.GenEditState.Instance);         var pos = new Vector3((float)xPosition.Value, (float)yPosition.Value, (float)zPosition.Value);         i.Position = pos;         scene.AddChild(i);     }      public void initFolder()     {         string path = "save";         if (!Directory.Exists(path))         {             Directory.CreateDirectory(path);         }         path = "template";         if (!Directory.Exists(path))         {             Directory.CreateDirectory(path);         }     }      public void Export()     {         var t = GetNode<LineEdit>("%Path").Text;          if (t.Length == 0)         {             return;         }         var pk = new PackedScene();          foreach(var node in scene.GetChildren())
        {
            node.Owner = scene;
        }           pk.Pack(scene);          var e = ResourceSaver.Save(pk, "res://save/" + t + ".tscn");         time = 3;         if (e != Error.Ok)         {             saveTip = "保存场景" + t + "失败";         }         else         {             saveTip = "保存场景" + t + "成功";         }          GetNode<LineEdit>("%Path").Text = "";     }     public void Import()     {         var t = GetNode<LineEdit>("%Path").Text;         if (t.Length == 0)         {             return;         }         var pk = ResourceLoader.Load<PackedScene>("res://save/" + t + ".tscn");         time = 3;         if (pk == null)         {             saveTip = "加载场景" + t + "失败";             return;         }         else         {             saveTip = "加载场景" + t + "成功";         }         Selected = null;         Collider = null;         scene.QueueFree();         scene = pk.Instantiate<Node3D>();         AddChild(scene);          GetNode<LineEdit>("%Path").Text = "";     }      public void ImportTemplate()     {         var t = GetNode<LineEdit>("%Path").Text;         if (t.Length == 0)         {             return;         }         var pk = ResourceLoader.Load<PackedScene>("res://template/" + t + ".tscn");         time = 3;         if (pk == null)         {             saveTip = "加载模板" + t + "失败";             return;         }         else         {             saveTip = "加载模板" + t + "成功";         }          template = pk;         template.ResourceLocalToScene = true;         GetNode<LineEdit>("%Path").Text = "";     }      private void testNode(Node3D node)
    {
        if (node == null) { navContainer.Visible = false; navVsiContainer.Visible=false; return; }
        top.Disabled = !node.HasMeta("top");
        bottom.Disabled = !node.HasMeta("bottom");
        east.Disabled = !node.HasMeta("east");
        west.Disabled = !node.HasMeta("west");
        south.Disabled = !node.HasMeta("south");
        north.Disabled = !node.HasMeta("north");
        navContainer.Visible = node.HasMeta("isNav");
        navVsiContainer.Visible = node.HasMeta("Nav");
        navtop.Disabled = !node.HasMeta("Nav");
        navbottom.Disabled = !node.HasMeta("Nav");
        naveast.Disabled = !node.HasMeta("Nav");
        navwest.Disabled = !node.HasMeta("Nav");
        navsouth.Disabled = !node.HasMeta("Nav");
        navnorth.Disabled = !node.HasMeta("Nav");
    }      public void Nav() => NavTo(new Vector3((float)navContainer.GetNode<SpinBox>("X").Value, (float)navContainer.GetNode<SpinBox>("Y").Value, (float)navContainer.GetNode<SpinBox>("Z").Value));      protected void NavTo(Vector3 vector)
    {
        if (Selected.HasNode("Navigater"))
        {
            var n = Selected.GetNode<Navigater>("Navigater");
            n.MovementTarget = vector;
        }
    }
     public void Select()     {         if (conMode && Selected != null && Collider!=null)
        {
            GD.Print("Linking "+Selected+" and "+Collider);
            link(Selected, Collider);
        }
        else
        {             Selected = Collider;             var pos = Collider.Position;             xPosition.Value = pos.X;             yPosition.Value = pos.Y;             zPosition.Value = pos.Z;             p = true;             testNode(Selected);
        }     }     public void Deselect()     {         Selected = null;
        navContainer.Visible = false;
        navVsiContainer.Visible = false;
    }      public void link(Node3D a,Node3D b)
    {
        if (a.HasNode("Nav") && b.HasNode("Nav")&&a!=b)
        {
            var na = a.GetNode<Node3D>("Nav");
            var nb = b.GetNode<Node3D>("Nav");
            var pa = a.Position;
            var pb = b.Position;
            NavigationLink3D node=null;
            GD.Print(direction);
            switch (direction)
            {
                case Direction.South:
                    node = na.GetNode<NavigationLink3D>("LinkNorth");
                    break;
                case Direction.North:
                     node = na.GetNode<NavigationLink3D>("LinkSouth");
                    break;
                case Direction.Bottom:
                    if(!conMode) break;
                     node = na.GetNode<NavigationLink3D>("LinkTop");
                    break;
                case Direction.Top:
                    if (!conMode) break;
                     node = na.GetNode<NavigationLink3D>("LinkBottom");
                    break;
                case Direction.East:
                     node = na.GetNode<NavigationLink3D>("LinkWest");
                    break;
                case Direction.West:
                     node = na.GetNode<NavigationLink3D>("LinkEast");
                    break;
                default:
                    break;
            }
            if (node == null) return;
            node.Enabled = true;
            node.EndPosition = pb - pa;
            node.Visible = true;
            NavigationServer3D.LinkSetMap(node.GetRid(), GetWorld3D().NavigationMap);
        }
    }      public void Place()     {         if (delMode)         {             if(Collider == Selected)             {                 Selected = null;             }             Collider.QueueFree();             Collider = null;         }         else         {             var pos = Collider.Position;             if(direction.HasValue)             {                 pos += DirectionUtil.GetNormalFromFace(direction.Value);             }             if (navMode)
            {
                NavTo(pos);
            }
            else
            {
                var l = template.GetLocalScene();
                var i = template.Instantiate<Node3D>(PackedScene.GenEditState.Instance);                 i.Position = pos;                 scene.AddChild(i);             }         }     }      public void changeDelMode()     {         delMode = !delMode;         conMode = false;         navMode = false;     }     public void changeNavMode()     {         navMode = !navMode;         delMode = false;         conMode = false;     }
    public void changeConMode()     {         conMode = !conMode;         delMode = false;         navMode = false;     }       public void changeTopVsi(bool bt)
    {
        if (Selected != null && !top.Disabled)
        {
            Selected.GetNode<CsgMesh3D>("top").Visible = bt;
            Selected.SetMeta("top", bt);
        }
    }     public void changeBottomVsi(bool bt)
    {
        if (Selected != null && !bottom.Disabled)
        {
            Selected.GetNode<CsgMesh3D>("bottom").Visible = bt;
            Selected.SetMeta("bottom", bt);
        }
    }     public void changeEastVsi(bool bt)
    {
        if (Selected != null && !east.Disabled)
        {
            Selected.GetNode<CsgMesh3D>("east").Visible = bt;
            Selected.SetMeta("east", bt);
        }
    }     public void changeWestVsi(bool bt)
    {
        if (Selected != null && !west.Disabled)
        {
            Selected.GetNode<CsgMesh3D>("west").Visible = bt;
            Selected.SetMeta("west", bt);
        }
    }     public void changeSouthVsi(bool bt)
    {
        if (Selected != null && !south.Disabled)
        {
            Selected.GetNode<CsgMesh3D>("south").Visible = bt;
            Selected.SetMeta("south", bt);
        }
    }     public void changeNorthVsi(bool bt)
    {
        if (Selected != null && !north.Disabled)
        {
            Selected.GetNode<CsgMesh3D>("north").Visible = bt;
            Selected.SetMeta("north", bt);
        }
    }

    public void changeNavTopVsi(bool bt)
    {
        if (Selected != null && !navtop.Disabled)
        {
            Selected.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkTop").Visible = bt;
        }
    }     public void changeNavBottomVsi(bool bt)
    {
        if (Selected != null && !navbottom.Disabled)
        {
            Selected.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkBottom").Visible = bt;
        }
    }     public void changeNavEastVsi(bool bt)
    {
        if (Selected != null && !naveast.Disabled)
        {
            Selected.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkEast").Visible = bt;
        }
    }     public void changeNavWestVsi(bool bt)
    {
        if (Selected != null && !navwest.Disabled)
        {
            Selected.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkWest").Visible = bt;
        }
    }     public void changeNavSouthVsi(bool bt)
    {
        if (Selected != null && !navsouth.Disabled)
        {
            Selected.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkSouth").Visible = bt;
        }
    }     public void changeNavNorthVsi(bool bt)
    {
        if (Selected != null && !navnorth.Disabled)
        {
            Selected.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkNorth").Visible = bt;
        }
    }     public override void _Ready()     {         initFolder();         rayCast = GetNode<RayCast3D>("CameraRay");         label = GetNode<Label>("%name");         scene = GetNode<Node3D>("Scene");         var containerPos = GetNode<HBoxContainer>("%position");         xPosition = containerPos.GetNode<SpinBox>("X");         yPosition = containerPos.GetNode<SpinBox>("Y");         zPosition = containerPos.GetNode<SpinBox>("Z");         var containerRot = GetNode<HBoxContainer>("%rotation");         xRotation = containerRot.GetNode<SpinBox>("X");         yRotation = containerRot.GetNode<SpinBox>("Y");         zRotation = containerRot.GetNode<SpinBox>("Z");         var containerVsi = GetNode<HBoxContainer>("%visible");         top = containerVsi.GetNode<CheckBox>("top");         bottom = containerVsi.GetNode<CheckBox>("bottom");         east = containerVsi.GetNode<CheckBox>("east");         west = containerVsi.GetNode<CheckBox>("west");         south = containerVsi.GetNode<CheckBox>("south");         north = containerVsi.GetNode<CheckBox>("north");         navContainer = GetNode<HBoxContainer>("%navposition");         nav = navContainer.GetNode<Button>("Nav");         navVsiContainer = GetNode<HBoxContainer>("%navvisible");         navtop = navVsiContainer.GetNode<CheckBox>("top");         navbottom = navVsiContainer.GetNode<CheckBox>("bottom");         naveast = navVsiContainer.GetNode<CheckBox>("east");         navwest = navVsiContainer.GetNode<CheckBox>("west");         navsouth = navVsiContainer.GetNode<CheckBox>("south");         navnorth = navVsiContainer.GetNode<CheckBox>("north");           p = false;         delMode = false;         navMode = false;         conMode = false;         GetNode<Button>("%AddButton").Pressed += AddTemplate;         GetNode<Button>("%ImportScene").Pressed += Import;         GetNode<Button>("%ImportTemplate").Pressed += ImportTemplate;         GetNode<Button>("%ExportScene").Pressed += Export;         AddTemplate();          top.Toggled += changeTopVsi;         bottom.Toggled += changeBottomVsi;         north.Toggled += changeNorthVsi;         south.Toggled += changeSouthVsi;         east.Toggled += changeEastVsi;         west.Toggled += changeWestVsi;

        navtop.Toggled += changeNavTopVsi;         navbottom.Toggled += changeNavBottomVsi;         navnorth.Toggled += changeNavNorthVsi;         navsouth.Toggled += changeNavSouthVsi;         naveast.Toggled += changeNavEastVsi;         navwest.Toggled += changeNavWestVsi;          nav.Pressed += Nav;     }      // Called every frame. 'delta' is the elapsed time since the previous frame.     public override void _Process(double delta)     {         if (isColliding)         {             if (rayCast.GetCollider() is PhysicsBody3D body)             {                 Node3D node = body.GetParent<Node3D>();                 if (node != Collider)                 {                     Collider = node;                     p = false;                 }                 if (!p && !isSelected)                 {                     var pos = Collider.Position;                     xPosition.Value = pos.X;                     yPosition.Value = pos.Y;                     zPosition.Value = pos.Z;                     var rot = Collider.RotationDegrees;                     xRotation.Value = rot.X;                     yRotation.Value = rot.Y;                     zRotation.Value = rot.Z;                     testNode(node);                     if (!top.Disabled)top.SetPressedNoSignal(Collider.GetNode<CsgMesh3D>("top").Visible);
                    if (!bottom.Disabled) bottom.SetPressedNoSignal(Collider.GetNode<CsgMesh3D>("bottom").Visible);
                    if (!east.Disabled) east.SetPressedNoSignal(Collider.GetNode<CsgMesh3D>("east").Visible);
                    if (!west.Disabled) west.SetPressedNoSignal(Collider.GetNode<CsgMesh3D>("west").Visible);
                    if (!south.Disabled) south.SetPressedNoSignal(Collider.GetNode<CsgMesh3D>("south").Visible);
                    if (!north.Disabled) north.SetPressedNoSignal(Collider.GetNode<CsgMesh3D>("north").Visible);
                    if(Collider.HasMeta("Nav")){
                        if (!navtop.Disabled) navtop.SetPressedNoSignal(Collider.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkTop").Visible);
                        if (!navbottom.Disabled) navbottom.SetPressedNoSignal(Collider.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkBottom").Visible);
                        if (!naveast.Disabled) naveast.SetPressedNoSignal(Collider.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkEast").Visible);
                        if (!navwest.Disabled) navwest.SetPressedNoSignal(Collider.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkWest").Visible);
                        if (!navsouth.Disabled) navsouth.SetPressedNoSignal(Collider.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkSouth").Visible);
                        if (!navnorth.Disabled) navnorth.SetPressedNoSignal(Collider.GetNode<Node3D>("Nav").GetNode<NavigationLink3D>("LinkNorth").Visible);
                    }                     p = true;                 }                 var collisionNormal = rayCast.GetCollisionNormal();                 direction = DirectionUtil.GetFaceFromNormal(collisionNormal);             }         }         if (isSelected)         {             label.Text = "[已选中]"+Selected.Name;             Selected.Position = new Vector3((float)xPosition.Value, (float)yPosition.Value, (float)zPosition.Value);             Selected.RotationDegrees = new Vector3((float)xRotation.Value, (float)yRotation.Value, (float)zRotation.Value);         }         else         {             if (p && Collider!=null)             {                 label.Text = Collider.Name;                 //Collider.Position = new Vector3((float)xPosition.Value, (float)yPosition.Value, (float)zPosition.Value);
                //Collider.RotationDegrees = new Vector3((float)xRotation.Value, (float)yRotation.Value, (float)zRotation.Value);             }             if (!isColliding)             {                 label.Text = "";                 Collider = null;                 p = false;                 navContainer.Visible = false;                 navVsiContainer.Visible = false;             }          }          if (delMode)         {             label.Text = "[删除模式]" + label.Text;         }         if (conMode)         {             label.Text = "[连接模式]" + label.Text;         }         if (navMode)         {             label.Text = "[导航模式]" + label.Text;         }          if (time > 0)         {             time -= (float)delta;             label.Text = saveTip;         }       }      private float time = 0;     private string saveTip = ""; } 
