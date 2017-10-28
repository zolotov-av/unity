using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRotation: MonoBehaviour
{
	
	/**
	 * Цель за которой надо сделить
	 */
	public Transform target;
	
	/**
	 * Максимальная скорость вращения (градус/сек)
	 */
	public float maxSpeed = 30f;
	
	/**
	 * Включить вращение
	 *
	 * Если true, то объект будет вращаться вслед за целью
	 * Включается/выключается нажатием клавишы Space (пробел)
	 */
	public bool enableRotation = false;
	
	/**
	 * Текстовый блок куда выводить отладочную информацию
	 */
	public Text txt;
	
	protected void DebugInfo()
	{
		// вычисляем угол между осями right
		Vector3 r1 = transform.right;
		Vector3 r2 = target.right;
		float cr = Mathf.Clamp(Vector3.Dot(r1, r2), -1f, 1f);
		float ar = Mathf.Acos(cr) * Mathf.Rad2Deg;
		
		// вычисляем угол между осями forward
		Vector3 f1 = transform.forward;
		Vector3 f2 = target.forward;
		float cf = Mathf.Clamp(Vector3.Dot(f1, f2), -1f, 1f);
		float af = Mathf.Acos(cf) * Mathf.Rad2Deg;
		
		// вычисляем угол между осями up
		Vector3 u1 = transform.up;
		Vector3 u2 = target.up;
		float cu = Mathf.Clamp(Vector3.Dot(u1, u2), -1f, 1f);
		float au = Mathf.Acos(cu) * Mathf.Rad2Deg;
		
		// угол между двумя вращениями
		float a1 = Mathf.Sqrt(ar * ar + af * af + au * au) / Mathf.Sqrt(2f);
		float a2 = Quaternion.Angle(transform.rotation, target.rotation);
		
		txt.text =
			"ar: " + string.Format("{0:0.000}", ar) + "\n" +
			"af: " + string.Format("{0:0.000}", af) + "\n" +
			"au: " + string.Format("{0:0.000}", au) + "\n" +
			"a1: " + string.Format("{0:0.000}", a1) + "\n" +
			"a2: " + string.Format("{0:0.000}", a2);
	}
	
	void FixedUpdate()
	{
		if ( txt != null )
		{
			DebugInfo();
		}
		
		if ( enableRotation )
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, maxSpeed *  Time.deltaTime);
		}
	}
	
	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.Space) )
		{
			enableRotation = !enableRotation;
		}
	}
	
}
