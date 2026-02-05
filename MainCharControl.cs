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

    public override void _PhysicsProcess(double delta)
    {
        GetInput();
        MoveAndSlide();
        ProcessBreadcrumb();
    }

    public override void _Ready()
    {
        // Initialize breadcrumbs queue
        breadcrumbs = new Queue<Vector2>();
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
                if (breadcrumbs.Count > 10)
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
}
