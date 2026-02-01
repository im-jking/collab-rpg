using System;
using Godot;

public partial class Enemy : CharacterBody2D
{
    [Export]
    public int Speed { get; set; }

    // The MainChar that will be tracked
    private MainCharControl _mainChar;

    // Are we aggroed to the MainChar?
    private bool _isAggro = false;

    public override void _Ready() { }

    public override void _Process(double delta)
    {
        // Find the MainChar - assumes Party is at same depth as Enemy
        _mainChar ??= GetNode<MainCharControl>("../Party/MainChar");

        // Check distance to MainChar
        Vector2 charLoc = _mainChar.GlobalTransform.Origin;
        float distance = charLoc.DistanceTo(GlobalTransform.Origin);

        // If close enough, become aggroed
        if (distance < 500.0f)
        {
            _isAggro = true;
        }
        else
        {
            _isAggro = false;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // If aggroed, move toward the player and around walls
        if (_isAggro)
        {
            MoveTowardPlayer();
        }
    }

    public void MoveTowardPlayer()
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = PhysicsRayQueryParameters2D.Create(
            GlobalTransform.Origin,
            _mainChar.GlobalTransform.Origin
        );

        // Exclude self from the ray-cast result
        query.Exclude = [GetRid()];
        Godot.Collections.Dictionary result = spaceState.IntersectRay(query);

        // If the player is visible, move toward it
        if (result.Count > 0)
        {
            GD.Print(
                "Collision with result ",
                (ulong)result["collider_id"],
                " and target ",
                _mainChar.GetInstanceId()
            );
            if (((ulong)result["collider_id"]) == _mainChar.GetInstanceId())
            {
                Velocity = Speed * Position.DirectionTo(_mainChar.GlobalTransform.Origin);
            }
        }
    }
}
