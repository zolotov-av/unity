using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class DamageBehaviour: MonoBehaviour
{
	
	protected int currentHealth = 0;
	public int maxHealth = 100;
	
	[HideInInspector]
	public int superArmor = 0;
	public int defaultSuperArmor = 10;
	public int maxSuperArmor = 100;
	
	public void ApplyDamage(int damage)
	{
		currentHealth -= damage;
		if ( currentHealth <= 0 )
		{
			currentHealth = 0;
			// TODO dead
		}
	}
	
	public void ResetSuperArmor()
	{
		superArmor = defaultSuperArmor;
		if ( superArmor < 0 ) superArmor = 0;
	}
	
	public void IncreaseSuperArmor(int value)
	{
		Debug.Log(gameObject.name + " increase super armor (" + value + ")");
		superArmor += value;
		if ( superArmor > maxSuperArmor )
		{
			superArmor = maxSuperArmor;
		}
	}
	
	public void DecreaseSuperArmor(int value)
	{
		superArmor -= value;
		if ( superArmor < 0 ) superArmor = 0;
	}
	
} // class DamageBehaviour

} // namespace Nanosoft
