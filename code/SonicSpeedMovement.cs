using Sandbox;
using Sandbox.Citizen;
using Sandbox.VR;

public sealed class SonicSpeedMovement : Component
{
	[Property] public float Speed;
	[Property] public float Run;
	[Property] public float CurrentSpeed;
	[Property] public float JumpHighet;
	[Property] public float Spin;
	[Property] public CitizenAnimationHelper Anim;
	[Property] public CharacterController controller;
	[Property] public CameraComponent cam;
	[Property] bool movementenable;

	public Angles EyeAngles {  get; set; }

	protected override void OnStart()
	{
		movementenable = true;
	}

	protected override void OnUpdate()
	{
		EyeAngles += Input.AnalogLook;
		Transform.Rotation = Rotation.FromYaw( EyeAngles.yaw );
		if ( Input.Down( "Duck" ) )
		{
			//animation here
			Anim.TriggerJump();
			movementenable = false;
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
		}

	}

	protected override void OnFixedUpdate()
	{
		if ( movementenable )
		{
			base.OnFixedUpdate();

			if ( controller == null ) return;

			var wishSpeed = Input.Down( "Run" ) ? Run : Speed;
			var wishVelocity = Input.AnalogMove * wishSpeed * Transform.Rotation;

			float ogrunvalue = Run;

			if ( Input.Down( "Run" ) )
			{
				//cam.FieldOfView = 120;
			}
			else if ( Input.Released( "Run" ) )
			{
				cam.FieldOfView = 90;
			}

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
				cam.FieldOfView = 90;
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
