using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class GuardTraining: MonoBehaviour
{
	
	public bool isMain = false;
	public GuardTraining pair;
	public Animator anim;
	
	private float coolDown = 2f;
	private float switchTime = -1f;
	
	private bool isAttacker = true;
	private bool flipState = false;
	private bool busy = false;
	
	void Awake()
	{
		switchTime = Random.Range(0.4f, 0.8f);
		coolDown = switchTime + 1f;
	}
	
	void Update()
	{
		if ( !isMain ) return;
		
		if ( busy || pair.busy || coolDown > Time.time ) return;
		
		if ( switchTime < Time.time )
		{
			Debug.Log("time to switch");
			switchTime = Time.time;
			busy = true;
			anim.SetTrigger("Greating");
			pair.anim.SetTrigger("Greating");
			return;
		}
		
		if ( isAttacker )
		{
			Debug.Log("make attack, time=" + Time.time + ", coolDown=" + coolDown);
			anim.SetTrigger(flipState ? "Attack1" : "Attack2");
			//pair.anim.SetTrigger("Damage1");
			flipState = !flipState;
			busy = true;
		}
		
	}
	
	void OnAttack1Hit()
	{
		anim.SetTrigger("Damage1");
	}
	
	void OnAttack2Hit()
	{
		anim.SetTrigger("Damage1");
	}
	
	void Attack1Hit(int x)
	{
		if ( pair != null ) pair.OnAttack1Hit();
	}
	
	void Attack2Hit(int x)
	{
		if ( pair != null ) pair.OnAttack2Hit();
	}
	
	void AttackEnd()
	{
		busy = false;
		coolDown = Time.time + 0.4f;
	}
	
	void GreatingEnd()
	{
		busy = false;
		switchTime = Time.time + 16f;
		coolDown = Time.time + Random.Range(0.4f, 0.8f);
		isMain = !isMain;
	}
	
}

} // namespace Nanosoft
