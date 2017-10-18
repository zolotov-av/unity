using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Скрипт управление UI-компонентом отображающим список квестов
 */
public class QuestList: MonoBehaviour
{
	
	/**
	 * Ссылка на префаб элементов
	 */
	public GameObject itemPrefab;
	
	/**
	 * Размер выделенного массива (кеш элементов)
	 */
	private int capacity;
	
	/**
	 * Фактическое число элементов в массиве
	 */
	private int count;
	
	/**
	 * Ссылки на элементы
	 */
	private GameObject[] items;
	
	/**
	 * Ссылка на менеджер квестов
	 */
	[HideInInspector]
	public QuestManager questManager;
	
	void Awake()
	{
		Debug.Log("QuestList.Awake()");
		
		capacity = 6;
		count = 0;
		items = new GameObject[capacity];
		Transform t = gameObject.transform;
		for(int i = 0; i < capacity; i++)
		{
			GameObject obj = Instantiate(itemPrefab, t);
			obj.SetActive(false);
			items[i] = obj;
		}
	}
	
	void Start()
	{
		Debug.Log("QuestList.Start()");
		Refresh();
	}
	
	/**
	 * Обновить список квестов
	 */
	public void Refresh()
	{
		if ( questManager == null ) return;
		
		// TODO resize and scrolling
		Quest[] quests = questManager.quests;
		int len = quests.Length;
		Debug.Log("QuestList.Refresh(), quests.count=" + len + ", capacity=" + capacity);
		if ( len > capacity ) len = capacity;
		for(int i = 0; i < len; i++)
		{
			var obj = items[i];
			obj.transform.Find("Text").gameObject.GetComponent<Text>().text = quests[i].questName;
			obj.SetActive(true);
		}
		for(int i = len; i < count; i++)
		{
			items[i].SetActive(false);
		}
		count = len;
	}
	
} // class QuestList

} // namespace Nanosoft
