using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Класс описывающий квест
 *
 * Квест может состоять из серии этапов или из одного единственного этапа
 */
public class Quest: MonoBehaviour
{
	
	/**
	 * Имя квеста (идентификатор)
	 */
	public string questName = "";
	
	/**
	 * Имя квеста (отображаемое игроку)
	 */
	public string questTitle = "";
	
	/**
	 * Краткое текстовое описание квеста
	 */
	public string description = "";
	
	/**
	 * Флаг доступности квеста
	 * Если true, то квест доступен игроку, игрок может принять и выполнить
	 * квест
	 */
	public bool visible;
	
	/**
	 * Флаг активности квеста
	 * Если true, то игрок принял квест и квест находиться в состоянии
	 * выполнения
	 */
	public bool active;
	
	/**
	 * Флаг завершения квеста (успех)
	 * Если true, то квест успешно выполнен
	 */
	public bool complited;
	
	/**
	 * Флаг завершения квеста (провал)
	 * Если true, то квест провален
	 */
	public bool failed;
	
	/**
	 * Список этапов квеста
	 */
	public QuestStage[] stages;
	
	/**
	 * Журнал квеста
	 */
	public string[] questLog;
	
	/**
	 * Вернуть значение квестовой переменной
	 */
	public virtual string GetQuestVar(string key)
	{
		return "";
	}
	
	/**
	 * Установить значение квестовой переменной
	 */
	public virtual bool SetQuestVar(string key, string value)
	{
		return false;
	}
	
	/**
	 * Установить/вызвать триггер
	 */
	public virtual void SetTrigger(string trigger)
	{
	}
	
	/**
	 * Установить/вызвать триггер
	 */
	public virtual void ResetTrigger(string trigger)
	{
	}
	
	/**
	 * Сделать записть в журнал квеста
	 */
	public bool logEvent(string message, string note = null)
	{
		int len = questLog.Length;
		for(int i = 0; i < len; i++)
		{
			if ( questLog[i] == null )
			{
				questLog[i] = message;
				CanvasScript.Zone(note == null ? "Новая запись в дневнике" : note);
				QuestManager.WriteSound();
				return true;
			}
		}
		
		CanvasScript.Zone("Ошибка! журнал заданий переполнен");
		QuestManager.WriteSound();
		return false;
	}
	
	/**
	 * Обработчик события для кнопки.
	 *
	 * Возможно не самое лучше место для этой функции, но пока лучшего не
	 * придумал
	 */
	public void ShowQuestLog()
	{
		QuestManager.ShowQuestLog(this);
	}
	
	void Awake()
	{
		questLog = new string[10];
	}
	
} // class Quest

} // namespace Nanosoft
