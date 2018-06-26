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
	 * Инициализация
	 */
	protected void Init()
	{
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		capsule = GetComponent<CapsuleCollider>();
		navAgent = GetComponent<NavMeshAgent>();
		//sound = GetComponent<AudioSource>();
		
		dead = false;
		currentHealth = maxHealth;
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
		
		navAgent.SetDestination(p);
	}
	
	/**
	 * Остановить/отменить NavMeshAgent
	 */
	public void StopNavigation()
	{
		if ( navAgent.enabled )
		{
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
	
} // class Character

} // namespace Nanosoft