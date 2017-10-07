using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

	public GameObject camera;
	
    public Text playerDbg;
	
	private CameraScript cam;
	private Animator animator;
	private Rigidbody rb;
    
	private bool cursorLocked;
    private string rotateCameraXInput = "Mouse X";
    private string rotateCameraYInput = "Mouse Y";
	private string horizontalInput = "Horizontal";
	private string verticallInput = "Vertical";
	
	private bool running = false;
	private float speed = 0f;
	private float speedH = 0f;
	private float speedV = 0f;
	
	private float walkSpeed = 2f;
	private float runSpeed = 5f;
	
	
    protected string coord(Vector3 v)
    {
        return ((int)v.x).ToString() + ", " + ((int)v.y).ToString() + ", " + ((int)v.z).ToString();
    }
	
    protected void DbgUpdate()
    {
        if (playerDbg)
        {
            playerDbg.text = "v18\n" +
				"PlayerPos: " + coord(transform.position);
        }
    }
	
	public void lockCursor(bool state)
	{
		if ( state )
		{
			if ( ! cursorLocked )
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				cursorLocked = true;
			}
		}
		else
		{
			if ( cursorLocked )
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				cursorLocked = false;
			}
		}
	}
	
	public void UpdateOptions(float rotateDelta)
	{
		transform.Rotate(Vector3.up * rotateDelta * 3);
	}
	
	protected void handleMovement()
	{
		speedH = Input.GetAxis(horizontalInput);
		speedV = Input.GetAxis(verticallInput);
		
		speed = Mathf.Sqrt(speedH*speedH + speedV*speedV);
		if ( speed > 1f )
		{
			speedH /= speed;
			speedV /= speed;
			speed = 1f;
		}
		
		if ( running )
		{
			speed *= runSpeed;
			speedH *= runSpeed;
			speedV *= runSpeed;
		}
		else
		{
			speed *= walkSpeed;
			speedH *= walkSpeed;
			speedV *= walkSpeed;
		}
		
		animator.SetFloat("speed", speed);
		animator.SetFloat("speedH", speedH);
		animator.SetFloat("speedV", speedV);
	}
	
	// Use this for initialization
	protected virtual void Start () {
		cursorLocked = false;
		cam = camera.GetComponent<CameraScript>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}
	
    protected virtual void FixedUpdate()
	{
		if ( cursorLocked )
		{
			var rotateDelta = Input.GetAxis(rotateCameraXInput);
			var angleDelta = Input.GetAxis(rotateCameraYInput);
			var distanceDelta = Input.GetAxis("Camera Distance");
			
			UpdateOptions(rotateDelta);
			cam.UpdateOptions(distanceDelta, angleDelta);
			Vector3 dir = transform.forward * speedV + transform.right * speedH;
			rb.MovePosition(transform.position + dir * Time.deltaTime);
		}
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
		if ( ! cursorLocked )
		{
			if ( Input.GetMouseButtonDown(0) )
			{
				lockCursor(true);
			}
		}
		else
		{
			handleMovement();
			
			if ( Input.GetKeyDown("1") )
			{
				animator.SetTrigger("Greet01");
			}
			
			if ( Input.GetKeyDown("2") )
			{
				animator.SetTrigger("Action1");
			}
			
			running = Input.GetKey("left shift");
			
			if ( Input.GetKeyDown(KeyCode.Escape) )
			{
				lockCursor(false);
			}
		}
    }
	
	protected virtual void LateUpdate()
	{
		DbgUpdate();
	}
}
