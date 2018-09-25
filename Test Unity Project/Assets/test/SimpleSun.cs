using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//#pragma warning disable 0414

namespace Nanosoft
{

public class SunLight: MonoBehaviour
{
	
	private float curr;
	public float speed = 10f;
	
	void Awake()
	{
		curr = 0f;
	}
	
	void Update()
	{
		
		curr += speed * Time.deltaTime;
		while ( curr > 360f ) curr -= 360f;
		transform.eulerAngles = new Vector3(curr, 0f, 0f);
	}
	
}

} // namespace Nanosoft
