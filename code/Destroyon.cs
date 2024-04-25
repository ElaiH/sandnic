using Sandbox;

public sealed class Destroyon : Component
{
	[Property] float time;
	protected override void OnUpdate()
	{
		time += -100 * Time.Delta;
		Log.Info( time );
		if(time < 1)
		{
			GameObject.Destroy();
		}
	}
}
