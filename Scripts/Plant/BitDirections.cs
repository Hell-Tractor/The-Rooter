using Godot;

public class BitDirection {
    public static readonly int Left = 1;
    public static readonly int Right = 2;
    public static readonly int Up = 4;
    public static readonly int Down = 8;

    public static int FromVector2(Vector2 direction) {
        if (direction == Vector2.Left)
            return Left;
        if (direction == Vector2.Right)
            return Right;
        if (direction == Vector2.Up)
            return Up;
        if (direction == Vector2.Down)
            return Down;
        return 0;
    }
}