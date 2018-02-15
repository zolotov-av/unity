using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class QueryChan: MonoBehaviour
{
	
	/**
	 * Ссылка на аниматор персонажа
	 */
	private Animator animator;
	
	/**
	 * Время ожидания для смены анимации
	 */
	private float waitTime = 2f;
	
	/**
	 * Флаг занятости (диалог)
	 */
	private bool m_busy = false;
	
	const float min_wait = 10f;
	const float max_wait = 20f;
	
	public bool busy
	{
		get { return m_busy; }
		set {
			if ( m_busy && !value )
			{
				waitTime = Time.time + Random.Range(min_wait, max_wait);
			}
			m_busy = value;
		}
	}
	
	void Start ()
	{
		animator = GetComponentInChildren<Animator>();
		if ( animator == null )
		{
			Debug.LogError("QueryChan animator not found");
		}
		
		waitTime = Time.time + Random.Range(min_wait, max_wait);
	}
	
	void Update ()
	{
		if ( !busy && Time.time > waitTime )
		{
			animator.SetTrigger("wait1");
			waitTime = Time.time + Random.Range(min_wait, max_wait);
		}
	}
	
}

} // namespace Nanosoft
