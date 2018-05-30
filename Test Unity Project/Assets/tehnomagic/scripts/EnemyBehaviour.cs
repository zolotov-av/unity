using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Nanosoft
{

public class EnemyBehaviour: DamageBehaviour
{
	
	enum State {Idle, Follow, Attack, Rage, Return};
	
	private Animator animator;
	private AudioSource sound;
	
	/**
	 * Расстояние до цели
	 */
	protected float distance;
	
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
	public float attackDistanceMax = 1.4f;
	
	public bool look = false;
	public float lookDistance = 4f;
	public float lookDistanceMax = 4.5f;
	
	public Transform labelAnchor;
	public Transform attackAnchor;
	
	/**
	 * Мобы могут быть объединены в паки
	 */
	private Transform packObject;
	
	/**
	 * Игрок на которого агрится противник
	 */
	private PlayerBehaviour target = null;
	
	/**
	 * Флаг навигации (движения через NavMeshAgent)
	 */
	private bool navigate = false;
	
	/**
	 * Ссылка на NavMeshAgent
	 */
	private NavMeshAgent navAgent = null;
	private float navAgentCD = 0f;
	private float attackCD = 0f;
	private float aggroCD = 0f;
	private bool busy = false;
	private bool follow = false;
	private float followCD = 0f;
	
	private bool dead = false;
	
	private float rage = 0f;
	private float rageExpire = 0f;
	private const float rageThreshHi = 100f;
	private const float rageThreshLow = 20f;
	private const float rageCoolSpeed = 1f;
	
	/**
	 * Состояние противника - ожидает, преследует, атакует
	 */
	private State state;
	
	private Collider[] cache;
	
	public void GiveupAggro()
	{
		if ( target != null )
		{
			Debug.Log("Enemy[" + gameObject.name + "] gived up player[" + target.gameObject.name + "]");
			target = null;
			state = State.Idle;
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
		rage = 10f;
		aggroCD = Time.time + Random.Range(5f, 7f);
		state = State.Follow;
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
	 * Перейти к новому состоянию
	 */
	protected void SetNextState()
	{
		rage -= rageCoolSpeed * Time.deltaTime;
		if ( rage < 0f ) rage = 0f;
		
		if ( state == State.Rage )
		{
			if ( rage < rageThreshLow || Time.time > rageExpire )
			{
				rage = 10f;
				state = State.Follow;
				labelAnchor.GetComponent<MeshRenderer>().enabled = false;
				Debug.Log(gameObject.name + " rage finished");
			}
			
			return;
		}
		
		if ( state == State.Follow )
		{
			if ( distance < attackDistance )
			{
				state = State.Attack;
				navigate = false;
				navAgent.ResetPath();
			}
			
			return;
		}
		
		if ( state == State.Attack )
		{
			if ( distance > attackDistanceMax )
			{
				state = State.Follow;
			}
			
			return;
		}
	}
	
	/**
	 * Действия в состоянии преследования
	 * Цель преследования - сократить дистанцию, чтобы можно было нанести удар
	 */
	protected void HandleFollowState()
	{
		if ( follow )
		{
			if ( Time.time > followCD )
			{
				follow = false;
				navigate = false;
				navAgent.ResetPath();
				followCD = Time.time + Random.Range(1f, 2f);
			}
		}
		else
		{
			if ( Time.time > followCD )
			{
				follow = true;
				followCD = Time.time + Random.Range(2f, 3f);
			}
		}
		
		if ( follow && !busy && navAgentCD < Time.time && distance > attackDistanceMax )
		{
			navigate = true;
			navAgent.SetDestination(target.transform.position);
			navAgentCD = Time.time + 0.4f;
		}
	}
	
	/**
	 * Действия в состоянии атаки
	 */
	protected void HandleAttackState()
	{
		if ( !busy && Time.time > attackCD )
		{
			float angle = Vector3.Angle(transform.forward, target.transform.position - transform.position);
			if ( angle < 10f )
			{
				busy = true;
				animator.SetTrigger("Attack1");
				attackCD = Time.time + Random.Range(2f, 4f);
			}
		}
	}
	
	/**
	 * Действия в состоянии ярости
	 * В состоянии ярости монстр непрерывно преследует и атакует цель
	 */
	protected void HandleRageState()
	{
		if ( busy ) return;
		
		if ( distance < attackDistance )
		{
			float angle = Vector3.Angle(transform.forward, target.transform.position - transform.position);
			if ( angle < 10f )
			{
				if ( navigate )
				{
					navigate = false;
					navAgent.ResetPath();
				}
				busy = true;
				animator.SetTrigger("Attack1");
				attackCD = Time.time + Random.Range(0.4f, 0.8f);
				return;
			}
		}
		
		if ( Time.time > navAgentCD )
		{
			navigate = true;
			navAgent.SetDestination(target.transform.position);
			navAgentCD = Time.time + 0.4f;
		}
	}
	
	/**
	 * Обновить агро
	 */
	protected void UpdateAggro()
	{
		if ( state == State.Idle )
		{
			return;
		}
		
		distance = Vector3.Distance(transform.position, target.transform.position);
		if ( distance > giveupDistance && Time.time > aggroCD )
		{
			GiveupAggro();
			return;
		}
		
		if ( look )
		{
			if ( distance > lookDistanceMax )
			{
				look = false;
				navAgent.updateRotation = true;
			}
		}
		else
		{
			if ( distance < lookDistance )
			{
				look = true;
				navAgent.updateRotation = false;
			}
		}
		
		if ( look && !busy )
		{
			Quaternion rot = Quaternion.LookRotation(target.transform.position - transform.position);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 240f * Time.deltaTime);
		}
		
		SetNextState();
		
		switch ( state )
		{
		case State.Follow:
			HandleFollowState();
			break;
		case State.Attack:
			HandleAttackState();
			break;
		case State.Rage:
			HandleRageState();
			break;
		}
	}
	
	/**
	 * Обработчик удара Attack1
	 */
	void Attack1Hit()
	{
		if ( attackAnchor == null ) return;
		int layerMask = 1 << 8;
		const float radius = 1.5f;
		int count = Physics.OverlapSphereNonAlloc(attackAnchor.position, radius, cache, layerMask, QueryTriggerInteraction.Ignore);
		for(int i = 0; i < count; i++)
		{
			PlayerBehaviour player = cache[i].GetComponent<PlayerBehaviour>();
			if ( player != null )
			{
				int damage = Mathf.FloorToInt(Random.Range(28, 36));
				player.ApplyDamage(damage);
			}
		}
	}
	
	/**
	 * Обработчик завершения атаки
	 */
	void AttackEnd()
	{
		busy = false;
	}
	
	/**
	 * Обработчик завершения реакции на удар
	 */
	void HitEnd()
	{
		attackCD = 0f;
		busy = false;
	}
	
	void Awake()
	{
		animator = GetComponent<Animator>();
		sound = GetComponent<AudioSource>();
		navAgent = GetComponent<NavMeshAgent>();
		
		packObject = transform.parent;
		if ( packObject != null )
		{
			transform.SetParent(null);
		}
		
		look = false;
		navAgent.updateRotation = !look;
		currentHealth = maxHealth;
		
		cache = new Collider[16];
		
		ResetSuperArmor();
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
			int damage = Mathf.FloorToInt(Random.Range(attack.minDamage, attack.maxDamage));
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
				state = State.Idle;
				navAgent.ResetPath();
				labelAnchor.GetComponent<MeshRenderer>().enabled = false;
				GetComponent<Collider>().enabled = false;
				return;
			}
			if ( state != State.Rage )
			{
				animator.SetTrigger("Hit1");
				busy = true;
			}
			sound.Stop();
			sound.Play();
			
			//navAgent.ResetPath();
			//navAgentCD = Time.time + Random.Range(0.4f, 0.8f);
			PlayerBehaviour player = attack.owner.GetComponent<PlayerBehaviour>();
			if ( player != null ) AggroPlayer(player);
			
			// TODO balance rage
			rage += 10;
			if ( follow ) rage += 6;
			
			if ( state != State.Rage && rage > rageThreshHi )
			{
				Debug.Log(gameObject.name + " become rage");
				state = State.Rage;
				rageExpire = Time.time + 5f;
				labelAnchor.GetComponent<MeshRenderer>().enabled = true;
			}
		}
	}
	
	void Update()
	{
		UpdateAggro();
		
		animator.SetFloat("speedv", Vector3.Dot(navAgent.velocity, transform.forward));
		animator.SetFloat("speedh", Vector3.Dot(navAgent.velocity, transform.right));
	}
	
}

} // namespace Nanosoft