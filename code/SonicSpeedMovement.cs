using Sandbox;
using Sandbox.Citizen;
using Sandbox.VR;
using System.Diagnostics;
using System.Numerics;
using System.Runtime;
using static Sandbox.Utility.DataProgress;

public sealed class SonicSpeedMovement : Component, Component.ICollisionListener
{
	//Variables
	public bool Jump = false;

	[Property]
	public GameObject runparticale;
	[Property]
	public GameObject boomparticale;

	[Property]
	[Category("Variables")]
	public int Rings { get; set; } = 0;
	[Property]
	[Category( "Variables" )]
	public int MaxRings { get; set; } = 999;
	[Property]
	[Category( "Variables" )]
	public float Boost { get; set; } = 0f;
	[Property]
	[Category( "Variables" )]
	public float MaxBoost { get; set; } = 100f;
	public TimeSince TimeAlive { get; set; } = 0f;
	[Category( "Stats" )]
	[Property] public float Speed = 200;

	[Category( "Stats" )]
	[Property] public float MinSpeed = 200;

	[Category( "Stats" )]
	[Property] public float JumpHighet = 900;

	[Category( "Stats" )]
	[Property] public float Spin = 300000;

	[Category( "Components" )]
	[Property] public CitizenAnimationHelper Anim;

	[Category( "Components" )]
	[Property] public CharacterController controller;

	[Category( "Components" )]
	[Property] public CameraComponent cam;

	[Category( "IguessDebug" )]
	[Property] bool movementenable;

	[Category( "IguessDebug" )]
	[Property] public float mcqueen;

	Vector3 physic;

	[Category( "Stats" )]
	[Property]public float PunchStrength { get; set; } = 1f;
	//[Property]public float PunchCooldown { get; set; } = 0.5f;

	[Category( "Stats" )]
	[Property]public float PunchRange { get; set; } = 50f;
	[Property]public Vector3 EyePosition { get; set; }
	TimeSince _lastPunch;
	Transform _camthing;
	public float rotate;

	public Vector3 EyeWorldPosition => Transform.Local.PointToWorld( EyePosition );
	bool momentum;

	//CamStuff
	[Sync]
	public Angles EyeAngles {  get; set; }

	protected override void OnStart()
	{
		movementenable = true;
		_camthing = cam.Transform.Local;
	}

	public void aligned()
	{
		var hit = Scene.Trace
			.FromTo( Transform.Position, Vector3.Down)
			.Size( 10f )
			.WithoutTags( "player" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		if ( hit.Hit )
		{

			//if ( controller.IsOnGround )
			//{
			Log.Info( hit.Normal );
			Transform.Rotation = Quaternion.Lerp( Transform.Rotation, Quaternion.CreateFromYawPitchRoll(hit.Normal.z, 0, EyeAngles.yaw / 50 ), 1f );
			//if ( hit.Distance <= 1200 )
			//{
			//Scene.PhysicsWorld.Gravity = new Vector3( 0, 0, -850 );
			//controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
			//Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );
			//}
			//else if ( hit.Distance >= 1200 )
			//{

			//Scene.PhysicsWorld.Gravity = new Vector3( 0, 0, -850 );

			//controller.Velocity = hit.Distance / 100 + Scene.PhysicsWorld.Gravity;

			//controller.IsOnGround = true;
			//Anim.IsGrounded = true;
			//controller.Velocity += Scene.PhysicsWorld.Gravity * Transform.Rotation / 100;
			//if (hit.Direction.x != 0)
			//{

			//controller.Velocity = hit.Direction * Vector3.Forward;


			//controller.IsOnGround = true;
			//Anim.IsGrounded = true;


			/*if ( !controller.IsOnGround )
			{
				physic = new Vector3( 0, 0, -850 );
				controller.Velocity += physic * Time.Delta;
				Transform.Rotation = Quaternion.Lerp( Transform.Rotation, Quaternion.CreateFromYawPitchRoll( 0, 0, EyeAngles.yaw / 50  ), 1f );
			}
			else
			{
				//physic = new Vector3( Transform.Rotation.Pitch(), Transform.Rotation.Roll(), Transform.Rotation.Yaw() );
				controller.Velocity += Scene.PhysicsWorld.Gravity * Transform.Rotation / 10;
				Transform.Rotation = Quaternion.Lerp( Transform.Rotation, Quaternion.CreateFromYawPitchRoll( hit.Direction.x, hit.Direction.y, EyeAngles.yaw / 50), 1f );
				controller.Velocity += physic * Transform.Rotation / 100;
			}

			//if (controller.IsOnGround)
			//{
			//	//controller.Velocity += Scene.PhysicsWorld.Gravity * Transform.Rotation / 10;
			//	controller.Acceleration = 0;
			//	var a = Input.AnalogMove * mcqueen * controller.Velocity;
			//	controller.Accelerate( a );
			//	Scene.PhysicsWorld.Step( 5 );
			//}
			//else
			//{
			//	Scene.PhysicsWorld.Gravity = new Vector3( 0, 0, -850 );
			//}
			//controller.Velocity = hit.Distance / 100 + Scene.PhysicsWorld.Gravity;
			//Scene.PhysicsWorld.Gravity *= Transform.Rotation;
			//}
			/*else
			{
				controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
				Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );
				EyeAngles += Input.AnalogMove;
				/*EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, -80f, 80f ) );
				//Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );

				if ( cam != null )
				{
					var cameraTransform = _camthing.RotateAround( EyePosition, EyeAngles.WithYaw( 0f ) );
					var cameraPosition = Transform.Local.PointToWorld( cameraTransform.Position );
					var cameraTrace = Scene.Trace.Ray( EyeWorldPosition, cameraPosition )
						.Size( 5f )
						.IgnoreGameObjectHierarchy( GameObject )
						.WithoutTags( "player" )
						.Run();

					cam.Transform.Position = cameraTrace.EndPosition;
					cam.Transform.LocalRotation = cameraTransform.Rotation;
				}
				*/
			//}
			//}

			//}

		}
		else
		{
			Scene.PhysicsWorld.Gravity = 10;
		}	
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
		//aligned();
		//if(mcqueen > 1000)
		//{
		//Log.Info( Scene.PhysicsWorld.Gravity );
		//}
		//Log.Info( Scene.PhysicsWorld.Gravity);
		//cam.Transform.Local = _camthing.RotateAround( EyePosition, EyeAngles.WithYaw(0f) );

		//cam.Transform.Position = new Vector3( Transform.Position.x - 180, Transform.Position.y, Transform.Position.z + 85 );

		//EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, -80f, 80f ) );
		//Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );

		/*if ( cam != null )
		{
			var cameraTransform = _camthing.RotateAround( EyePosition, EyeAngles.WithYaw( 0f ) );
			var cameraPosition = Transform.Local.PointToWorld( cameraTransform.Position );
			var cameraTrace = Scene.Trace.Ray( EyeWorldPosition, cameraPosition )
				.Size( 5f )
				.IgnoreGameObjectHierarchy( GameObject )
				.WithoutTags( "player" )
				.Run();

			cam.Transform.Position = cameraTrace.EndPosition;
			cam.Transform.LocalRotation = cameraTransform.Rotation;
		}
		*/
		if (controller.IsOnGround && mcqueen > 255 )
		{
		EyeAngles += Input.AnalogMove;
		EyeAngles += Input.AnalogLook;
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw);
		}
		else
		{
		EyeAngles += Input.AnalogLook;
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw);
		//cam.Transform.Local = _camthing.RotateAround( EyePosition, EyeAngles.WithRoll( 0f ) );
		}
		//aligned();

