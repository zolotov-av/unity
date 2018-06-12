using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class SunLight: MonoBehaviour
{
	
	private int time = 0;
	private float tm = 0f;
	private float curr = 0f;
	private float dest = 0f;
	public float s_diff = 0f;
	public float speed = 0f;
	
	private float m_latitude = -50;
	private float cosA = 1f;
	private float sinA = 0f;
	
	public float latitude
	{
		get { return m_latitude; }
		set { SetLatitude(value); }
	}
	
	public void SetLatitude(float value)
	{
		m_latitude = value;
		float a = m_latitude * Mathf.Deg2Rad;
		cosA = Mathf.Cos(a);
		sinA = Mathf.Sin(a);
	}
	
	void Awake()
	{
		float a = m_latitude * Mathf.Deg2Rad;
		cosA = Mathf.Cos(a);
		sinA = Mathf.Sin(a);
	}
	
	void Update()
	{
		
		if ( tm < Time.time )
		{
			time = (time + 1) % 360;
			tm = Time.time + 0.5f;
			
			dest = time;
		}
		
		float diff = dest - curr;
		while ( diff > 180f ) diff -= 360f;
		while ( diff < -180f ) diff += 360f;
		s_diff = diff;
		float len = Mathf.Abs(diff);
		if ( len > 0.01 )
		{
			speed = Mathf.Clamp(len, 1.8f, 3f);
			curr += speed * diff * Time.deltaTime / len;
			transform.eulerAngles = new Vector3(curr, 0f, 0f);
			while ( curr > 360f ) curr -= 360f;
		}
		
		
		/*
		float b = Time.time * 0.005f + 240*Mathf.Deg2Rad;
		//float a = 50f * Mathf.Deg2Rad;
		float cosB = Mathf.Cos(b);
		float sinB = Mathf.Sin(b);
		//float cosA = Mathf.Cos(a);
		//float sinA = Mathf.Sin(a);
		//Vector3 s = new Vector3(sinB, cosB, 0f);
		//Vector3 n = new Vector3(0, 0, 1f);
		Vector3 s = new Vector3(sinB, cosA * cosB, -sinA * cosB);
		Vector3 n = new Vector3(0, sinA, cosA);
		transform.rotation = Quaternion.LookRotation(s, n);
		*/
	}
	
}

} // namespace Nanosoft

