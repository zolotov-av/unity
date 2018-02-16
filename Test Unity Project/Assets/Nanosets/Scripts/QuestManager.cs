using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Менеджер квестов
 */
public class QuestManager: MonoBehaviour
{
	
	/**
	 * Ссылка на актуальный экземпляр менеджера квестов
	 */
	protected static QuestManager instance;
	
	/**
	 * Ссылка на AudioSource - звук записи в журнал
	 */
	protected AudioSource clip;
	
	/**
	 * Список квестов
	 */
	public Quest[] quests;
	
	protected void Init()
	{
		clip = GetComponent<AudioSource>();
		quests = new Quest[10];
		
		int i = 0;
		foreach(Transform child in transform)
		{
			Quest quest = child.GetComponent<Quest>();
			if ( quest != null )
			{
				Debug.Log("QuestManager child quest: " + quest.questName);
				quests[i] = quest;
				i++;
			}
		}
	}
	
	void Awake()
	{
		if ( instance == null )
		{
			instance = this;
			Init();
		}
		else
		{
			Debug.LogError("Only one QuestManager is allowed");
		}
	}
	
	/**
	 * Начать диалог
	 */
	public static void StartDialog(string avatarName, Sprite avatarPhoto, DialogItem dialog)
	{
		CanvasScript.SetAvatar(avatarName, avatarPhoto);
		CanvasScript.ShowDialog(dialog);
	}
	
	/**
	 * Запустить ответное действие
	 *
	 * Данная функция вызывается при нажатии соответствующей кнопки в диалоговом
	 * окне.
	 */
	public static void RunAction(DialogAction reply)
	{
		if ( reply.actionTrigger != null && reply.actionTrigger != "" )
		{
			Debug.Log("actionQuest: " + reply.actionQuest + ", reply.actionTrigger: " + reply.actionTrigger);
			QuestManager.SetTrigger(reply.actionQuest, reply.actionTrigger);
		}
		
		switch ( reply.actionType )
		{
		
		case DialogAction.ExitDialog:
			CanvasScript.instance.CloseDialog();
			break;
		
		case DialogAction.GotoNext:
			CanvasScript.ShowDialog(reply.nextDialog);
			break;
		
		default:
			Debug.LogError("Unknown DialogAction's type: " + reply.actionType);
			break;
		
		}
	}
	
	/**
	 * Вернуть значение квестовой переменной
	 */
	public static string GetQuestVar(string questName, string key)
	{
		foreach(Quest quest in instance.quests)
		{
			if ( quest == null ) continue;
			if ( quest.questName == questName )
			{
				return quest.GetQuestVar(key);
			}
		}
		
		return "";
	}
	
	/**
	 * Установить значение квестовой переменной
	 */
	public static bool SetQuestVar(string questName, string key, string value)
	{
		foreach(Quest quest in instance.quests)
		{
			if ( quest == null ) continue;
			if ( quest.questName == questName )
			{
				return quest.SetQuestVar(key, value);
			}
		}
		
		return false;
	}
	
	/**
	 * Установить/запустить триггер
	 */
	public static void SetTrigger(string questName, string trigger)
	{
		foreach(Quest quest in instance.quests)
		{
			if ( quest == null ) continue;
			if ( quest.questName == questName )
			{
				quest.SetTrigger(trigger);
			}
		}
	}
	
	/**
	 * Сбросить триггер
	 */
	public static void ResetTrigger(string questName, string trigger)
	{
		foreach(Quest quest in instance.quests)
		{
			if ( quest == null ) continue;
			if ( quest.questName == questName )
			{
				quest.ResetTrigger(trigger);
				return;
			}
		}
		
		Debug.LogError("quest (" + questName + ") not found");
	}
	
	public static void WriteSound()
	{
		AudioSource c = instance.clip;
		if ( c != null )
		{
			c.Stop();
			c.Play();
		}
	}
	
} // class QuestManager

} // namespace Nanosoft
