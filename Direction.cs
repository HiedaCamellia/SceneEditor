using Godot;

enum Direction
{
    Up,
    Down,
    Left,
    Right,
    Forward,
    Backward
}

class DirectionUtil
{
    public static Direction? GetFaceFromNormal(Vector3 normal)
    {
        if (normal == Vector3.Forward) return Direction.Forward;
        if (normal == Vector3.Back) return Direction.Backward;
        if (normal == Vector3.Left) return Direction.Left;
        if (normal == Vector3.Right) return Direction.Right;
        if (normal == Vector3.Up) return Direction.Up;
        if (normal == Vector3.Down) return Direction.Down;
        return null;
    }

    public static Vector3 GetNormalFromFace(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector3.Up;
            case Direction.Down:
                return Vector3.Down;
            case Direction.Left:
                return Vector3.Left;
            case Direction.Right:
                return Vector3.Right;
            case Direction.Forward:
                return Vector3.Forward;
            case Direction.Backward:
                return Vector3.Back;
            default:
                return Vector3.Zero;
        }
    }
}