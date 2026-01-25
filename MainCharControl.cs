using Godot;
using System;

public partial class MainCharControl : CharacterBody2D
{
	[Export]
	public int Speed {get; set;} = 300;

	public override void _PhysicsProcess(double delta)
	{
        GetInput();
        MoveAndSlide();
	}
		
    public void GetInput()
    {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Velocity = inputDirection * Speed;
    }
}
