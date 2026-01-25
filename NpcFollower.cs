using Godot;
using System;
using System.Collections.Generic;

public partial class NpcFollower : CharacterBody2D
{
	[Export]
	public MainFollower Leader {get; set;} = null;
	
	public int Speed {get; set;} = 0;
	private Queue<Vector2> followQueue;

	public override void _Ready()
	{
		if(Leader != null)
		{
			Speed = Leader.Speed;
		}

		followQueue = new Queue<Vector2>();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Require leader to process any movement
		if(Leader == null) {
			Console.WriteLine("Leader is null");
			return;
		}

		// Only move when the leader moves
		if(Leader.Leader.Velocity != new Vector2(0,0))
		{
			FollowLeader();
		}
	}

	public void FollowLeader() {
		// Delay movement
		if (followQueue.Count > 30) {
			// Remove and return least recent Velocity
			Velocity = followQueue.Dequeue();

			MoveAndSlide();	
		}

		followQueue.Enqueue(Leader.Velocity);
	}
}
