using Godot;

enum Direction
{
    Top,
    Bottom,
    North,
    South,
    West,
    East
}

class DirectionUtil
{
    public static Direction? GetFaceFromNormal(Vector3 normal)
    {
        if (normal == Vector3.Forward) return Direction.West;
        if (normal == Vector3.Back) return Direction.East;
        if (normal == Vector3.Left) return Direction.North;
        if (normal == Vector3.Right) return Direction.South;
        if (normal == Vector3.Up) return Direction.Top;
        if (normal == Vector3.Down) return Direction.Bottom;
        return null;
    }

    public static Vector3 GetNormalFromFace(Direction direction)
    {
        switch (direction)
        {
            case Direction.Top:
                return Vector3.Up;
            case Direction.Bottom:
                return Vector3.Down;
            case Direction.North:
                return Vector3.Left;
            case Direction.South:
                return Vector3.Right;
            case Direction.West:
                return Vector3.Forward;
            case Direction.East:
                return Vector3.Back;
            default:
                return Vector3.Zero;
        }
    }
}