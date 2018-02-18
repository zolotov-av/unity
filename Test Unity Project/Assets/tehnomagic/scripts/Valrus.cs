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
	
	void Awake()
	{
		animator = GetComponent<Animator>();
		sound = GetComponent<AudioSource>();
		sword.SetActive(battle);
	}
	
	void Update()
	{
		if ( Input.GetKeyDown("f") )
		{
			battle = !battle;
			sword.SetActive(battle);
		}
	}
	
}

} // namespace Nanosoft
