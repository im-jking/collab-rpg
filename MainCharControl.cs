using System;
using Godot;

public partial class MainCharControl : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 300;

    // Object near enough to interact with - set by other object
    public TalkNpc intObject;

    // Text box to scroll through
    public SpeechText intTextBox;

    public override void _PhysicsProcess(double delta)
    {
        GetInput();
        MoveAndSlide();
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
}
