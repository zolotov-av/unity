using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Класс управляющий окном списка квестов
 */
public class QuestWindow: WindowBehaviour
{
	
	/**
	 * Ссылка на панель списока квестов
	 */
	public GameObject questList;
	
	/**
	 * Ссылка на префаб элементов
	 */
	public GameObject questItemPrefab;
	
	/**
	 * Ссылка на панельку с пояснением что квестов нет
	 */
	public GameObject noQuestInfo;
	
	/**
	 * Ссылка на менеджер квестов
	 */
	protected QuestManager questManager;
	
	/**
	 * Максимальное число квестов
	 */
	public int capacity = 10;
	
	/**
	 * Актуальное число квестов в списке
	 */
	private int questCount = 0;
	
	/**
	 * Ссылки на элементы
	 */
	private GameObject[] questItems;
	
	/**
	 * Инициализация
	 */
	public void Init(QuestManager qm)
	{
		Debug.Log("QuestWindow.Init()");
		questManager = qm;
		InitQuestList();
		Refresh();
	}
	
	/**
	 * Инициализация списка квестов (элементов окна)
	 */
	protected void InitQuestList()
	{
		questCount = 0;
		questItems = new GameObject[capacity];
		Transform t = questList.transform;
		for(int i = 0; i < capacity; i++)
		{
			GameObject obj = Instantiate(questItemPrefab, t);
			obj.SetActive(false);
			questItems[i] = obj;
		}
	}
	
	/**
	 * Обновить список квестов
	 */
	public void Refresh()
	{
		if ( questManager.GetActiveQuests() > 0 )
		{
			noQuestInfo.SetActive(false);
			RefreshQuestList();
			questList.SetActive(true);
		}
		else
		{
			questList.SetActive(false);
			noQuestInfo.SetActive(true);
		}
		
	}
	
	protected void RefreshQuestList()
	{
		int i = 0;
		foreach(Quest quest in questManager.quests)
		{
			if ( i >= capacity ) break;
			if ( quest == null || !quest.active ) continue;
			
			var obj = questItems[i];
			obj.transform.Find("Text").GetComponent<Text>().text = quest.questTitle;
			obj.SetActive(true);
			i++;
		}
		
		for(int j = i; j < questCount; j++)
		{
			questItems[j].SetActive(false);
		}
		questCount = i;
	}
	
} // class QuestWindow

} // namespace Nanosoft
