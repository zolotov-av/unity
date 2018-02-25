using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class Valrus: PlayerBehaviour
{
	
	public GameObject attack1Prefab;
	public AudioClip attack1SoundA;
	public AudioClip attack1SoundB;
	public GameObject attack2Projector;
	
	public GameObject sword;
	
	/**
	 * Ссылка на аниматор персонажа
	 */
	private Animator animator;
	private AudioSource sound;
	
	protected bool battle = false;
	protected bool busy = false;
	protected string attack = "none";
	protected bool soundA = false;
	
	private float attack2cd = 0f;
	private float attack2expire = 0f;
	
	void Hit(int x)
	{
		Debug.Log("Valrus hit");
		
		if ( attack == "attack1" )
		{
			Debug.Log("Valrus attack1 hit");
			GameObject obj = Instantiate(attack1Prefab, transform.position, transform.rotation);
			ValrusAttack1 va = obj.GetComponent<ValrusAttack1>();
			if ( va == null )
			{
				Debug.LogError("prefab have not ValrusAttack1");
				Destroy(obj);
			}
			else
			{
				va.Run(this);
				busy = false;
			}
		}
		
		if ( attack == "attack3" )
		{
			Debug.Log("Valrus attack2 hit");
			busy = false;
			Attack2Hit();
		}
		
		attack = "none";
	}
	
	public override void Attack1()
	{
		if ( busy || !battle ) return;
		busy = true;
		attack = "attack1";
		animator.SetTrigger(attack);
		sound.Stop();
		sound.PlayOneShot(soundA ? attack1SoundA : attack1SoundB);
		soundA = !soundA;
	}
	
	public override void Attack2()
	{
		if ( busy || !battle ) return;
		if ( Time.time <= attack2cd ) return;
		
		busy = true;
		attack = "attack3";
		animator.SetTrigger(attack);
		attack2cd = Time.time + 1f;
		
		//sound.Stop();
		//sound.PlayOneShot(soundA ? attack1SoundA : attack1SoundB);
		//soundA = !soundA;
	}
	
	protected void Attack2Hit()
	{
		attack2Projector.transform.position = transform.position + transform.forward * 1.6f;
		attack2Projector.SetActive(true);
		attack2expire = Time.time + 3f;
	}
	
	void Awake()
	{
		animator = GetComponent<Animator>();
		sound = GetComponent<AudioSource>();
		sword.SetActive(battle);
		attack2Projector.SetActive(false);
		attack2Projector.transform.SetParent(null, true);
		attack2Projector.GetComponent<AttackBehaviour>().owner = gameObject;
		DontDestroyOnLoad(attack2Projector);
	}
	
	void Update()
	{
		if ( Input.GetKeyDown("f") )
		{
			battle = !battle;
			sword.SetActive(battle);
		}
		
		if ( attack2expire != 0f && Time.time > attack2expire )
		{
			attack2Projector.SetActive(false);
			attack2expire = 0f;
		}
	}
	
}

} // namespace Nanosoft
