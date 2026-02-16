using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class MainCharControl : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 300;

    // Object near enough to interact with - set by other object
    public TalkNpc intObject;

    // Text box to scroll through - belongs to NPC
    public SpeechText intTextBox;

    // Breadcrumbs - recent locations for enemies to follow
    public Queue<Vector2> breadcrumbs;

    // Maximum health - declared in editor
    [Export]
    public int MaxHealth { get; set; }

    // TEST: Enemy to damage
    [Export]
    public Enemy enemy { get; set; }

    // Current health - operated on throughout script
    private int curHealth;

    public override void _Ready()
    {
        // Set initial health
        curHealth = MaxHealth;

        // Initialize breadcrumbs queue
        breadcrumbs = new Queue<Vector2>();
    }

    public override void _PhysicsProcess(double delta)
    {
        GetInput();
        MoveAndSlide();
        ProcessBreadcrumb();
    }

    public void GetInput()
    {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = inputDirection * Speed;

        // Interact with nearby object
        if (Input.IsActionJustPressed("interact") && intObject != null)
        {
            intObject.isInteracted = true;
        }

        // Scroll through text box when applicable
        if (Input.IsActionJustPressed("scroll") && intTextBox != null)
        {
            SpeechText intTextScript = intTextBox as SpeechText;
            intTextScript.ScrollDown();
        }

        // TEST: Damage enemy
        if (Input.IsActionJustPressed("damage_test"))
        {
            enemy.Damage(50);
        }
    }

    public void ProcessBreadcrumb()
    {
        Vector2 curPos = GlobalTransform.Origin;

        if (breadcrumbs.Count > 0)
        {
            Vector2 prevCrumb = breadcrumbs.Last();

            // Calculate vector distance between current player location and previous breadcrumb location
            float vectorDist =
                Math.Abs(Math.Abs(curPos.X) - Math.Abs(prevCrumb.X))
                + Math.Abs(Math.Abs(curPos.Y) - Math.Abs(prevCrumb.Y));

            // If far enough, add a new breadcrumb
            if (vectorDist > 2)
            {
                breadcrumbs.Enqueue(curPos);

                // If queue is long enough, remove the earliest entry
                if (breadcrumbs.Count > 20)
                {
                    breadcrumbs.Dequeue();
                }
            }
        }
        else
        {
            // If no crumbs yet, place one!
            breadcrumbs.Enqueue(curPos);
        }
    }

    // Function for enemies to call when damaging this character
    public void Damage(int damage)
    {
        curHealth -= damage;

        // TODO: animation/healthbar changes

        if (curHealth <= 0)
        {
            // TODO: Death state
        }
    }
}
