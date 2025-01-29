using Godot;
using System;

public partial class Navigater : CharacterBody3D
{
    private NavigationAgent3D _navigationAgent;

    private float _movementSpeed = 2.0f;

    private Vector3 _lastPostion = new Vector3();
    private Vector3 _targetPostion = new Vector3();

    public Vector3 MovementTarget
    {
        get { return _navigationAgent.TargetPosition; }
        set { _navigationAgent.TargetPosition = value; }
    }

    public override void _Ready()
    {
        base._Ready();

        _navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");

        // These values need to be adjusted for the actor's speed
        // and the navigation layout.
        _navigationAgent.PathDesiredDistance = 0.5f;
        _navigationAgent.TargetDesiredDistance = 0.5f;

        _targetPostion = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Vector3 currentAgentPosition = GlobalTransform.Origin;

        if (_navigationAgent.IsNavigationFinished() && currentAgentPosition.DistanceTo(_targetPostion) < 0.01 )
        {
            GlobalPosition = _targetPostion;
            return;
        }

        var p = _navigationAgent.GetFinalPosition();
        //如果p的每个值不为整数，则忽略
        if (Math.Abs(p.X - Math.Round(p.X)) > 0.01 || Math.Abs(p.Y - Math.Round(p.Y)) > 0.01 || Math.Abs(p.Z - Math.Round(p.Z)) > 0.01)
        {
            return;
        }


        if (currentAgentPosition.DistanceTo(_targetPostion) < 0.05 || _targetPostion.Equals(Vector3.Zero))
        {
            GlobalPosition = _targetPostion;
            GetNextTarget();
            Jump();
        }
        if(_targetPostion.Equals(Vector3.Zero))
        {
            return;
        }
        //GD.Print("currentAgentPosition: " + currentAgentPosition);

        Velocity = currentAgentPosition.DirectionTo(_targetPostion) * _movementSpeed;
        if(currentAgentPosition.DistanceTo(_targetPostion)< Velocity.Length()/60)
        {
            GlobalPosition = _targetPostion;
        }
        else
        {
            MoveAndSlide();
        }
    }

    private void GetNextTarget()
    {
        if (_navigationAgent.IsNavigationFinished())
        {
            return;
        }
        Vector3 nextPathPosition = _navigationAgent.GetNextPathPosition();
        var m = GlobalPosition.DirectionTo(nextPathPosition);
        if (m.X != 0 && m.Z != 0 && m.Y==0)
        {
            var n =_lastPostion.DirectionTo(_targetPostion);
            if (n.X == 0)
            {
                nextPathPosition = new Vector3(_lastPostion.X, nextPathPosition.Y, nextPathPosition.Z);
            }
            else if (n.Z == 0)
            {
                nextPathPosition = new Vector3(nextPathPosition.X, nextPathPosition.Y, _lastPostion.Z);
            }
        }

        GD.Print("nextPathPosition: " + nextPathPosition);
        _targetPostion = nextPathPosition;
        _lastPostion = GlobalPosition;

    }

    private void Jump()
    {
        //如果下一个target的移动不是沿坐标轴进行的，就是视差移动，直接跳转过去
        var n =_lastPostion.DirectionTo(_targetPostion);
        if (n.Y == 0) return;
        if (n.X == 0 && n.Z == 0) return;

        GlobalPosition = _targetPostion;
    }
}