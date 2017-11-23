using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Nanosoft
{

/**
 * Трекер касаний
 */
public class TouchTracker: PointerEventData
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
	 * Начальная позиция касания
	 */
	public Vector2 startPosition;
	
	/**
	 * Время начала касания
	 */
	public float startTime = 0f;
	
	/**
	 * Пользовательская метка
	 */
	public int tag = 0;
	
	/**
	 * Объект над которым находиться мышка/касание
	 */
	public GameObject hoverUI;
	
	/**
	 * Объект над которым находиться мышка/касание
	 */
	protected Selectable hover;
	
	/**
	 * Целевой объект
	 */
	protected Selectable target;
	
	/**
	 * RectTransform целевого объекта
	 */
	protected RectTransform targetRT;
	
	/**
	 * В этом фрейме кнопка была отпущена
	 */
	public bool up;
	
	/**
	 * В этом фрейме кнопка была зажата
	 */
	public bool down;
	
	/**
	 * Конструктор
	 */
	public TouchTracker(): base(EventSystem.current)
	{
	}
	
	/**
	 * Начать ослеживание касания
	 */
	public void BeginTrack(Vector2 point)
	{
		active = true;
		startTime = Time.unscaledTime;
		startPosition = point;
		position = point;
		delta = Vector2.zero;
	}
	
	/**
	 * Обработать касание
	 */
	public void Track(Vector2 point)
	{
		delta = point - position;
		position = point;
	}
	
	/**
	 * Завершить ослеживание касания
	 */
	public void EndTrack()
	{
		active = false;
	}
	
	/**
	 * Найти элемент UI над которым находиться курсор/палец
	 */
	protected GameObject RaycastUI()
	{
		List<RaycastResult> rc_list = new List<RaycastResult>(); 
		EventSystem.current.RaycastAll(this, rc_list);
		
		int count = rc_list.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject obj = rc_list[i].gameObject;
			if ( obj != null )
			{
				rc_list.Clear();
				return obj;
			}
		}
		
		rc_list.Clear();
		return null;
	}
	
	/**
	 * Найти элемент (Selectable) над которым находиться курсор/палец
	 */
	protected Selectable GetHover()
	{
		if ( active )
		{
			hoverUI = null;
			if ( RectTransformUtility.RectangleContainsScreenPoint(targetRT, position, null) )
			{
				return target;
			}
			
			return null;
		}
		else
		{
			hoverUI = RaycastUI();
			if ( hoverUI == null ) return null;
			return hoverUI.GetComponentInParent<Selectable>();
		}
	}
	
	/**
	 * Обработка OnPointerEnter() и OnPointerExit() для мышки
	 */
	protected void TrackMouseHover()
	{
		Selectable oldHover = hover;
		Selectable newHover = GetHover();
		
		if ( newHover != oldHover )
		{
			if ( oldHover != null )
			{
				oldHover.OnPointerExit(this);
				Debug.Log("OnPointerExit executed on " + oldHover.gameObject.ToString());
			}
			
			hover = newHover;
			
			if ( newHover != null )
			{
				newHover.OnPointerEnter(this);
				Debug.Log("OnPointerEnter executed on " + newHover.gameObject.ToString());
			}
		}
	}
	
	/**
	 * Обработать состояние мышки
	 */
	public void TrackMouse()
	{
		Vector2 point = Input.mousePosition;
		delta = point - position;
		position = point;
		up = Input.GetMouseButtonUp(0);
		down = Input.GetMouseButtonDown(0);
		
		TrackMouseHover();
		
		if ( active )
		{
			if ( up )
			{
				target.OnPointerUp(this);
				Debug.Log("OnPointerUp executed on " + target.gameObject.ToString());
				
				if ( hover != null )
				{
					ExecuteEvents.Execute<IPointerClickHandler>(hover.gameObject, this, ExecuteEvents.pointerClickHandler);
					Debug.Log("OnPointerClick executed on " + hover.gameObject.ToString());
				}
				
				active = false;
				target = null;
				targetRT = null;
			}
		}
		else
		{
			if ( down && hover != null )
			{
				active = true;
				target = hover;
				targetRT = target.gameObject.transform as RectTransform;
				target.OnPointerDown(this);
				Debug.Log("OnPointerDown executed on " + target.gameObject.ToString());
			}
		}
	}
	
} // class TouchTracker

} // namespace Nanosoft
