using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Скрипт следящей камеры
 *
 * Заставляет камеру следить за указанным объектом (персонажем), камера
 * располагается за спиной персонажа, на расстоянии distance, на высоте height,
 * смотрит под углом camAngle (в градусах)
 *
 * Примечание: данный скрипт написан лишь для демонстрации позиционирования
 *   камеры для персонажа от третьего лица. Полноценное управление камерой
 *   может отличаться от проекта к проекту.
 */
public class CameraFollow: MonoBehaviour
{
	
	/**
	 * Цель за которой должна следить камера
	 */
	public Transform target;
	
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
	 * Дистанция камеры (расстояние от персонажа до камеры)
	 */
	public float distance = 5f;
	
	/**
	 * Угол наклона камеры (тангаж)
	 */
	public float camAngle = 30f;
	
	/**
	 * Минимальный угол наклона камеры
	 */
	public float minAngle = -40f;
	
	/**
	 * Максимальный угол наклона камеры
	 */
	public float maxAngle = 80f;
	
	/**
	 * Функция ограничивает угол некоторым пределами min/max
	 */
	public static float ClampAngle(float angle, float min, float max)
	{
		while (angle > 360) angle -= 360;
		while (angle < -360) angle += 360;
		return Mathf.Clamp(angle, min, max);
	}
	
	/**
	 * Установить угол наклона камеры с ограничением min/max
	 */
	public void SetCamAngle(float angle)
	{
		camAngle = ClampAngle(camAngle, minAngle, maxAngle);
	}
	
	/**
	 * Расчет камеры делаем в LateUpdate(), чтобы было гарантированно после
	 * всех обновлений (расчетов в Update())
	 */
	void LateUpdate()
	{
		// точка на которую будет смотреть камера (центр экрана)
		Vector3 tp = target.position + target.up * height;
		
		// переводим угол наклона из градусов в радианы
		float rad = camAngle * Mathf.Deg2Rad;
		
		// расчет положения камеры
		// UPD: для Raycast'а нам нужен вектор-смещение от цели к камере
		//
		// немного арифметики - перемещаем камеру назад на расстояние distance
		// и наклоняем под углом camAngle, а точнее вычисляем позицию где
		// должна находиться камера, чтобы она была на расстоянии distance
		// и смотрела под углом camAngle
		Vector3 cv = target.up * (Mathf.Sin(rad) * distance);
		Vector3 sv = target.forward * (-Mathf.Cos(rad) * distance);
		Vector3 cdir = cv + sv;
		
		// пускаем луч от цели к камере, чтобы убедиться что на пути нет
		// препятствий (магии нет, проверка осуществляется только по коллайдерам)
		RaycastHit hit;
		if ( Physics.Raycast(tp, cdir, out hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) )
		{
			// если луч нашел препрятствие, то корректируем дистанцию
			transform.position = tp + cdir * (hit.distance / distance);
		}
		else
		{
			// если препятствий нет, то полное расстояние
			transform.position = tp + cdir;
		}
		
		// смотреть на заданную точку.
		// У функции LookAt() есть второй необязательный аргумент и возможно
		// его потребуется учитывать в некоторых случаях (если ваш персонаж,
		// будет летать во всевозможных направлениях)
		transform.LookAt(tp);
	}
	
} // class CameraFollow

} // namespace Nanosoft
