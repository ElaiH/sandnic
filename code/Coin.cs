using Sandbox;

public sealed class Coin : Component, Component.ITriggerListener
{
	[Property] CoinManager coinMaster;
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		Log.Info( "yipeeee" );
		coinMaster.Coins++;
		GameObject.Destroy();
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{

	}
}
