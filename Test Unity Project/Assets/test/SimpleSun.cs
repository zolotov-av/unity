using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//#pragma warning disable 0414

namespace Nanosoft
{

public class SunLight: MonoBehaviour
{
	
	private float curr;
	private Light light;
	
	public float speed = 10f;
	public Gradient gradient;
	
	void Awake()
	{
		curr = 0f;
		light = GetComponent<Light>();
	}
	
	void Update()
	{
		
		curr += speed * Time.deltaTime;
		while ( curr > 180f ) curr -= 360f;
		while ( curr < -180f ) curr += 360f;
		transform.eulerAngles = new Vector3(curr, 0f, 0f);
		
		float diff = curr - 90f;
		while ( diff > 180f ) diff -= 360f;
		while ( diff < -180f ) diff += 360f;
		
		float len = (diff + 180f) / 360f;
		light.color = gradient.Evaluate(len);
	}
	
}

} // namespace Nanosoft
