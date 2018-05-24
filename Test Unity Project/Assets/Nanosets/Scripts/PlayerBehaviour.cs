﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Базовый класс представляющий персонажа как игровую сущность
 *
 * Данный класс предоставляет абстрактный интерфейс через который игровой
 * мир может воздействовать на персонажа.
 */
public class PlayerBehaviour: MonoBehaviour
{
	
	private IAction action;
	
	/**
	 * true - персонаж занят (анимация удара или заморожен)
	 * false - персонаж свободен (им можно управлять)
	 */
	public bool busy = false;
	
	public void SetAction(IAction act)
	{
		if ( act == null ) return;
		
		action = act;
		CanvasScript.ShowAction(action);
	}
	
	public void RemoveAction(IAction act)
	{
		if ( act == null ) return;
		if ( action != act ) return;
		
		action = null;
		CanvasScript.HideAction();
	}
	
	public void RemoveAction()
	{
		action = null;
		CanvasScript.HideAction();
	}
	
	public virtual void Attack1()
	{
	}
	
	public virtual void Attack2()
	{
	}
	
	public void RunActionTouch()
	{
		action.RunAction(this);
	}
	
	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.E) && action != null )
		{
			action.RunAction(this);
		}
	}
	
} // class PlayerBehaviour

public interface IAction
{
	
	string GetActionMessage();
	
	void RunAction(PlayerBehaviour pb);
}

} // namespace Nanosoft
