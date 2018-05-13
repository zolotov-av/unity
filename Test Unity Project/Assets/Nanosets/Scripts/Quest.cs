using System;
using System.Reflection;
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
	 * Список триггеров (здесь и триггеры и переменные)
	 */
	private Dictionary<string, MethodInfo> triggers;
	
	/**
	 * Инициализация списка триггеров
	 */
	protected void InitTriggers(Type type)
	{
		triggers.Clear();
		MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		foreach(MethodInfo method in methods)
		{
			ParameterInfo[] argv = method.GetParameters();
			int argc = argv.Length;
			
			if ( !method.IsPublic ) continue;
			
			string name = method.Name;
			
			if ( name.EndsWith("Trigger") )
			{
				if ( name.StartsWith("Set") || name.StartsWith("Reset") )
				{
					if ( argc == 0 ) triggers.Add(name, method);
				}
				
				continue;
			}
			
			if ( name.EndsWith("Var") )
			{
				if ( name.StartsWith("Get") )
				{
					if ( argc == 0 ) triggers.Add(name, method);
					continue;
				}
				
				if ( name.StartsWith("Set") )
				{
					if ( argc == 1 ) triggers.Add(name, method);
					continue;
				}
				
				continue;
			}
		}
	}
	
	/**
	 * Вернуть значение квестовой переменной
	 */
	public virtual string GetQuestVar(string key)
	{
		MethodInfo method;
		if ( triggers.TryGetValue("Get" + key + "Var", out method) )
		{
			return (string) method.Invoke(this, null);
		}
		Debug.LogWarning("GetQuestVar(" + key + ") var not found");
		return "";
	}
	
	/**
	 * Установить значение квестовой переменной
	 */
	public virtual bool SetQuestVar(string key, string value)
	{
		MethodInfo method;
		if ( triggers.TryGetValue("Set" + key + "Var", out method) )
		{
			method.Invoke(this, new object[]{value});
			return true;
		}
		Debug.LogWarning("SetQuestVar(" + key + ") var not found");
		return false;
	}
	
	/**
	 * Установить/вызвать триггер
	 */
	public virtual void SetTrigger(string trigger)
	{
		MethodInfo method;
		if ( triggers.TryGetValue("Set" + trigger + "Trigger", out method) )
		{
			method.Invoke(this, null);
		}
		else
		{
			Debug.LogWarning("SetTrigger(" + trigger + ") var not found");
		}
	}
	
	/**
	 * Установить/вызвать триггер
	 */
	public virtual void ResetTrigger(string trigger)
	{
		MethodInfo method;
		if ( triggers.TryGetValue("Reset" + trigger + "Trigger", out method) )
		{
			method.Invoke(this, null);
		}
		else
		{
			Debug.LogWarning("ResetTrigger(" + trigger + ") var not found");
		}
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
	
	protected void Awake()
	{
		// TODO
		questLog = new string[10];
		triggers = new Dictionary<string, MethodInfo>();
	}
	
} // class Quest

} // namespace Nanosoft
