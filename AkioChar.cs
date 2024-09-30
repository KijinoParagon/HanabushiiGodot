using Godot;
using System;
using System.Linq;

public partial class AkioChar : CharacterBody2D
{

	/*
		We use enums to enumerate the possible states

		This helps keep it clear what we're setting it to
	*/
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

	//Now we have the state values
	JumpStates JumpState = JumpStates.Ground;
	MoveStates MoveState = MoveStates.Idle;
	SlideStates SlideState = SlideStates.None;
	
	
	//Here are a few init variables
	bool onGround = false;
	bool onWall = false;
	ulong lastWallId = 0;
	ulong currentWallId = 0;
	bool wallLeft;
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
		//If the player is on the wall, they'll fall slower
		if(onWall){
			velocity.Y += (float) (600 * delta);
		}
		else if (velocity.Y <= 1200 && !onGround){
			velocity.Y += (float) (1200 * delta);
		}

		if(velocity.X > 0)
		{
			velocity.X -= (float) (delta * 1200);
			if(velocity.X < 0)
				velocity.X = 0;
		}
		if(velocity.X < 0)
		{
			velocity.X += (float) (delta * 1200);
			if(velocity.X > 0)
				velocity.X = 0;
		}

		//How to handle the jump input
		if (Input.IsActionJustPressed("Up")){

			//Check to see if they're off the ground.
			//If they are, it's a double jump.
			if(JumpState == JumpStates.Ground && !onGround){
				velocity.Y = -JumpHeight;
				JumpState = JumpStates.DoubleJump;
				sprite.Play("Jump");
			}
			//If on the ground, it's the first jump.
			else if (JumpState == JumpStates.Ground){
				velocity.Y = -JumpHeight;
				JumpState = JumpStates.Jump;
				sprite.Play("Jump");
			}
			/*If they're on a wall, it's a wall jump.
			  Also, set the lastWallId to the current.
			  This is so that we can prevent them from clinging
			  to the same wall on repeat.
			  We also add a kick off, so they push away from wall.
			*/
			else if (onWall){
				lastWallId = currentWallId;
				velocity.Y = -JumpHeight;
				velocity.X = (wallLeft) ? 300 : -300;
				onWall = false;
				sprite.FlipH = !sprite.FlipH;
				sprite.Play("DoubleJump");
			}
			/*
				If they're in the first jump, it's a double jump
			*/
			else if (JumpState == JumpStates.Jump || JumpState == JumpStates.WallJump){
				velocity.Y =  -JumpHeight;
				JumpState = JumpStates.DoubleJump;
				sprite.Play("DoubleJump");
			}
			//Otherwise, they have no more jumps...			
		}

		//These next two handle horizontal movement
		if (Input.IsActionPressed("Left")){
			velocity.X = (float) -(300);
			sprite.FlipH = true;
			if(sprite.Animation == "default")
				sprite.Play("move");
		}
		if (Input.IsActionPressed("Right")){
			velocity.X = (float) (300);
			sprite.FlipH = false;
			if(sprite.Animation == "default")
				sprite.Play("move");
		}

		if (Input.IsActionJustPressed("Down"))
		{
			if(sprite.FlipH)
				velocity.X = (float) -(600);
			else
				velocity.X = (float) (600);
			sprite.Play("Slide");
		}

		//This stops them if they let go of a direction
		//The horizontal movement system can use some improvement
		if (Input.IsActionJustReleased("Left") || Input.IsActionJustReleased("Right")) {
			sprite.Play("default");
		}

		if(onGround && velocity.X == 0)
		{
			sprite.Play("default");
		}

		/*
		This is how we do movement
		  Velocity is a variable of CharacterBody2D
		  It's used for MoveAndSlide(), but we can't 
		  edit it directly for some reason...
		  If someone solves this, let me know.
		*/
		Velocity = velocity;
		MoveAndSlide();	
	

		//We presume every loop that they're not on a wall
		//And not on the ground
		//Until proven otherwise
		onGround = false;
		currentWallId = 0;
		//Loop through all of the collisions
		for (int i = 0; i < GetSlideCollisionCount(); i++) 
		{
			//Below gets the collision instance
			KinematicCollision2D collision = GetSlideCollision(i);

			/*
				Now we check what they hit.
				First we check to see if they hit the ground.
				If so, then they can't wall jump, so we don't need
				to check walls.
			*/
			
			if (collision.GetNormal().Y <= -0.8){
				//Reset their jumps
				//Reset it so they can hit walls again
				//Set their vertical velocity to 0
				JumpState = JumpStates.Ground;
				onGround = true;
				velocity.Y = 0;
				onWall = false;
				lastWallId = 0;
				currentWallId = 0;
				break;
			}
			//Since we have no other types of collision yet,
			//This presumes we hit a wall
			else if (lastWallId != collision.GetColliderId() && (collision.GetNormal().X <= -0.8 || collision.GetNormal().X >= 0.8)){
				currentWallId = collision.GetColliderId();
				velocity.Y = 0;
				velocity.X = 0;
				onWall = true;
				//Also, check to see which direction they hit
				if (collision.GetNormal().X >= 0.8){
					wallLeft = true;
				}
				else {
					wallLeft = false;
				}
			}
		}
		
	}
}
