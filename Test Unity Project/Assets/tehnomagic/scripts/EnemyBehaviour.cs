using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class EnemyBehaviour: MonoBehaviour
{
	
	private Animator animator;
	private AudioSource sound;
	
	void Awake()
	{
		animator = GetComponent<Animator>();
		sound = GetComponent<AudioSource>();
	}
	
	void OnTriggerEnter(Collider other)
	{
		Debug.Log("EnemyBehaviour OnTriggerEnter");
		
		AttackBehaviour attack = other.GetComponent<AttackBehaviour>();
		if ( attack != null )
		{
			animator.SetTrigger("Hit1");
			sound.Stop();
			sound.Play();
		}
	}
	
}

} // namespace Nanosoft