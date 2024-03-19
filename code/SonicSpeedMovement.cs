using Sandbox;
using Sandbox.Citizen;
using Sandbox.VR;
using System.Runtime;

public sealed class SonicSpeedMovement : Component
{
	[Property] public float Speed;
	[Property] public float MinSpeed;
	[Property] public float Run;
	[Property] public float MinRun;
	[Property] public float JumpHighet;
	[Property] public float Spin;
	[Property] public CitizenAnimationHelper Anim;
	[Property] public CharacterController controller;
	[Property] public CameraComponent cam;
	[Property] bool movementenable;
	[Property]public float PunchStrength { get; set; } = 1f;
	//[Property]public float PunchCooldown { get; set; } = 0.5f;
	[Property]public float PunchRange { get; set; } = 50f;
	[Property]
	public Vector3 EyePosition { get; set; }
	TimeSince _lastPunch;

	public Vector3 EyeWorldPosition => Transform.Local.PointToWorld( EyePosition );
	bool momentum;

	//CamStuff
	[Property] public bool FirstPerson { get; set; }
	[Sync]
	public Angles EyeAngles {  get; set; }
	[Property] public GameObject Eye { get; set; }
	[Property] public GameObject Body { get; set; }

	protected override void OnStart()
	{
		movementenable = true;
		Run = MinRun;
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();

		if ( IsProxy )
			return;

		var cam = Scene.GetAllComponents<CameraComponent>().FirstOrDefault();
		if ( cam is not null )
		{
			var ee = cam.Transform.Rotation.Angles();
			ee.roll = 0;
			EyeAngles = ee;
		}
	}

	protected override void DrawGizmos()
	{
		if ( !Gizmo.IsSelected ) return;

		var draw = Gizmo.Draw;

		draw.LineSphere( EyePosition, 10f );
		draw.LineCylinder( EyePosition, EyePosition + Transform.Rotation.Forward * PunchRange, 5f, 5f, 10 );
	}
	protected override void OnUpdate()
	{
		EyeAngles += Input.AnalogMove;
		EyeAngles += Input.AnalogLook;
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );
		cam.Transform.Position = new Vector3( Transform.Position.x + - 200, Transform.Position.y + 30, Transform.Position.z + 85 );

		if ( momentum )
		{
			Run += Time.Delta * 50;
			Speed += Time.Delta * 50;
			cam.FieldOfView += Time.Delta;
			if ( cam.FieldOfView > 130 )
			{
				cam.FieldOfView = 130;
			}
		}
		
		if ( controller.IsOnGround )
		{
			if ( Input.Down( "Duck" ) )
			{
				//animation here
				Anim.TriggerJump();
				movementenable = false;
				cam.FieldOfView -= Time.Delta * 10;
				if ( cam.FieldOfView < 30 )
				{
					cam.FieldOfView = 30;
				}
			}
			else if ( Input.Released( "Duck" ) )
			{
				//cam.FieldOfView = 120;
				if ( controller == null ) return;

				var wishVelocity = Vector3.Forward * Spin * Transform.Rotation;

				controller.Accelerate( wishVelocity );

				controller.Move();
				Anim.TriggerJump();

				movementenable = true;
				cam.FieldOfView = 90;
				Run += 300;
				Speed += 400;
			}
		}
		var punchTrace = Scene.Trace
			.FromTo( EyeWorldPosition, EyeWorldPosition + EyeAngles.Forward * PunchRange )
			.Size( 10f )
			.WithoutTags( "player" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( punchTrace.Hit )
			if ( punchTrace.GameObject.Components.TryGet<UnitInfo>( out var unitInfo ) )
			{
				Log.Info( "Hit" );
				Scene.TimeScale = 0.1f;
			}

	}

	protected override void OnFixedUpdate()
	{
		if ( movementenable )
		{

			base.OnFixedUpdate();

			if ( controller == null ) return;

			var wishSpeed = Input.Down( "Run" ) ? Run : Speed;
			if( Input.AnalogMove != 0 )
			{
				momentum = true;
			}
			else
			{
				momentum = false;
				Run = MinRun;
				Speed = MinSpeed;
				cam.FieldOfView = 90;
			}
			if(Input.Pressed("Run"))
			{
				cam.FieldOfView += 10;
			}
			else if ( Input.Released( "Run" ) )
			{
				cam.FieldOfView -= 10;
			}
			var wishVelocity = Input.AnalogMove * wishSpeed * Transform.Rotation;

			float ogrunvalue = Run;

			controller.Accelerate( wishVelocity );


			if ( controller.IsOnGround )
			{
				controller.Acceleration = 10f;
				controller.ApplyFriction( 5f, 20f );

				if ( Input.Pressed( "Jump" ) )
				{
					controller.Punch( Vector3.Up * JumpHighet );

					if ( Anim != null )
						Anim.TriggerJump();
				}
				Scene.TimeScale = 1f;
			}
			else
			{
				controller.Acceleration = 5f;
				controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
				Scene.TimeScale = 0.7f;
			}

			controller.Move();
			if ( Anim != null )
			{
				Anim.IsGrounded = controller.IsOnGround;
				Anim.WithVelocity( controller.Velocity );

				if ( _lastPunch >= 2f )
					Anim.HoldType = CitizenAnimationHelper.HoldTypes.None;
			}
			if ( Input.Pressed( "Punch" )) //&& _lastPunch >= PunchCooldown )
				Punch();
		}
	}
	public void Punch()
	{
		if ( controller.IsOnGround )
		{
			if ( Anim != null )
			{
				Anim.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
				Anim.Target.Set( "b_attack", true );
			}
		}

		var punchTrace = Scene.Trace
			.FromTo( EyeWorldPosition, EyeWorldPosition + EyeAngles.Forward * PunchRange )
			.Size( 10f )
			.WithoutTags( "player" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( punchTrace.Hit )
			if ( punchTrace.GameObject.Components.TryGet<UnitInfo>( out var unitInfo ) )
			{
				unitInfo.Damage( PunchStrength );
				Log.Info( "Hit" );
				GameObject.Transform.Position = punchTrace.GameObject.Transform.Position;
				var wishVelocity = Vector3.Up * 800;

				controller.Accelerate( wishVelocity );
				var gg = Vector3.Forward * 700;

				controller.Accelerate( gg );
			}

		_lastPunch = 0f;
	}
}
