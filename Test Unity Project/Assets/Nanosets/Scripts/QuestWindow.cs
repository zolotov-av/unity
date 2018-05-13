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
	 * Ссылка на панельку с журналом квеста
	 */
	public GameObject questLog;
	
	/**
	 * Ссылка на префаб элемента журнала квеста
	 */
	public GameObject questLogPrefab;
	
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
	 * Максимальное число записей в журнале квеста
	 */
	public int logCapacity = 10;
	
	/**
	 * Актуальное число записей в журнале квеста
	 */
	private int logCount = 0;
	
	/**
	 * Сслыка на элементы журнала квеста
	 */
	private GameObject[] logItems;
	
	/**
	 * Инициализация
	 */
	public void Init(QuestManager qm)
	{
		questManager = qm;
		InitQuestList();
		InitQuestLog();
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
	 * Инициализация журнала квеста
	 */
	protected void InitQuestLog()
	{
		logCount = 0;
		logItems = new GameObject[logCapacity];
		Transform t = questLog.transform;
		for(int i = 0; i < logCapacity; i++)
		{
			GameObject obj = Instantiate(questLogPrefab, t);
			obj.SetActive(false);
			logItems[i] = obj;
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
			ClearQuestLog();
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
			
			var btnClick = obj.GetComponent<Button>().onClick;
			btnClick.RemoveAllListeners();
			btnClick.AddListener( quest.ShowQuestLog );
			
			obj.SetActive(true);
			i++;
		}
		
		for(int j = i; j < questCount; j++)
		{
			questItems[j].SetActive(false);
		}
		questCount = i;
		
		if ( questCount > 0 )
		{
			// TODO something...
			questItems[0].GetComponent<Button>().onClick.Invoke();
		}
		else
		{
			ClearQuestLog();
		}
	}
	
	/**
	 * Отобразить журнал квеста
	 *
	 * Данная функция вызывается при нажатии соответствующей кнопки в журнале
	 * заданий
	 */
	public void ShowQuestLog(Quest quest)
	{
		int i = 0;
		foreach(string msg in quest.questLog)
		{
			if ( i >= logCapacity || msg == null ) break;
			
			var obj = logItems[i];
			obj.GetComponent<Text>().text = msg;
			
			obj.SetActive(true);
			i++;
		}
		
		for(int j = i; j < logCount; j++)
		{
			logItems[j].SetActive(false);
		}
		logCount = i;
	}
	
	/**
	 * Очистить журнал квеста
	 */
	public void ClearQuestLog()
	{
		for(int j = 0; j < logCount; j++)
		{
			logItems[j].SetActive(false);
		}
		logCount = 0;
	}
	
} // class QuestWindow

} // namespace Nanosoft
