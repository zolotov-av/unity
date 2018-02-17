using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Квест "Меч героя"
 */
public class QuestLostSword: Quest
{
	
	private string found = "start";
	
	/**
	 * Вернуть значение квестовой переменной
	 */
	public override string GetQuestVar(string key)
	{
		if ( key == "found" )
		{
			return found;
		}
		
		return "";
	}
	
	/**
	 * Установить значение квестовой переменной
	 */
	public override bool SetQuestVar(string key, string value)
	{
		if ( key == "found" )
		{
			found = value;
			return true;
		}
		
		return false;
	}
	
	/**
	 * Установить/вызвать триггер
	 */
	public override void SetTrigger(string trigger)
	{
		if ( trigger == "accept-quest" )
		{
			AcceptTrigger();
			return;
		}
		
		if ( trigger == "end-quest" )
		{
			EndTrigger();
			return;
		}
		
		Debug.LogError("QuestLostSword: trigger (" + trigger + ") not found");
	}
	
	public void AcceptTrigger()
	{
		found = "no";
		CanvasScript.Zone("Добавлен квест");
		active = true;
		QuestManager.WriteSound();
		QuestManager.Refresh();
		LostSwordTrigger.Enable();
	}
	
	public void EndTrigger()
	{
		found = "done";
		CanvasScript.Zone("Квест завершен");
		active = false;
		QuestManager.WriteSound();
		QuestManager.Refresh();
		LostSwordTrigger.Disable();
	}
	
}

} // namespace Nanosoft
