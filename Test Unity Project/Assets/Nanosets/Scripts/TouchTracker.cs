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
	 * Дельта позиции касания
	 */
	public Vector2 deltaPosition;
	
	/**
	 * Флаг движения
	 * true - было движение, false - движения не было
	 */
	public bool moved = false;
	
	/**
	 * Пользовательская метка
	 */
	public int tag = 0;
	
} // class TouchTracker

} // namespace Nanosoft
