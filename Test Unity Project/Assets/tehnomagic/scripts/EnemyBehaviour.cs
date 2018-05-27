using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Nanosoft
{

public class EnemyBehaviour: MonoBehaviour
{
	
	private Animator animator;
	private AudioSource sound;
	
	/**
	 * Расстояние на котором противник агриться на игрока
	 */
	public float aggroDistance = 5f;
	
	/**
	 * Расстояние на котором противник прекращает преследование игрока
	 */
	public float giveupDistance = 10f;
	
	/**
	 * Расстояние на котором противник может атаковать
	 */
	public float attackDistance = 1.2f;
	
	private int currentHealth = 0;
	public int maxHealth = 100;
	
	public Transform labelAnchor;
	
	/**
	 * Игрок на которого агрится противник
	 */
	private PlayerBehaviour target = null;
	
	/**
	 * Ссылка на NavMeshAgent
	 */
	private NavMeshAgent navAgent = null;
	private float navAgentCD = 0f;
	private float attackCD = 0f;
	private float aggroCD = 0f;
	private bool busy = false;
	
	private bool dead = false;
	
	public void GiveupAggro()
	{
		if ( target != null )
		{
			Debug.Log("Enemy[" + gameObject.name + "] gived up player[" + target.gameObject.name + "]");
			target = null;
			navAgent.ResetPath();
		}
	}
	
	public void AggroPlayer(PlayerBehaviour player)
	{
		if ( target == player )
		{
			aggroCD = Time.time + Random.Range(5f, 7f);
			return;
		}
		GiveupAggro();
		Debug.Log("Enemy[" + gameObject.name + "] aggro to player[" + player.gameObject.name + "]");
		target = player;
		aggroCD = Time.time + Random.Range(5f, 7f);
	}
	
	/**
	 * Проверка агро
	 * Здесь нужно проверить видит ли (слышит ли и т.п.) противник указанного игрока
	 */
	public void CheckAggro(PlayerBehaviour player)
	{
		if ( dead )
		{
			// мертвые не агрятся
			return;
		}
		
		if ( target != null )
		{
			// если уже есть цель, то ничего недалать не надо
			return;
		}
		
		if ( Vector3.Distance(transform.position, player.transform.position) < aggroDistance )
		{
			// агримся на игрока
			AggroPlayer(player);
		}
	}
	
	/**
	 * Обновить агро
	 */
	protected void UpdateAggro()
	{
		if ( dead )
		{
			// мертвые не агрятся
			return;
		}
		
		if ( target == null )
		{
			// цели нет
			return;
		}
		
		float distance = Vector3.Distance(transform.position, target.transform.position);
		if ( distance > giveupDistance && Time.time > aggroCD )
		{
			GiveupAggro();
			return;
		}
		
		//Quaternion rot = Quaternion.LookRotation(target.transform.position - transform.position);
		//transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 240f * Time.deltaTime);
		
		if ( !busy && attackCD < Time.time )
		{
			if ( distance < attackDistance )
			{
				float angle = Vector3.Angle(transform.forward, target.transform.position - transform.position);
				if ( angle > 10f )
				{
					Quaternion rot = Quaternion.LookRotation(target.transform.position - transform.position);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 240f * Time.deltaTime);
				}
				else
				{
					navAgent.ResetPath();
					busy = true;
					animator.SetTrigger("Attack1h1");
					attackCD = Time.time + Random.Range(2f, 4f);
				}
			}
			else
			{
				attackCD = Time.time + Random.Range(0.2f, 0.5f);
			}
		}
		
		if ( !busy && navAgentCD < Time.time )
		{
			navAgent.SetDestination(target.transform.position);
			navAgentCD = Time.time + Random.Range(3f, 4f);
		}
		
		
	}
	
	/**
	 * Обработчик удара Attack1
	 */
	void Attack1Hit()
	{
		
	}
	
	/**
	 * Обработчик завершения атаки
	 */
	void AttackEnd()
	{
		busy = false;
	}
	
	void Awake()
	{
		animator = GetComponent<Animator>();
		sound = GetComponent<AudioSource>();
		navAgent = GetComponent<NavMeshAgent>();
		
		currentHealth = maxHealth;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if ( dead )
		{
			// мертвые не получают дамаг
			return;
		}
		
		AttackBehaviour attack = other.GetComponent<AttackBehaviour>();
		if ( attack != null )
		{
			int damage = Mathf.FloorToInt(Random.Range(24, 36));
			CanvasScript.ThrowLabel(damage, labelAnchor.transform.position);
			currentHealth -= damage;
			if ( currentHealth <= 0 )
			{
				currentHealth = 0;
				animator.SetBool("Dead", true);
				animator.SetTrigger("Hit1");
				sound.Stop();
				sound.Play();
				busy = false;
				dead = true;
				navAgent.ResetPath();
				return;
			}
			animator.SetTrigger("Hit1");
			sound.Stop();
			sound.Play();
			busy = false;
			navAgent.ResetPath();
			navAgentCD = Time.time + Random.Range(0.4f, 0.8f);
			PlayerBehaviour player = attack.owner.GetComponent<PlayerBehaviour>();
			if ( player != null ) AggroPlayer(player);
		}
	}
	
	void Update()
	{
		UpdateAggro();
		
		animator.SetFloat("speedv", Vector3.Dot(navAgent.velocity, transform.forward));
		animator.SetFloat("speedh", 0f);
	}
	
}

} // namespace Nanosoft