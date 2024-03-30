using Sandbox;

public sealed class LookAround : Component
{
	// Properties
    [Property] public GameObject Body { get; set; }
    [Property] public GameObject Head { get; set; }
    [Property] public float Distance { get; set; } = 0f;

    // Variables
    public bool IsFirstPerson => Distance == 0f; // Helpful but not required. You could always just check if Distance == 0f
    private CameraComponent Camera;
    private ModelRenderer BodyRenderer;

    protected override void OnAwake()
    {
        Camera = Components.Get<CameraComponent>();
    }

	protected override void OnUpdate()
    {
        // Rotate the head based on mouse movement
        var eyeAngles = Head.Transform.Rotation.Angles();
        eyeAngles.pitch += Input.MouseDelta.y * 0.1f;
        eyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
        eyeAngles.roll = 0f;
        eyeAngles.pitch = eyeAngles.pitch.Clamp( -80f, 80f ); // So we don't break our necks
        Head.Transform.Rotation = eyeAngles.ToRotation();

        // Set the position of the camera
        if(Camera is not null)
        {
            var camPos = Head.Transform.Position;
            if(!IsFirstPerson)
            {
                // Perform a trace backwards to see where we can safely place the camera
                var camForward = eyeAngles.ToRotation().Forward;
                var camTrace = Scene.Trace.Ray(camPos, camPos - (camForward * Distance))
                    .WithoutTags("player", "trigger")
                    .Run();
                if(camTrace.Hit)
                {
                    camPos = camTrace.HitPosition + camTrace.Normal;
                }
                else
                {
                    camPos = camTrace.EndPosition;
                }
            }
            

            // Set the position of the camera to our calculated position
            Camera.Transform.Position = camPos;
            Camera.Transform.Rotation = eyeAngles.ToRotation();
        }
    }
}