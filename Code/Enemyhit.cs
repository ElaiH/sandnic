using Sandbox;

public sealed class Enemyhit : Component, Component.ITriggerListener
{
	protected override void OnUpdate()
	{

	}

	public void OnTriggerEnter( Collider other )
	{
		if(other.Tags.Has("hiting")) 
		{
			GameObject.Parent.Destroy();
			other.GameObject.Parent.Components.Get<CharacterController>().Punch( Vector3.Up * 1000 );
		}
	}

	public void OnTriggerExit( Collider other )
	{

	}
}
