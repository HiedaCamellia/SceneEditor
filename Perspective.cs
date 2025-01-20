using Godot;
using System;

public partial class Perspective : Camera3D
{
    protected Editor editor;
    // Called when the node enters the scene tree for the first time.
    protected bool isCaptured = false;
    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseModeEnum.Captured);
        isCaptured = true;
        editor = GetParent<Editor>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("esc"))
        {
            Input.SetMouseMode(Input.MouseModeEnum.Visible);
            isCaptured = false;
        }
        if (isCaptured)
        {
            if (Input.IsActionPressed("up"))
            {
                Translate(new Vector3(0, 0.1f, 0));
            }
            if (Input.IsActionPressed("down"))
            {
                Translate(new Vector3(0, -0.1f, 0));
            }
            if (Input.IsActionPressed("left"))
            {
                Translate(new Vector3(-0.1f, 0, 0));
            }
            if (Input.IsActionPressed("right"))
            {
                Translate(new Vector3(0.1f, 0, 0));
            }
            if (Input.IsActionPressed("forward"))
            {
                Translate(new Vector3(0, 0, -0.1f));
            }
            if (Input.IsActionPressed("backward"))
            {
                Translate(new Vector3(0, 0, 0.1f));
            }
        }

    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouse eventMouse)
        {
            if (isCaptured && eventMouse is InputEventMouseMotion mouseMotion)
            {
                var x = mouseMotion.Relative.X;
                var y = mouseMotion.Relative.Y;

                var rotation = RotationDegrees;


                const float sensitivity = 0.1f;

                rotation.Y -= x * sensitivity;
                rotation.X -= y * sensitivity;


                rotation.X = Mathf.Clamp(rotation.X, -90.0f, 90.0f);


                RotationDegrees = rotation;
                GetViewport().SetInputAsHandled();
            }
            if (eventMouse is InputEventMouseButton mouseButton)
            {
                if (!isCaptured)
                {
                    Input.SetMouseMode(Input.MouseModeEnum.Captured);
                    isCaptured = true;
                }
                else
                {
                    if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.IsPressed())
                    {
                        if (editor.isColliding)
                        {
                            editor.Select();
                        }
                        else
                        {
                            editor.Deselect();
                        }
                    }
                    if(mouseButton.ButtonIndex == MouseButton.Right && mouseButton.IsPressed())
                    {
                        editor.Place();
                    }

                }
                GetViewport().SetInputAsHandled();
            }

        }
        if (@event.IsActionPressed("del"))
        {
            editor.changeDelMode();
            GetViewport().SetInputAsHandled();
        }
    }
}
