using Sandbox;
using Sandbox.Citizen;
using Sandbox.VR;

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
	bool momentum;

	public Angles EyeAngles {  get; set; }

	protected override void OnStart()
	{
		movementenable = true;
		Run = MinRun;
	}

	protected override void OnUpdate()
	{
		if ( momentum )
		{
			Run += Time.Delta * 10;
			Speed += Time.Delta * 10;
			cam.FieldOfView += Time.Delta;
			if ( cam.FieldOfView > 130 )
			{
				cam.FieldOfView = 130;
			}
		}

		EyeAngles += Input.AnalogLook;
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );
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
		else if( Input.Released( "Duck" ) )
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
				cam.FieldOfView = 80;
				controller.Acceleration = 5f;
				controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
				Scene.TimeScale = .5f;
			}

			controller.Move();
			if ( Anim != null )
			{
				Anim.IsGrounded = controller.IsOnGround;
				Anim.WithVelocity( controller.Velocity );
			}
		}
	}
}
