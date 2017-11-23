using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Менеджер касаний
 */
public class TouchManager
{
	
	/**
	 * Трекеры
	 */
	public TouchTracker[] touches;
	
	/**
	 * Конструктор
	 */
	public TouchManager(int count)
	{
		touches = new TouchTracker[count];
		for(int i = 0; i < count; i++)
		{
			touches[i] = new TouchTracker();
		}
	}
	
	/**
	 * Выделить не занятый трекер
	 */
	public TouchTracker AllocTracker(int fingerId)
	{
		int count = touches.Length;
		for(int i = 0; i < count; i++)
		{
			var touch = touches[i];
			if ( ! touch.active )
			{
				touch.active = true;
				touch.pointerId = fingerId;
				return touch;
			}
		}
		
		return null;
	}
	
	/**
	 * Найти активный трекер по fingerId
	 */
	public TouchTracker GetTracker(int fingerId)
	{
		int count = touches.Length;
		for(int i = 0; i < count; i++)
		{
			var touch = touches[i];
			if ( touch.pointerId == fingerId && touch.active ) return touch;
		}
		
		return null;
	}
	
	/**
	 * Освободить трекер
	 */
	public void FreeTracker(int fingerId)
	{
		var touch = GetTracker(fingerId);
		if ( touch != null ) touch.active = false;
	}
	
} // class TouchManager

} // namespace Nanosoft