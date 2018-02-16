using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nanosoft;

public class LostSwordTrigger: MonoBehaviour, IAction
{
	public Collider trigger;
	
	public static LostSwordTrigger instance;
	
	public GameObject sword;

	// Use this for initialization
	void Awake()
	{
		if ( instance == null )
		{
			instance = this;
		}
	}
	
	public static void Enable()
	{
		Debug.Log("enable trigger");
		instance.trigger.enabled = true;
	}
	
	public static void Disable()
	{
		instance.trigger.enabled = false;
	}
	
	/**
	 * Обработчик триггера, тут мы сохраняем ссылку на персонажа (и нужные ему
	 * объекты) в свое поле. Отмечаем все объекты которые надо переместить
	 * на новую карту как неудаляемые.
	 */
	public void OnTriggerEnter(Collider other)
	{
		var pb = other.GetComponent<PlayerBehaviour>();
		if ( pb )
		{
			pb.SetAction(this);
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		var pb = other.GetComponent<PlayerBehaviour>();
		if ( pb )
		{
			pb.RemoveAction(this);
		}
	}
	
	public string GetActionMessage()
	{
		return "Тут что-то блестит, поднять?";
	}
	
	public void RunAction(PlayerBehaviour pb)
	{
		Debug.Log("LostSwordTrigger.RunAction()");
		pb.RemoveAction(this);
		sword.SetActive(false);
		trigger.enabled = false;
		QuestManager.SetQuestVar("lost-sword", "found", "yes");
		QuestManager.WriteSound();
		CanvasScript.Zone("Кажется вы нашли \"Меч героя\", покажите его Квери-чан", 3.4f);
	}
	
}
