using Godot;
using System;

public partial class CameraRay : RayCast3D
{
    public Camera3D camera;  
    public override void _Ready()
    {
        camera = GetParent().GetNode<Camera3D>("%Perspective");
    }

    public override void _Process(double delta)
    {

        Vector3 direction = -camera.Transform.Basis.Z.Normalized();

        Position = camera.Position;

        TargetPosition = direction * 10;  

    }
}
