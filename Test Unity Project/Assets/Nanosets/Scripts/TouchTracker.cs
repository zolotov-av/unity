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
	 * Системный трекер
	 *
	 * Система использует этот трекер для обработки событий UI
	 */
	public bool system = false;
	
	/**
	 * Пользовательский трекер
	 *
	 * Пользователь может использовать этот трекер не беспокоясь, о конфликтах
	 * с системой
	 */
	public bool user = false;
	
	/**
	 * Время начала касания
	 */
	public float startTime = 0f;
	
	/**
	 * Метка для пользователя
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
		position = point;
		delta = Vector2.zero;
		startTime = Time.unscaledTime;
		tag = 0;
		up = false;
		down = false;
		
		hoverUI = RaycastUI();
		if ( hoverUI != null )
		{
			system = true;
			user = false;
			
			hover = hoverUI.GetComponentInParent<Selectable>();
			if ( hover == null )
			{
				target = null;
				targetRT = null;
			}
			else
			{
				target = hover;
				targetRT = target.gameObject.transform as RectTransform;
				target.OnPointerEnter(this);
				target.OnPointerDown(this);
			}
		}
		else
		{
			system = false;
			user = true;
			
			hover = null;
			target = null;
			targetRT = null;
		}
	}
	
	/**
	 * Обработать касание
	 */
	public void Track(Vector2 point)
	{
		delta = point - position;
		position = point;
		
		if ( targetRT != null ) TrackMouseHover();
	}
	
	/**
	 * Завершить ослеживание касания
	 */
	public void EndTrack()
	{
		if ( targetRT != null )
		{
			target.OnPointerUp(this);
			
			if ( hover != null )
			{
				hover.OnPointerExit(this);
				ExecuteEvents.Execute<IPointerClickHandler>(hover.gameObject, this, ExecuteEvents.pointerClickHandler);
			}
		}
		
		active = false;
		system = false;
		user = false;
		target = null;
		targetRT = null;
		hover = null;
		hoverUI = null;
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
			}
			
			hover = newHover;
			
			if ( newHover != null )
			{
				newHover.OnPointerEnter(this);
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
				
				if ( hover != null )
				{
					ExecuteEvents.Execute<IPointerClickHandler>(hover.gameObject, this, ExecuteEvents.pointerClickHandler);
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
			}
		}
	}
	
} // class TouchTracker

} // namespace Nanosoft
