using Sandbox;

public sealed class SpinAround : Component
{
	[Property] public Angles SpinAngles { get; set; }

	protected override void OnUpdate()
	{
		if ( IsProxy ) return;

		Transform.LocalRotation *= (SpinAngles * Time.Delta).ToRotation();
	}
}