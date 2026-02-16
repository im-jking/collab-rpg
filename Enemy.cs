using System;
using System.Runtime.CompilerServices;
using Godot;

public partial class Enemy : CharacterBody2D
{
    [Export]
    public int Speed { get; set; }

    // Maximum health - declared in editor for generality
    [Export]
    public int MaxHealth { get; set; }

    // Health bar backfill - for setting size based on current health
    [Export]
    public Sprite2D HealthFillSprite;

    // Current health - operated on throughout script
    private int curHealth;

    // The MainChar that will be tracked
    private MainCharControl _mainChar;

    // Are we aggroed to the MainChar?
    private bool _isAggro = false;

    // Current target of movement
    private Vector2 target;

    public override void _Ready()
    {
        curHealth = MaxHealth;
    }

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

        // If the player is visible and unobstructed, move toward it
        if (result.Count > 0 && ((ulong)result["collider_id"]) == _mainChar.GetInstanceId())
        {
            target = _mainChar.GlobalTransform.Origin;
        }
        else // Non-player entity, or nothing, in line of sight
        {
            // Move around walls by tracking breadcrumbs
            foreach (Vector2 crumb in _mainChar.breadcrumbs)
            {
                Rect2 shape = GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect();
                float width = shape.Size.X;
                float height = shape.Size.Y;

                // Raycast to see if accessible - must be visible from all four corners of enemy
                PhysicsRayQueryParameters2D topRightRay = PhysicsRayQueryParameters2D.Create(
                    GlobalTransform.Origin + new Vector2(width, height),
                    crumb,
                    exclude: [GetRid()]
                );
                var topRightResult = spaceState.IntersectRay(topRightRay);

                PhysicsRayQueryParameters2D topLeftRay = PhysicsRayQueryParameters2D.Create(
                    GlobalTransform.Origin + new Vector2(-width, height),
                    crumb,
                    exclude: [GetRid()]
                );
                var topLeftResult = spaceState.IntersectRay(topLeftRay);

                PhysicsRayQueryParameters2D botRightRay = PhysicsRayQueryParameters2D.Create(
                    GlobalTransform.Origin + new Vector2(width, -height),
                    crumb,
                    exclude: [GetRid()]
                );
                var botRightResult = spaceState.IntersectRay(botRightRay);

                PhysicsRayQueryParameters2D botLeftRay = PhysicsRayQueryParameters2D.Create(
                    GlobalTransform.Origin + new Vector2(-width, -height),
                    crumb,
                    exclude: [GetRid()]
                );
                var botLeftResult = spaceState.IntersectRay(botLeftRay);

                // Nothing is obstructing this movement - go toward this
                if (
                    topRightResult.Count == 0
                    && topLeftResult.Count == 0
                    && botRightResult.Count == 0
                    && botLeftResult.Count == 0
                )
                {
                    String crumbString = "";
                    foreach (Vector2 stringCrumb in _mainChar.breadcrumbs)
                    {
                        crumbString += "[" + stringCrumb.X + ", " + stringCrumb.Y + "],";
                    }
                    // GD.Print("Moving toward crumb at ", crumb);
                    // GD.Print("Breadcrumbs = ", crumbString);

                    target = crumb;
                    break;
                }
            }
        }

        Velocity = Speed * Position.DirectionTo(target);
        MoveAndSlide();
    }

    // Function to call when damaging this enemy
    public void Damage(int damage)
    {
        int prevHealth = curHealth;
        curHealth -= damage;

        // TODO: animation/healthbar changes

        if (curHealth <= 0)
        {
            // TODO: Death state
            this.QueueFree();
        }
        else
        {
            // Update health bar animation
            if (HealthFillSprite != null)
            {
                float factor = (1.0f * curHealth) / (1.0f * prevHealth);
                HealthFillSprite.Scale *= new Vector2(factor, 1);
            }
        }
    }
}
