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
	
	/**
	 * Цель за которой должна следить камера
	 */
	public GameObject target;
	
	/**
	 * Текстовый блок в UI в который выводиться отладочная информация
	 */
	public Text camdbg;
	
	[Header("Camera Settings")]
	
	/**
	 * Высота полета камеры
	 *
	 * Начало координат в модели персонажа обычно находиться в ногах. Этот
	 * параметр нужен чтобы камера не валялась на земле и смотрела на тело
	 * персонажа, а не на его ноги. Задает вертикальное смещение куда будет
	 * смотреть камера, например на уровне грудной клетки или плечь персонажа.
	 */
	public float height = 1.4f;
	
	/**
	 * Начальное расстояние от персонажа до камеры
	 */
	public float startDistance = 3f;
	
	/**
	 * Начальный угол наклона камеры (тангаж - наклон в сторону земли/неба)
	 */
	public float startAngle = 20f;
	
	/**
	 * Максимальное расстояние от персонажа до камеры
	 */
	public float maxDistance = 10f;
	
	/**
	 * Угол наклона камеры (тангаж)
	 */
	private float camAngle = 0f;
	
	/**
	 * Скорость изменения угла наклона камеры (тангажа)
	 */
	private float camAngleSpeed = 0f;
	
	/**
	 * Дистанция камеры (расстояние от персонажа до камеры)
	 */
	private float distance = 0f;
	
	/**
	 * Ориентация камеры относительно цели
	 *
	 * Параметр rotation задает направление куда смотрит персонаж (реальное,
	 * в случае следящего режима, или виртуальное, в случае режима свободного
	 * вращения камеры). Реальное положение и направление камеры рассчитывается
	 * автоматически в LateUpdate(), с учетом дистанции и тангажа (наклона
	 * в строну земли/неба).
	 *
	 * В следящем режиме камеры rotation = target.transform.rotation, т.е.
	 * указывает направление куда смотрит персонаж, а сама камера находиться за
	 * спиной персонажа и смотрит примерно в то же самое направление с
	 * поправкой на тангаж (наклон в строну земли или неба).
	 *
	 * В режиме свободного вращения камеры rotation задает виртуальное
	 * направление перснонажа, куда бы он смотрел, если был бы повернут также.
	 * Этот режим позволяет смотреть на персонажа с любой стороны.
	 */
	[HideInInspector]
	public Quaternion rotation;
	
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
			camdbg.text = "Debug info, " + 
				"Camera v22\n" +
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
		distance = Mathf.Clamp(distance, 1f, maxDistance);
		
		camAngleSpeed = Mathf.Clamp(angleDelta * angleSensitivity, -maxAngleSpeed, maxAngleSpeed);
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
		
		Vector3 up = rotation * Vector3.up;
		Vector3 forward = rotation * Vector3.forward;
		Vector3 cv = up * (Mathf.Sin(rad) * distance);
		Vector3 sv = forward * (-Mathf.Cos(rad) * distance);
		transform.position = tp + cv + sv;
		
		transform.LookAt(tp);
		
		DbgUpdate();
	}
	
} // class CameraScript

} // namespace Nanosoft
