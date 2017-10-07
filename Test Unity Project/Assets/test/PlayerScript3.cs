using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript3 : MonoBehaviour {

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
	
	private bool run = false; // непрерывное движение
	private bool attack = false; // производиться атака
	private bool running = false; // бег
	private bool jumping = false;
	
	/**
	* 0 - idle (обычная позиция)
	* 1 - run (непрерывное движение)
	* 2 - battle stance (боевая стойка)
	*/
	private int state = 0;
	
	private float speed = 0f;
	private float speedH = 0f;
	private float speedV = 0f;
	
	private float walkSpeed = 2f;
	private float runSpeed = 5f;
	
	private bool PreviouslyGrounded;
	private bool IsGrounded;
	private CapsuleCollider m_Capsule;
	public GameObject sphereObj;
	private SphereCollider sphere;
	private Vector3 moveXZ;
	
    protected string coord(Vector3 v)
    {
        return ((int)v.x).ToString() + ", " + ((int)v.y).ToString() + ", " + ((int)v.z).ToString();
    }
	
    protected void DbgUpdate()
    {
        if (playerDbg)
        {
            playerDbg.text = "v22\n" +
				"PlayerPos: " + coord(transform.position) + "\n" +
				"Run: " + (run ? "on" : "off") + "\n" +
				"Speed: " +  string.Format("{0:0.00}", speed) + "\n" +
				"State: " + state.ToString() + "\n" +
				"IsGrounded: " + (IsGrounded ? "yes" : "no");

        }
    }
	
	/// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
	private void GroundCheck()
	{
		const float gap = 0.05f;
		
		PreviouslyGrounded = IsGrounded;
		RaycastHit hitInfo;
		
		float s_radius = m_Capsule.radius - gap;
		Vector3 s_position = transform.position + Vector3.up * (s_radius + gap);
		
		sphere.radius = s_radius; // debuging
		sphereObj.transform.position = s_position; // debuging
		
		if (Physics.SphereCast(s_position, s_radius, Vector3.down, out hitInfo,
							   2f*gap, Physics.AllLayers, QueryTriggerInteraction.Ignore))
		{
			IsGrounded = true;
			//GroundContactNormal = hitInfo.normal;
		}
		else
		{
			IsGrounded = false;
			//GroundContactNormal = Vector3.up;
		}
		
		if ( PreviouslyGrounded )
		{
			if ( !IsGrounded ) animator.SetBool("Falling", true);
		}
		else
		{
			if ( IsGrounded )
			{
				animator.SetBool("Falling", false);
				jumping = false;
			}
		}
	}
	
	/*
	/// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        //    public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        //    public float stickToGroundHelperDistance = 0.5f; // stops the character
        //    public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
        //    public bool airControl; // can the user control the direction that is being moved in the air
        //    [Tooltip("set it to 0.1 or more if you get stuck in wall")]
        //    public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
	private void GroundCheck()
	{
		m_PreviouslyGrounded = m_IsGrounded;
		RaycastHit hitInfo;
		if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
							   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
		{
			m_IsGrounded = true;
			m_GroundContactNormal = hitInfo.normal;
		}
		else
		{
			m_IsGrounded = false;
			m_GroundContactNormal = Vector3.up;
		}
		if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
		{
			m_Jumping = false;
		}
	}
	*/
	
	
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
		if ( Input.GetButtonDown("Run") )
		{
			run = ! run;
		}
		
		speedH = Input.GetAxis(horizontalInput);
		speedV = Input.GetAxis(verticallInput);
		speed = Mathf.Sqrt(speedH*speedH + speedV*speedV);
		
		if ( speed > 1f )
		{
			speedH /= speed;
			speedV /= speed;
			speed = 1f;
		}
		
		if ( speed > 0.01f )
		{
			run = false;
		}
		
		if ( run )
		{
			speed = speedV = 1f;
			speedH = 0f;
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
	
	protected void handleBattle()
	{
		if ( ! attack )
		{
			animator.SetFloat("speed", 0f);
			animator.SetFloat("speedH", 0f);
			animator.SetFloat("speedV", 0f);
			run = false;
			
			if ( Input.GetKey("w") )
			{
				animator.SetTrigger("ForwardAttack");
				attack = true;
			}
			else if ( Input.GetKey("s") )
			{
				animator.SetTrigger("RollBackward");
				attack = true;
			}
			else if ( Input.GetKey("q") )
			{
				animator.SetTrigger("RollLeft");
				attack = true;
			}
			else if ( Input.GetKey("e") )
			{
				animator.SetTrigger("RollRight");
				attack = true;
			}
			else if ( Input.GetKey("a") )
			{
				animator.SetTrigger("LeftAttack");
				attack = true;
			}
			else if ( Input.GetKey("d") )
			{
				animator.SetTrigger("RightAttack");
				attack = true;
			}
		}
	}
	
	// Use this for initialization
	protected virtual void Start () {
		cursorLocked = false;
		cam = camera.GetComponent<CameraScript>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();
		sphere = sphereObj.GetComponent<SphereCollider>();
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
			if ( state == 0 )
			{
				if ( !jumping )
				{
					moveXZ = transform.forward * speedV + transform.right * speedH;
					if ( Input.GetKeyDown("space") )
					{
						animator.SetTrigger("Jump");
					}
				}
				rb.MovePosition(transform.position + moveXZ * Time.deltaTime);
			}
			
		}
		
		GroundCheck();
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
			if ( state == 2 )
			{
				handleBattle();
				if ( Input.GetMouseButtonUp(0) )
				{
					state = 0;
				}
			}
			else
			{
				handleMovement();
				if ( Input.GetMouseButtonDown(0) )
				{
					state = 2;
				}
			}
			
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
		
		animator.SetInteger("state", state);
    }
	
	protected virtual void LateUpdate()
	{
		DbgUpdate();
	}
	
	public void Hit(float v)
	{
		// ???
	}
	
	public void Jump(float v)
	{
		jumping = true;
		rb.AddForce(0f, 800f, 0f, ForceMode.Impulse);
		//rb.AddForce(0f, 300f, 700f, ForceMode.Impulse);
		//rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
		animator.SetBool("Falling", true);
	}
	
	public void EndAttack(float v)
	{
		attack = false;
	}
}
