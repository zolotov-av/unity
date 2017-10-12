using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Контроллер управления персонажем в стиле Готики (первой части)
 */
public class GothicController: PlayerController
{
	
	/**
	 * Текстовый блок в UI в который выводиться отладочная информация
	 */
	public Text playerDbg;
	
	/**
	 * Ссылка на контроллер камеры
	 */
	private CameraScript cam;
	
	/**
	 * Ссылка на аниматор персонажа
	 */
	private Animator animator;
	
	/**
	 * Ссылка на Rigidbody (твердое тело)
	 */
	private Rigidbody rb;
	
	private string rotateCameraXInput = "Mouse X";
	private string rotateCameraYInput = "Mouse Y";
	private string horizontalInput = "Horizontal";
	private string verticallInput = "Vertical";
	
	/**
	 * Режим бега
	 *
	 * В этом режиме персонаж всё время бежит вперед, даже если игрок
	 * не нажимает клавиши движения
	 */
	private bool run = false;
	
	/**
	 * Режим ускорения
	 *
	 * В обычном режиме (sprint=false) персонаж бегает/ходит со скоростью
	 * заданной параметром normalSpeed, в режиме ускорения (sprint=true)
	 * персонаж бежит со скоростью заданной параметром sprintSpeed
	 */
	private bool sprint = false;
	
	/**
	 * Состояние атаки
	 */
	private bool attack = false;
	
	/**
	 * Состояние прыжка
	 */
	private bool jumping = false;
	
	/**
	 * Состояние падения
	 */
	private bool falling = false;
	
	/**
	 * 0 - normal (обычная позиция)
	 * 1 - battle stance (боевая стойка)
	 */
	private int state = 0;
	public const int normalState = 0;
	public const int battleState = 1;
	
	/**
	 * Текущая скорость персонажа (с которой персонаж движется в данный момент)
	 */
	private float velocity = 0f;
	
	/**
	 * Вектор скорости персонажа в локальных координатах
	 */
	private Vector3 localVelocity;
	
	/**
	 * Вектор движения персонажа в локальных координатах
	 *
	 * В этой переменной фиксируется скорость (localVelocity) персонажа
	 * в момент падения или прыжка. Пока персонаж прыгает или падает, игрок
	 * не может изменить ни направление ни скорость.
	 */
	private Vector3 moveXZ;
	
	/**
	 * Скрость обычного бега/ходьбы
	 */
	public float normalSpeed = 3f;
	
	/**
	 * Скрость быстрого бега
	 */
	public float sprintSpeed = 5f;
	
	/**
	 * Флаг стоит ли персонаж на земле или падает/прыгает
	 */
	private bool IsGrounded;
	
