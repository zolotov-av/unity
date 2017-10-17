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
	 * Ссылка на менеджер квестов
	 */
	public QuestManager questManager;
	
	void Start()
	{
		GetComponent<Image>().enabled = false;
	}
	
	/**
	 * Обновить список квестов
	 */
	public void Refresh()
	{
		Quest[] quests = questManager.quests;
		int len = quests.Length;
		string text = "";
		for(int i = 0; i < len; i++)
		{
			text += quests[i].questName + "\n";
		}
		transform.Find("Text").GetComponent<Text>().text = text;
	}
	
} // class QuestList

} // namespace Nanosoft
