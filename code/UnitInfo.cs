global using Sandbox;
global using System;

public enum UnitType
{
	/// <summary>
	/// Environmental units or resources
	/// </summary>
	[Icon( "check_box_outline_blank" )]
	None,
	/// <summary>
	/// Players
	/// </summary>
	[Icon( "nordic_walking" )]
	Player,
	/// <summary>
	/// Bad Guy
	/// </summary>
	[Icon( "filter_drama" )]
	Badnic
}

[Icon( "psychology" )]
public sealed class UnitInfo : Component
{
	[Property]
	public UnitType Team { get; set; }

	/// <summary>
	/// Max health of the unit, clamps health from 0 to MaxHealth
	/// </summary>
	[Property]
	[Range( 0.1f, 10f, 0.1f )]
	public float MaxHealth { get; set; } = 5f;

	/// <summary>
	/// How many HP are regenerated each second out of combat
	/// </summary>
	[Property]
	[Range( 0f, 2f, 0.1f )]
	public float HealthRegenAmount { get; set; } = 0.5f;

	/// <summary>
	/// How many seconds out of combat before you start regenerating
	/// </summary>
	[Property]
	[Range( 1f, 5f, 1f )]
	public float HealthRegenTimer { get; set; } = 3f;

	/// <summary>
	/// How long to wait before destroying the game object after death
	/// </summary>
	[Property]
	[Range( 0f, 2f, 0.1f )]
	public float DelayDeath { get; set; } = 0f;

	public float Health { get; private set; }

	public bool Alive { get; private set; } = true;

	public event Action<float> OnDamage;
	public event Action OnDeath;

	TimeSince _lastDamage;
	TimeUntil _nextHeal;

	protected override void OnUpdate()
	{
		if ( _lastDamage >= HealthRegenTimer && Health != MaxHealth && Alive )
		{
			if ( _nextHeal )
			{
				Damage( -HealthRegenAmount );
				_nextHeal = 1f;
			}
		}
	}

	protected override void OnStart()
	{
		Health = MaxHealth;
	}

	/// <summary>
	/// Damage the unit, clamped, heal if set to negative
	/// </summary>
	/// <param name="damage"></param>
	public void Damage( float damage )
	{
		if ( !Alive ) return;

		Health = Math.Clamp( Health - damage, 0f, MaxHealth );

		if ( damage > 0 )
			_lastDamage = 0f;

		OnDamage?.Invoke( damage );

		if ( Health <= 0 )
			Krill();
	}

	/// <summary>
	/// Set the HP to 0 and Alive to false, then destroys it
	/// </summary>
	public async void Krill()
	{
		Health = 0f;
		Alive = false;

		OnDeath?.Invoke();

		await Task.DelaySeconds( DelayDeath );

		GameObject.Destroy();
	}
}
