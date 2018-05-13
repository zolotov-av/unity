using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nanosoft;

public class LostSwordTrigger: MonoBehaviour, IAction
{
	public Collider trigger;
	
	private static LostSwordTrigger instance;
	private static bool active = false;
	public static bool swordActive = true;
	
	public GameObject sword;
	
	void OnEnable()
	{
		instance = this;
		trigger.enabled = active;
		sword.SetActive(swordActive);
	}
	
	void OnDisable()
	{
		instance = null;
	}
	
	public static void Enable()
	{
		active = true;
		if ( instance != null )
		{
			instance.trigger.enabled = true;
		}
	}
	
	public static void Finish()
	{
		active = false;
		swordActive = false;
		if ( instance != null )
		{
			instance.trigger.enabled = false;
		}
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
		QuestManager.SetQuestVar("lost-sword", "Found", "yes");
		const string msg = "Кажется вы нашли \"Меч героя\", покажите его Квери-чан";
		QuestManager.logEvent("lost-sword", msg);
		QuestManager.Refresh();
		CanvasScript.Zone(msg, 3.4f);
	}
	
}
