using Sandbox;

public sealed class Aligned : Component
{
	[Property]
	public Rigidbody rb;
	protected override void OnUpdate()
	{
		aligned();
	}
	protected override void OnFixedUpdate()
	{
		Vector3 move  = new Vector3 (Input.AnalogMove * 10000);
		Vector3 cmove  = new Vector3 (-rb.Velocity.x, 0, -rb.Velocity.z);

		rb.ApplyForce(move * 1000);
		rb.ApplyForce(cmove * 1000);
	}
	public void aligned()
	{
		var hit = Scene.Trace
			.FromTo( Transform.Position + 10, -Vector3.Up )
			.Size( 50f )
			.WithoutTags( "player" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();
		if ( hit.Hit )
		{
			Log.Info( hit.Normal );
			Transform.Rotation = new Angles( hit.Normal );
		}
	}
}
