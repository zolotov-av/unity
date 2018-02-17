using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Класс управляющий окном списка квестов
 */
public class QuestWindow: WindowBehaviour
{
	
	/**
	 * Ссылка на менеджер квестов
	 */
	protected QuestManager questManager;
	
	/**
	 * Ссылка на список квестов
	 */
	public QuestList questList;
	
	/**
	 * Ссылка на панельку с пояснением что квестов нет
	 */
	public GameObject noQuestInfo;
	
	public void Init(QuestManager qm)
	{
		Debug.Log("QuestWindow.Init()");
		questManager = qm;
		questList.questManager = qm;
		questList.Init();
		Refresh();
	}
	/*
	public void SetQuestManager(QuestManager qm)
	{
		questManager = qm;
		questList.questManager = qm;
	}
	*/
	/**
	 * Обновить список квестов
	 */
	public void Refresh()
	{
		if ( questManager.GetActiveQuests() > 0 )
		{
			noQuestInfo.SetActive(false);
			questList.Refresh();
			questList.gameObject.SetActive(true);
		}
		else
		{
			questList.gameObject.SetActive(false);
			noQuestInfo.SetActive(true);
		}
		
	}
	
} // class QuestWindow

} // namespace Nanosoft
