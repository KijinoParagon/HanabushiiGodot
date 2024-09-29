using Godot;
using System;
using System.Linq;

public partial class AkioChar : CharacterBody2D
{
	enum MoveStates : int {
		Idle,
		Walk,
		Run
	}

	enum SlideStates : int {
		None,
		Slide,
		Roll
	}

	enum JumpStates : int {
		Ground,
		Jump,
		WallJump, 
		DoubleJump,
		DoubleWallJump

	}
	JumpStates JumpState = JumpStates.Ground;
	MoveStates MoveState = MoveStates.Idle;
	SlideStates SlideState = SlideStates.None;
	
	//Vector2 velocity = Vector2.Zero;
	bool onGround = false;
	AnimatedSprite2D sprite;
	Vector2 velocity;
	[Export]
	float JumpHeight = 600;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//sprite = GetNode<AnimatedSprite2D>("Sprite");
		
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		sprite.Play();
		velocity.X = 0;
		velocity.Y = 0;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (velocity.Y <= 1200 && !onGround)
		{
			velocity.Y += (float) (1200 * (delta));
		}

		if (Input.IsActionJustPressed("Up"))
		{
			if(JumpState == JumpStates.Ground)
			{
				velocity.Y = -JumpHeight;
				JumpState = JumpStates.Jump;
			}
			else if (JumpState == JumpStates.Jump)
			{
				velocity.Y =  -JumpHeight;
				JumpState = JumpStates.DoubleJump;
			}
			
		}

		if (Input.IsActionPressed("Left")){
			velocity.X = (float) -(300);
		}
		if (Input.IsActionPressed("Right")){
			velocity.X = (float) (300);
		}
		if (Input.IsActionJustReleased("Left") || Input.IsActionJustReleased("Right")) {
			velocity.X = 0;
		}

		GD.Print(velocity);
		Velocity = velocity;
		MoveAndSlide();	
	
		onGround = false;
		//Checking for hitting ground below Akio
		for (int i = 0; i < GetSlideCollisionCount(); i++) 
		{ 
			KinematicCollision2D collision = GetSlideCollision(i);
			GD.Print(collision.GetNormal());
			if (collision.GetNormal().Y == -1 ){
				JumpState = JumpStates.Ground;
				onGround = true;
				velocity.Y = 0;
			}	
		}

		
	}
}
