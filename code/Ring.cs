using Sandbox;

public sealed class Ring : Component, Component.ITriggerListener
{
	[Property]
	public int RingAmount { get; set; } = 1;
	[Property]
	public float BoostAmount { get; set; } = 10f;

	[Property] public Angles SpinAngles { get; set; }

	protected override void OnUpdate()
	{
		if ( IsProxy ) return;

		Transform.LocalRotation *= (SpinAngles * Time.Delta).ToRotation();
	}

	public void OnTriggerEnter( Collider other )
	{
		var player = other.Components.Get<SonicSpeedMovement>();
		if ( player != null )
		{
			player.Rings += RingAmount;
			player.Rings = Math.Clamp( player.Rings, 0, player.MaxRings );
			player.Boost += BoostAmount;
			player.Boost = Math.Clamp( player.Boost, 0, player.MaxBoost );
			GameObject.Destroy();
		}
	}

	public void OnTriggerExit( Collider other )
	{

	}

}