	/**
	 * Ссылка на коллайдер (капсула)
	 */
	private CapsuleCollider m_Capsule;
	
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
				"Velocity: " +  string.Format("{0:0.00}", velocity) + "\n" +
				"State: " + state.ToString() + "\n" +
				"IsGrounded: " + (IsGrounded ? "yes" : "no");
		}
	}

	/**
	 * Проверка стоит ли персонаж на земле
	 */
	private void GroundCheck()
	{
		bool PreviouslyGrounded = IsGrounded;
		IsGrounded = ControllerUtils.GroundCheck(transform.position, m_Capsule);
		
		if ( PreviouslyGrounded )
		{
			if ( !IsGrounded )
			{
				falling = true;
				animator.SetBool("Falling", true);
			}
		}
		else
		{
			if ( IsGrounded )
			{
				falling = false;
				jumping = false;
				animator.SetBool("Falling", false);
			}
		}
	}
	
	/**
	 * Обработка ввода движения
	 */
	protected void handleMovement()
	{
		// включение режима ускорения
		sprint = Input.GetKey("left shift");
		
		// включение/выключение режима непрерывного бега
		if ( Input.GetButtonDown("Run") )
		{
			run = ! run;
		}
		
		// прочитаем ввод направления движения
		var inputV = Input.GetAxis(verticallInput);
		var inputH = Input.GetAxis(horizontalInput);
		
		// нормализуем длину вектора, чтобы он был не больше заданной скорости
		var speed = sprint ? sprintSpeed : normalSpeed;
		var length = Mathf.Sqrt( inputH * inputH + inputV * inputV );
		if ( length > 0.01f )
		{
			velocity = speed * Mathf.Clamp(length, 0f, 1f);
			var scale = velocity / length;
			localVelocity.x = inputH * scale;
			localVelocity.y = 0f;
			localVelocity.z = inputV * scale;
			
			// если игрок нажал клавиши движения, то сбрасываем режим бега
			run = false;
		}
		else
		{
			if ( run )
			{
				velocity = speed;
				localVelocity.x = 0f;
				localVelocity.y = 0f;
				localVelocity.z = speed;
			}
			else
			{
				velocity = 0f;
				localVelocity.x = 0f;
				localVelocity.y = 0f;
				localVelocity.z = 0f;
			}
		}
		
		//
		if ( !falling && !jumping )
		{
			if ( Input.GetKey("space") )
			{
				jumping = true;
				animator.SetTrigger("Jump");
			}
		}
		
		animator.SetFloat("speed", velocity);
	}
	
	/**
	 * Обработка ввода в режиме битвы
	 */
	protected void handleBattle()
	{
		// если прошлая атакая еще не закончилась или если мы падаем,
		// то мы не можем начать новую атаку
		if ( attack || falling )
		{
			return;
		}
		
		velocity = 0f;
		localVelocity.x = 0f;
		localVelocity.y = 0f;
		localVelocity.z = 0f;
		
		if ( Input.GetKey("w") )
		{
			animator.SetTrigger("ForwardAttack");
			attack = true;
			return;
		}
		
		if ( Input.GetKey("s") )
		{
			animator.SetTrigger("RollBackward");
			attack = true;
			return;
		}
		
		if ( Input.GetKey("q") )
		{
			animator.SetTrigger("RollLeft");
			attack = true;
			return;
		}
		
		if ( Input.GetKey("e") )
		{
			animator.SetTrigger("RollRight");
			attack = true;
			return;
		}
		
		if ( Input.GetKey("a") )
		{
			animator.SetTrigger("LeftAttack");
			attack = true;
			return;
		}
		
		if ( Input.GetKey("d") )
		{
			animator.SetTrigger("RightAttack");
			attack = true;
			return;
		}
	}
	
	/**
	 * Обработка вращения персонажа/камеры
	 */
	protected void handleRotation()
	{
		if ( cursorLocked )
		{
			var rotateDelta = Input.GetAxis(rotateCameraXInput);
			var angleDelta = Input.GetAxis(rotateCameraYInput);
			var distanceDelta = Input.GetAxis("Camera Distance");
			
			transform.Rotate(Vector3.up * rotateDelta * 3);
			cam.UpdateOptions(distanceDelta, angleDelta);
			cam.rotation = transform.rotation;
		}
	}
	
	/**
	 * Обработка ввода
	 */
	protected void handleInput()
	{
		// Нажатие Escape возвращает нас в режим UI (пользовательского
		// интерфейса). При этом, в любом состоянии (на случай если что-то
		// заглючит) при нажатии Escape также все параметры сбрасываются
		// в некоторое начальное состояние, дальнейшая обработка ввода
		// прерывается до тех пор пока пользователь снова явно не захватит
		// курсор левой кнопкой мыши
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			lockCursor(false);
			state = normalState;
			
			// прерываем вращение камеры
			cam.UpdateOptions(0f, 0f);
			return;
		}
		
		if ( ! cursorLocked )
		{
			// если курсор не захвачен, то захват активируется нажатием левой
			// кнопки мыши
			if ( Input.GetMouseButtonDown(0) )
			{
				lockCursor(true);
				state = normalState;
				handleRotation();
				handleMovement();
				return;
			}
			else
			{
				// если курсор не захвачен весь прочий ввод игнорируется
				return;
			}
		}
		
		if ( state == normalState )
		{
			if ( Input.GetMouseButtonDown(0) )
			{
				run = false;
				state = battleState;
				handleRotation();
				handleBattle();
				return;
			}
			
			handleRotation();
			handleMovement();
			return;
		}
		
		if ( state == battleState )
		{
			if ( Input.GetMouseButtonUp(0) )
			{
				state = normalState;
				run = false;
				handleRotation();
				handleMovement();
				return;
			}
			
			handleRotation();
			handleBattle();
			return;
		}
	}
	
	void Start()
	{
		cursorLocked = false;
		cam = playerCamera.GetComponent<CameraScript>();
		cam.rotation = transform.rotation;
		
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();
		
		if ( !playerDbg )
		{
			var obj = GameObject.FindWithTag("PlayerDebug");
			if ( obj ) playerDbg = obj.GetComponent<Text>();
		}
	}
	
	void FixedUpdate()
	{
		GroundCheck();
		
		if ( !falling && !jumping )
		{
			moveXZ = transform.TransformDirection(localVelocity);
		}
		
		rb.MovePosition(transform.position + moveXZ * Time.deltaTime);
	}
	
	void Update ()
	{
		handleInput();
		
		animator.SetInteger("state", state);
	}
	
	void LateUpdate()
	{
		DbgUpdate();
	}
	
	public void Hit(float v)
	{
		// ???
	}
	
	public void Jump(float v)
	{
		rb.AddForce(0f, 230f, 0f, ForceMode.Impulse);
		//rb.AddForce(0f, 800f, 0f, ForceMode.Impulse);
	}
	
	public void EndAttack(float v)
	{
		attack = false;
	}
	
} // class GothicController

} // namespace Nanosoft
