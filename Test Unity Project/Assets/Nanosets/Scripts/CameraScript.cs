using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Скрипт для управления камеры
 *
 * Заставляет камеру следить за указанным объектом
 */
public class CameraScript: MonoBehaviour
{
	
	public const int FollowCamera = 0;
	public const int FreeCamera = 1;
	
	/**
	 * Цель за которой должна следить камера
	 */
	public GameObject target;
	
	/**
	 * Текстовый блок в UI в который выводиться отладочная информация
	 */
	public Text camdbg;
	
	[Header("Camera Settings")]
	public float height = 1.4f;
	public float startDistance = 3f;
	public float startAngle = 20f;
	
	private float camAngle = 0f;
	private float camAngleSpeed = 0f;
	private float distance = 0f;
	private Quaternion rotation;
	private int mode = FollowCamera;
	
	private const float angleSensitivity = 300f;
	private const float minAngle = -40f;
	private const float maxAngle = 80f;
	private const float maxAngleSpeed = 360f;
	
	#region Utils
	
	protected string coord(Vector3 v)
	{
		return ((int)v.x).ToString() + ", " + ((int)v.y).ToString() + ", " + ((int)v.z).ToString();
	}
	
	protected void DbgUpdate()
	{
		if (camdbg)
		{
			camdbg.text = "Nanosoft.Camera v22\n" +
				"CamPos: " + coord(transform.position) + "\n" +
				"TargetPos: " + coord(target.transform.position) + "\n" +
				"distance: " + string.Format("{0:0.00}", distance) + "\n" +
				"angle: " + string.Format("{0:0.00}", camAngle);
		}
	}
	
	public static float ClampAngle(float angle, float min, float max)
	{
	while (angle > 360) angle -= 360;
	while (angle < -360) angle += 360;
	return Mathf.Clamp(angle, min, max);
	}
	
	#endregion
	
	void Start()
	{
		distance = startDistance;
		camAngle = startAngle;
		
		if ( !camdbg )
		{
			var obj = GameObject.FindWithTag("CameraDebug");
			if ( obj ) camdbg = obj.GetComponent<Text>();
		}
	}
	
	public void UpdateOptions(float distanceDelta, float angleDelta)
	{
		distance -= distanceDelta;
		distance = Mathf.Clamp(distance, 1f, 5f);
		
		camAngleSpeed = Mathf.Clamp(angleDelta * angleSensitivity, -maxAngleSpeed, maxAngleSpeed);
	}
	
	public Quaternion GetRotation()
	{
		return new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
	}
	
	public void SetRotation(Quaternion rot)
	{
		rotation.Set(rot.x, rot.y, rot.z, rot.w);
	}
	
	public void SetMode(int m)
	{
		mode = m;
	}
	
	public void Rotate(Quaternion rot)
	{
		rotation *= rot;
	}
	
	void FixedUpdate()
	{
		camAngle = ClampAngle(camAngle - camAngleSpeed * Time.deltaTime, minAngle, maxAngle);
	}
	
	void LateUpdate()
	{
		Vector3 tp = target.transform.position + target.transform.up * height;
		
		float rad = camAngle * Mathf.Deg2Rad;
		Vector3 up, forward;
		if ( mode == FollowCamera )
		{
			up = target.transform.up;
			forward = target.transform.forward;
		}
		else
		{
			up = rotation * Vector3.up;
			forward = rotation * Vector3.forward;
		}
		Vector3 cv = up * (Mathf.Sin(rad) * distance);
		Vector3 sv = forward * (-Mathf.Cos(rad) * distance);
		transform.position = tp + cv + sv;
		
		transform.LookAt(tp);
		
		DbgUpdate();
	}
	
} // class CameraScript

} // namespace Nanosoft
