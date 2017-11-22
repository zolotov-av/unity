using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Трекер касаний
 */
public class TouchTracker
{
	/**
	 * Флаг активности трекера
	 */
	public bool active = false;
	
	/**
	 * Идентификатор касания
	 */
	public int fingerId;
	
	/**
	 * Текущая позиция касания
	 */
	public Vector2 position;
	
	/**
	 * Начальная позиция касания
	 */
	public Vector2 startPosition;
	
	/**
	 * Дельта позиции касания
	 */
	public Vector2 deltaPosition;
	
	/**
	 * Время начала касания
	 */
	public float startTime = 0f;
	
	/**
	 * Пользовательская метка
	 */
	public int tag = 0;
	
	/**
	 * Начать ослеживание касания
	 */
	public void BeginTrack(Vector2 point)
	{
		active = true;
		startTime = Time.unscaledTime;
		startPosition = point;
		position = point;
		deltaPosition = Vector2.zero;
	}
	
	/**
	 * Обработать касание
	 */
	public void Track(Vector2 point)
	{
		deltaPosition = point - position;
		position = point;
	}
	
	/**
	 * Завершить ослеживание касания
	 */
	public void EndTrack()
	{
		active = false;
	}
	
} // class TouchTracker

} // namespace Nanosoft
