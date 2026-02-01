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

    [Export]
    NavigationAgent2D _navigationAgent;

    private void OnVelocityComputed(Vector2 safeVelocity)
    {
        Velocity = safeVelocity;
        MoveAndSlide();
    }

    public override void _Ready()
    {
        _navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        _navigationAgent.VelocityComputed += OnVelocityComputed;
    }

    public override void _Process(double delta)
    {
        // Find the MainChar - assumes Party is at same depth as Enemy
        _mainChar ??= GetNode<MainCharControl>("../Party/MainChar");

        // Track the player's current position
        _navigationAgent.TargetPosition = _mainChar.GlobalTransform.Origin;

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
        // var spaceState = GetWorld2D().DirectSpaceState;
        // var query = PhysicsRayQueryParameters2D.Create(
        //     GlobalTransform.Origin,
        //     _mainChar.GlobalTransform.Origin
        // );

        // Do not query when the map has never synchronized and is empty.
        if (NavigationServer2D.MapGetIterationId(_navigationAgent.GetNavigationMap()) == 0)
        {
            return;
        }

        if (_navigationAgent.IsNavigationFinished())
        {
            return;
        }

        Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
        Vector2 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * Speed;
        if (_navigationAgent.AvoidanceEnabled)
        {
            _navigationAgent.Velocity = newVelocity;
        }
        else
        {
            OnVelocityComputed(newVelocity);
        }

        // // Exclude self from the ray-cast result
        // query.Exclude = [GetRid()];
        // Godot.Collections.Dictionary result = spaceState.IntersectRay(query);

        // // If the player is visible, move toward it
        // if (result.Count > 0)
        // {
        //     if (((ulong)result["collider_id"]) == _mainChar.GetInstanceId())
        //     {
        //         Velocity = Speed * Position.DirectionTo(_mainChar.GlobalTransform.Origin);
        //         MoveAndSlide();
        //     }
        //     else // Move around walls if possible
        //     { }
        // }
    }
}
