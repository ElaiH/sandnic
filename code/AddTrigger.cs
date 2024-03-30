using Sandbox;

public sealed class AddTrigger : Component, Component.ITriggerListener
{
	[Property]
	public float Amount { get; set; } = 10f;
	public void OnTriggerEnter( Collider other )
	{
		var player = other.Components.Get<SonicSpeedMovement>();
		if ( player != null )
		{
			player.Boost += Amount;
			player.Boost = Math.Clamp( player.Boost, 0, player.MaxBoost );
		}
	}

	public void OnTriggerExit( Collider other )
	{
		
	}

	protected override void OnUpdate()
	{

	}
}