		if ( mcqueen >= 1500 )
		{
			runparticale.Clone( GameObject.Transform.Position, GameObject.Transform.Rotation );
		}
		else
		{
			//GameObject.Transform.Rotation = Rotation.FromYaw(EyeAngles.yaw);
		}
		if ( momentum )
		{
			Speed += Time.Delta * 50;
			cam.FieldOfView += mcqueen / 100000;
			if ( cam.FieldOfView > 140 )
			{
				cam.FieldOfView = 140;
			}
		}
		Boost -= Time.Delta * 10;
		if(Boost < 20)
		{
			Boost = 20;
		}
		if ( controller.IsOnGround )
		{
			float fov = cam.FieldOfView;
			controller.Acceleration = 10f;
			controller.ApplyFriction( 5f );
			if ( Input.Down( "Duck" ) )
			{
				//animation here
				Anim.Components.Get<SkinnedModelRenderer>().Set( "b_ball", true );
				movementenable = false;
				fov = cam.FieldOfView;
				cam.FieldOfView -= Time.Delta * 10;
				if ( cam.FieldOfView < 30 )
				{
					cam.FieldOfView = 30;
				}
			}
			else if ( Input.Released( "Duck" ) )
			{
				cam.FieldOfView = fov;

				if ( controller == null ) return;

				var wishVelocity = Vector3.Forward * Spin * Transform.Rotation;

				controller.Accelerate( wishVelocity );

				controller.Move();

				Anim.Components.Get<SkinnedModelRenderer>().Set( "b_ball", false );

				movementenable = true;
				cam.FieldOfView = 90;
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
		{
			if ( punchTrace.GameObject.Components.TryGet<UnitInfo>( out var unitInfo ) )
			{
				Log.Info( "Hit" );
				Scene.TimeScale = 0.1f;
			}
		}
		else
		{
			//Scene.TimeScale = 1f;
		}

	}

	protected override void OnFixedUpdate()
	{
		if ( movementenable )
		{

			base.OnFixedUpdate();

			if ( controller == null ) return;

			var wishSpeed = Input.Down( "Run" ) ? Speed : Speed;
			if ( wishSpeed > 2000 )
			{
				wishSpeed = 2000;
				//Scene.TimeScale -= Time.Delta / 10;
				//if(Scene.TimeScale < 0.4f)
				//{
				//	Scene.TimeScale = 0.4f;
				//}
			}
			else
			{
				Scene.TimeScale = 1;
			}
			if (wishSpeed > 900)
			{
				JumpHighet = 500;
			}
			else if(wishSpeed < 900)
			{
				JumpHighet = 900;
			}
			if( Input.AnalogMove != 0 )
			{
				momentum = true;
			}
			else
			{
				momentum = false;
				Speed = MinSpeed;
				cam.FieldOfView = 90;
			}
			if(Input.Pressed("Run"))
			{
				Boosting();
			}
			var wishVelocity = Input.AnalogMove * wishSpeed * Transform.Rotation;

			controller.Accelerate( wishVelocity );

			mcqueen = wishSpeed;

			if ( controller.IsOnGround )
			{
				Jump = false;
				controller.Acceleration = 10f;
				controller.ApplyFriction( 5f, 20f );

				if ( Input.Pressed( "Jump" ) )
				{
					controller.Punch( Vector3.Up * JumpHighet );

					if ( Anim != null )
						Anim.TriggerJump();
				}
			}
			else
			{
				if ( Jump == false )
				{
					if ( Input.Pressed( "Jump" ) )
					{
						controller.Punch( Vector3.Up * JumpHighet );
						Jump = true;
					}
				}
				//Scene.TimeScale = 0.7f;
				controller.Acceleration = 5f;
				controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;

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
	public void Boosting()
	{
		Speed += Boost * 10;
		cam.FieldOfView += Boost;
		boomparticale.Clone( GameObject.Transform.Position );
	}
}
