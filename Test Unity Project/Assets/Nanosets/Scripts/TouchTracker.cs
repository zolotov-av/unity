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
	 * Ссылка на UI элемент определяющий зону касания
	 */
	public RectTransform rt;
	
	public bool active = false;
	
	public int fingerId;
	
	public float startTime;
	
	public float sensitivity = 1f;
	
	public Vector2 deltaPosition;
	
	public Vector2 position;
	
	public bool tap;
	public bool moved;
	
	public bool TouchIsFree(Touch touch)
	{
		if ( active )
		{
			if ( touch.fingerId == fingerId )
			{
				return false;
			}
		}
		
		if ( touch.phase == TouchPhase.Began )
		{
			if ( RectTransformUtility.RectangleContainsScreenPoint(rt, touch.position, null) )
			{
				return false;
			}
		}
		
		return true;
	}
	
	public void HandleTouch()
	{
		int count = Input.touchCount;
		if ( active )
		{
			for(int i = 0; i < count; i++)
			{
				var touch = Input.GetTouch(i);
				if ( touch.fingerId == fingerId )
				{
					if ( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled )
					{
						active = false;
						deltaPosition.x = 0f;
						deltaPosition.y = 0f;
						return;
					}
					
					int w = Screen.width;
					int h = Screen.height;
					float scale = 6000f / Mathf.Sqrt(w * w + h * h);
					deltaPosition = touch.deltaPosition * scale;
					return;
				}
			}
			
			active = false;
			deltaPosition.x = 0f;
			deltaPosition.y = 0f;
		}
		else
		{
			for(int i = 0; i < count; i++)
			{
				var touch = Input.GetTouch(i);
				if ( touch.phase == TouchPhase.Began )
				{
					if ( RectTransformUtility.RectangleContainsScreenPoint(rt, touch.position, null) )
					{
						active = true;
						fingerId = touch.fingerId;
						deltaPosition.x = 0f;
						deltaPosition.y = 0f;
						return;
					}
				}
			}
		}
	}
	
	/**
	 * Привязать тач
	 *
	 * Начать отслеживать тач по fingerId
	 */
	public void Bind(Touch touch)
	{
		tap = false;
		moved = false;
		active = true;
		fingerId = touch.fingerId;
		position = touch.position;
	}
	
	public void ProcessTouch()
	{
		if ( active )
		{
			int count = Input.touchCount;
			for(int i = 0; i < count; i++)
			{
				var touch = Input.GetTouch(i);
				if ( touch.fingerId == fingerId )
				{
					if ( touch.phase == TouchPhase.Ended )
					{
						tap = !moved;
						active = false;
						deltaPosition.x = 0f;
						deltaPosition.y = 0f;
						position = touch.position;
						return;
					}
					
					if ( touch.phase == TouchPhase.Canceled )
					{
						tap = false;
						active = false;
						deltaPosition.x = 0f;
						deltaPosition.y = 0f;
						position = touch.position;
						return;
					}
					
					if ( touch.phase == TouchPhase.Moved ) moved = true;
					if ( touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary )
					{
						int w = Screen.width;
						int h = Screen.height;
						float scale = 6000f / Mathf.Sqrt(w * w + h * h);
						
						deltaPosition = (touch.position - position) * scale;
						position = touch.position;
						return;
					}
					
					active = false;
					deltaPosition.x = 0f;
					deltaPosition.y = 0f;
					position = touch.position;
					return;
				}
			}
			
			active = false;
			deltaPosition.x = 0f;
			deltaPosition.y = 0f;
			return;
		}
	}
	
} // class TouchTracker

} // namespace Nanosoft
