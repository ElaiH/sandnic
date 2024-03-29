using Sandbox;

public sealed class Coin : Component, Component.ITriggerListener
{
	[Property] CoinManager coinMaster;
	[Property] bool roatatearound;
	[Property] bool roatateup;
	[Property] bool roatatePitch;
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		Log.Info( "yipeeee" );
		coinMaster.Coins++;
		GameObject.Destroy();
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{

	}
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if( roatateup )
		{
			Transform.Rotation *= Rotation.FromRoll( Time.Delta * 100 );
		}

		if ( roatatearound )
		{
			Transform.Rotation *= Rotation.FromYaw( Time.Delta * 100 );
		}

		if(roatatePitch )
		{
			Transform.Rotation *= Rotation.FromPitch( Time.Delta * 100 );
		}
	}
}
