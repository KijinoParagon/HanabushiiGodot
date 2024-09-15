using Godot;
using System;
using System.Linq;

public partial class AkioChar : CharacterBody2D
{

	[Export]
	public int speed {get; set;}
	[Export]
	public int gravity {get; set;}

	[Export]
	public int jumpVelocity {get; set;}
	[Export]
	public float maxJumpTime = 1;
	public float jumpTime = 0;
	//Vector2 velocity = Vector2.Zero;
	AnimatedSprite2D sprite;

	Vector2 velocity;
	bool moving = false;
	bool faceLeft = false;
	bool run = false;
	int runSpeed = 1;
	
	string state = "Idle";
	int jump = 0;

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
		if (Input.IsActionJustPressed("Left")){
			velocity.X -= speed;
			moving = true;
			faceLeft = true;
		}

		if (Input.IsActionJustPressed("Right")){
			velocity.X += speed;
			moving = true;
			faceLeft = false;
		}
		

		if (Input.IsActionJustPressed("Down")){
			//
		}
		if (Input.IsActionJustReleased("Left")){
			velocity.X += speed;
			moving = false;
		}

		if (Input.IsActionJustReleased("Right")){
			velocity.X -= speed;
			moving = false;
		}
		if (Input.IsActionJustReleased("Up") && jump%2 == 0){
			jump--;
		}

		if (Input.IsActionJustReleased("Down")){
			//
		}

		if (Input.IsActionPressed("Run"))
		{
			run = true;
			runSpeed = 2;
		}
		else {
			run = false;
			runSpeed = 1;
		}

		if (!moving)
		{
			sprite.Animation = "default";
		}
		else {
			sprite.Animation = "move";
			if(run)
				sprite.SpeedScale = 2;
			else
				sprite.SpeedScale = 1;
		}

		if(!IsOnFloor())
		{
			velocity.Y += gravity * (float) delta;
		}
		else {
			jump = 0;
			jumpTime = 0;
		}
		//We need to invert it for diminishing jump returns.
		if (Input.IsActionPressed("Up") && jump <= 2 && jumpTime < maxJumpTime){
			velocity.Y -= jumpVelocity * (float) delta * 500;
			GD.Print("Jumping");
			jumpTime += (float) delta;
		}
		if(Input.IsActionJustReleased("Up")){
			jump+=2;
			jumpTime = 0;
		}
		sprite.FlipH = faceLeft;
		//var col = MoveAndCollide(velocity).GetCollider();
		Velocity = velocity;
		MoveAndSlide();
		//var colM = col.GetMeta("Terrain");
		//Position += velocity * runSpeed;
		
			
			
		

		
	}
}
