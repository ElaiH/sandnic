using Sandbox;

public sealed class Tryingstuffandimdead : Component
{
	[Property] public Rigidbody rb;

	[Property] float gravity;

	public Vector3 vert => Vector3.VectorPlaneProject( rb.Velocity, rb.Transform.Position.z );
	protected override void OnUpdate()
	{
		if ( groundd )
		{
			if ( Input.Pressed( "Jump" ) )
				jump();
		}
	}

	[Property] float jumpforce;
	void jump()
	{
		rb.Velocity = Vector3.Up * jumpforce;
		Move();
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
		Gravity();
		ground();
	}

	void Gravity()
	{
		rb.Velocity -= Vector3.Up * gravity * Time.Delta;
	}

	[Property] float grounddistance;

	bool groundd;

	void ground()
	{
		var hit = Scene.Trace
			.FromTo( Transform.Position, Vector3.Down * grounddistance )
			.Size( 10f )
			.WithoutTags( "player" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( hit.Distance > 0 )
		{
			groundd = false;
		}
		if ( hit.Distance <= 0 )
		{
			groundd = true;
		}
	}

	[Property] float speed = 250;
	void Move()
	{
		rb.Velocity = (Vector3.Right * Input.AnalogMove* speed) + (Vector3.Left * Input.AnalogMove * speed) + (Vector3.Forward * Input.AnalogMove * speed) + (Vector3.Backward * Input.AnalogMove * speed)
			*vert;
	}
}
