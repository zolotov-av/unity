using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Nanosoft
{

public class EnemyBehaviour: DamageBehaviour
{
	
	enum State {Idle, Follow, Attack, Return};
	
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
	
	private bool rageOn = false;
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
		rageOn = false;
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
	 * Обновить агро
	 */
	protected void UpdateAggro()
	{
		/**
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
		*/
		
		if ( state == State.Idle )
		{
			return;
		}
		
		rage -= rageCoolSpeed * Time.deltaTime;
		if ( rage < 0f ) rage = 0f;
		
		if ( rageOn && (rage < rageThreshLow || Time.time > rageExpire) )
		{
			rage = 10f;
			rageOn = false;
			labelAnchor.GetComponent<MeshRenderer>().enabled = false;
			Debug.Log(gameObject.name + " rage finished");
		}
		
		float distance = Vector3.Distance(transform.position, target.transform.position);
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
		
		if ( state == State.Follow )
		{
			if ( distance < attackDistance )
			{
				Debug.Log(gameObject.name + " start attack");
				state = State.Attack;
				navigate = false;
				navAgent.ResetPath();
			}
		}
		else if ( state == State.Attack )
		{
			if ( distance > attackDistanceMax )
			{
				Debug.Log(gameObject.name + " start follow");
				state = State.Follow;
			}
		}
		
		if ( state == State.Follow )
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
		else if ( state == State.Attack )
		{
			if ( !busy && Time.time > attackCD )
			{
				float angle = Vector3.Angle(transform.forward, target.transform.position - transform.position);
				if ( angle > 10f )
				{
					//Quaternion rot = Quaternion.LookRotation(target.transform.position - transform.position);
					//transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 240f * Time.deltaTime);
				}
				else
				{
					busy = true;
					animator.SetTrigger("Attack1");
					float cd = rageOn ? Random.Range(0.4f, 0.8f) : Random.Range(2f, 4f);
					attackCD = Time.time + cd;
				}
			}
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
			Debug.Log(gameObject.name + "hits " + cache[i].gameObject.name);
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
		Debug.Log(gameObject.name + " HitEnd()");
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
			if ( !rageOn )
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
			
			if ( rage > rageThreshHi )
			{
				Debug.Log(gameObject.name + " become rage");
				rageOn = true;
				rageExpire = Time.time + 3f;
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