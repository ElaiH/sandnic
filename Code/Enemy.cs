using Sandbox;
using System;

public sealed class Enemy : Component, Component.ITriggerListener
{
	[Property] GameObject Coin;
	
	protected override void OnUpdate()
	{

	}

	public void OnTriggerEnter( Collider other )
	{
		other.Components.Get<SonicSpeedMovement>().controller.Punch( Vector3.Up * 1000 );
		other.Components.Get<SonicSpeedMovement>().controller.Punch( Vector3.Backward * 1000 );
		float rings = other.GameObject.Components.Get<SonicSpeedMovement>().Rings;
		if ( rings <= 0 )
		{
			other.GameObject.Destroy();
		}
		else
		{
			other.Components.Get<SonicSpeedMovement>().momentum = false;
			other.Components.Get<SonicSpeedMovement>().Speed = 150;
			other.Components.Get<SonicSpeedMovement>().cam.FieldOfView = 90;
		}
	}

	public void OnTriggerExit( Collider other )
	{
		float rings = other.GameObject.Components.Get<SonicSpeedMovement>().Rings;
		other.GameObject.Components.Get<SonicSpeedMovement>().Rings = 0;
		for ( int j = 0; j < rings; j++ )
		{
			if ( j == rings / 2 ) break;
			GameObject g = Coin.Clone( new Vector3( other.Transform.Position.x + 50, other.Transform.Position.y + 50, 100 ) );
			g.Components.Create<Rigidbody>();
			g.Components.Create<BoxCollider>().Scale = 5;
			g.Components.Get<Rigidbody>().ApplyForce(new Vector3( -50, -50, 0 ));
		}

	}
}
