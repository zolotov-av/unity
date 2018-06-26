using System.Collections;
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
public class PlayerBehaviour: Character
{
	
	protected IAction action;
	
	/**
	 *
	 */
	private Collider[] enemies = null;
	
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
	
	/**
	 * Инициализация внутренних структур
	 */
	protected new void Init()
	{
		base.Init();
		if ( enemies == null ) enemies = new Collider[32];
	}
	
	/**
	 * Найти врагов вызвать их агрессию
	 */
	protected void AggroEnemies()
	{
		const float r = 20f;
		const int layerMask = 1 << 11; // TODO no hardcode
		int count = Physics.OverlapSphereNonAlloc(transform.position, r, enemies, layerMask, QueryTriggerInteraction.Ignore);
		for(int i = 0; i < count; i++)
		{
			EnemyBehaviour enemy = enemies[i].GetComponent<EnemyBehaviour>();
			if ( enemy == null )
			{
				Debug.LogWarning("Enemy[" + enemies[i].gameObject.name + "] without EnemyBehaviour");
				continue;
			}
			
			enemy.CheckAggro(this);
		}
	}
	
	public int GetCurrentHealth()
	{
		return currentHealth;
	}
	
	public virtual void ApplyDamage(int damage)
	{
	}
	
	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.E) && action != null )
		{
			action.RunAction(this);
		}
		
		AggroEnemies();
	}
	
} // class PlayerBehaviour

public interface IAction
{
	
	string GetActionMessage();
	
	void RunAction(PlayerBehaviour pb);
}

} // namespace Nanosoft
