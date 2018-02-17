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
	private int capacity = 0;
	
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
	
	public void Init()
	{
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
	
	/**
	 * Обновить список квестов
	 */
	public void Refresh()
	{
		if ( questManager == null ) return;
		
		int i = 0;
		foreach(Quest quest in questManager.quests)
		{
			if ( i >= capacity ) break;
			if ( quest == null || !quest.active ) continue;
			
			var obj = items[i];
			obj.transform.Find("Text").GetComponent<Text>().text = quest.questTitle;
			obj.SetActive(true);
			i++;
		}
		
		for(int j = i; j < count; j++)
		{
			items[j].SetActive(false);
		}
		count = i;
	}
	
} // class QuestList

} // namespace Nanosoft
