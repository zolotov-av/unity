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
	
	private bool active = false;
	
	private int fingerId;
	
	public float sensitivity = 1f;
	
	[HideInInspector]
	public Vector2 deltaPosition;
	
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
	
} // class TouchTracker

} // namespace Nanosoft
