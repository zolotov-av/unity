using UnityEngine;
using UnityEngine.AI;

namespace Nanosoft
{

/**
 * Базовый класс для всех персонажей
 */
public class Character: MonoBehaviour
{

	/**
	 * Ссылка на Rigidbody (твердое тело)
	 */
	protected Rigidbody rb;
	
	/**
	 * Ссылка на аниматор персонажа
	 */
	protected Animator animator;
	
	/**
	 * Ссылка на коллайдер персонажа
	 */
	protected CapsuleCollider capsule;
	
	/**
	 * Ссылка на NavMeshAgent персонажа
	 */
	protected NavMeshAgent navAgent;
	
	/**
	 * Флаг смерти персонажа
	 */
	protected bool dead = false;
	
	/**
	 * Текущее здоровье персонажа
	 */
	protected int currentHealth = 0;
	
	/**
	 * Максимально возможное здоровье персонажа
	 */
	public int maxHealth = 100;
	
	/**
	 * Стоит ли персонаж на земле?
	 * TODO решить вопрос с видимостью (public/protected/private?)
	 */
	[HideInInspector]
	public bool grounded = false;
	
	/**
	 * Состояние прыжка (летит вверх)
	 * TODO решить вопрос с видимостью (public/protected/private?)
	 */
	[HideInInspector]
	public bool jumping = false;
	
	/**
	 * Состояние падения (падает вниз)
	 * TODO решить вопрос с видимостью (public/protected/private?)
	 */
	[HideInInspector]
	public bool falling = false;
	
	/**
	 * Флаг навигации (движения по клику)
	 */
	[HideInInspector]
	public bool navigation = false;
	
	/**
	 *  Флаг фактического движения NavMeshAgent
	 */
	protected bool navMoving = false;
	
	/**
	 * Флаг захвата управления
	 */
	protected bool grabbed = false;
	
	/**
	 * Ссылка на контроллер персонажа
	 */
	protected ICharacterControl controller;
	
	/**
	 * Инициализация
	 */
	protected void Init()
	{
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		capsule = GetComponent<CapsuleCollider>();
		navAgent = GetComponent<NavMeshAgent>();
		//sound = GetComponent<AudioSource>();
		
		navigation = false;
		navMoving = false;
		
		dead = false;
		currentHealth = maxHealth;
	}
	
	/**
	 * Захват управления
	 *
	 * GameState вызывает эту функцию, когда берет управление контроллером
	 */
	public void Grab(ICharacterControl controller)
	{
		grabbed = true;
		this.controller = controller;
	}
	
	/**
	 * Освобождение управления
	 *
	 * GameState вызывает эту функцию, когда освобождает контроллер
	 */
	public void Release()
	{
		grabbed = false;
		controller = null;
	}
	
	/**
	 * Сброс состояния персонажа при перезагрузке сцены
	 */
	public void ResetMovement()
	{
		StopNavigation();
		rb.Sleep();
	}
	
	/**
	 * Отправить персонажа в указанные координаты (мировые)
	 */
	public void Navigate(Vector3 p)
	{
		if ( !navAgent.enabled )
		{
			rb.isKinematic = true;
			navAgent.enabled = true;
		}
		
		navigation = true;
		navAgent.SetDestination(p);
	}
	
	/**
	 * Остановить/отменить NavMeshAgent
	 */
	public void StopNavigation()
	{
		if ( navigation )
		{
			controller.OnNavigationStop();
			navMoving = false;
			navigation = false;
			rb.isKinematic = false;
			navAgent.enabled = false;
		}
	}
	
	/**
	 * Сброс NavMeshAgent
	 * TODO временный костыль
	 */
	public void ResetNavigation()
	{
		navigation = false;
		rb.isKinematic = false;
		
		if ( navAgent != null )
		{
			navAgent.enabled = false;
			
			if ( capsule != null )
			{
				//Debug.Log("fix NavMeshAgent");
				navAgent.height = capsule.height;
				navAgent.radius = capsule.radius;
			}
		}
	}
	
	/**
	 * Проверка, движется ли персонаж через NavMeshAgent
	 * TODO временный костыль
	 */
	public Vector3 NavVelocity
	{
		get { return navAgent.velocity; }
	}
	
	/**
	 * Проверка состояния NavMeshAgent (двигается или стоит)
	 * Если контроллеру нужны события OnNavigationStartMoving() и
	 * OnNavigationStopMoving(), то он должен регулярно вызывать этот метод
	 */
	public void CheckNavigationMoving()
	{
		if ( navigation )
		{
			if ( navMoving )
			{
				if ( navAgent.velocity == Vector3.zero )
				{
					navMoving = false;
					controller.OnNavigationStopMoving();
					return;
				}
			}
			else
			{
				if ( navAgent.velocity != Vector3.zero )
				{
					navMoving = true;
					controller.OnNavigationStartMoving();
					return;
				}
			}
		}
	}
	
	/**
	 * Обновить информацию об управлении персонажем
	 */
	public void SetMoveControl(bool moving, Vector3 localVelocity)
	{
		animator.SetBool("walk", navMoving || moving);
		animator.SetFloat("speedh", localVelocity.x);
		animator.SetFloat("speedv", localVelocity.z);
	}
	
	public void Jump()
	{
		StopNavigation();
		rb.AddForce(0f, 13.72f, 0f, ForceMode.VelocityChange);
	}
	
	public void JumpHeight(float height)
	{
		StopNavigation();
		float v = Mathf.Sqrt(2f * height * Physics.gravity.magnitude);
		rb.AddForce(0f, v, 0f, ForceMode.VelocityChange);
	}
	
	/**
	 * Разворачивать персонажа в указанное направление
	 * Предполагается что функция будет вызываться в FixedUpdate, чтобы
	 * непрерывно, с ограниченной скорость поворачивать персонажа
	 */
	public void RotateTowards(Quaternion target, float rotationSpeed)
	{
		rb.rotation = Quaternion.RotateTowards(rb.rotation, target, rotationSpeed * Time.deltaTime);
	}
	
	/**
	 * Двигать персонажа в указанном направлении
	 * Предполагается что функция будет вызываться в FixedUpdate, чтобы
	 * непрерывно двигать персонажа
	 * Y-составляющая скорости (вертикальная) игнорируется
	 */
	public void MoveVelocity(Vector3 velocity)
	{
		velocity.y = rb.velocity.y;
		rb.velocity = velocity;
	}
	
	/**
	 * Проверка стоит ли персонаж на земле
	 * TODO решить вопрос с областью видимости (public/protected/private)
	 * TODO решить вопрос кто и когда будет вызывать
	 */
	public void GroundCheck()
	{
		bool PreviouslyGrounded = grounded;
		grounded = ControllerUtils.GroundCheck(capsule);
		
		if ( PreviouslyGrounded )
		{
			if ( !grounded )
			{
				Debug.Log("falling/jumping");
				animator.SetBool("grounded", false);
			}
		}
		else
		{
			if ( grounded )
			{
				Debug.Log("grounded");
				falling = false;
				jumping = false;
				animator.SetBool("grounded", true);
				animator.SetBool("jumping", false);
				animator.SetBool("falling", false);
			}
		}
		
		if ( ! grounded )
		{
			jumping = rb.velocity.y > 0f;
			falling = !jumping;
			
			animator.SetBool("jumping", jumping);
			animator.SetBool("falling", falling);
		}
		
	}
	
} // class Character

} // namespace Nanosoft