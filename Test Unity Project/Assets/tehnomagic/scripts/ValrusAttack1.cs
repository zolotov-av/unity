using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class ValrusAttack1: AttackBehaviour
{
	
	public const float speed = 10f;
	public const float distance = 6f;
	
	protected Vector3 velocity;
	protected float timeLimit;
	
	public void Run(Valrus obj, float height = 0.8f)
	{
		owner = obj.gameObject;
		timeLimit = Time.time + distance / speed;
		transform.position = owner.transform.position + owner.transform.up * height;
		transform.rotation = owner.transform.rotation;
		velocity = owner.transform.forward * speed;
	}
	
	void Update()
	{
		if ( Time.time > timeLimit )
		{
			Destroy(gameObject);
			return;
		}
	}
	
	void FixedUpdate()
	{
		transform.position += velocity * Time.deltaTime;
	}
	
}

} // namespace Nanosoft
