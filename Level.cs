using Godot;
using System;
using System.Linq;

public partial class Level : Node2D
{
	[Export]
	public int Gravity {get; set;} = 1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var Nodes = GetChildren().Where(node => node is Area2D);
		foreach(Area2D n in Nodes)
		{
			GD.Print(n.Position);
		}
	}
}
