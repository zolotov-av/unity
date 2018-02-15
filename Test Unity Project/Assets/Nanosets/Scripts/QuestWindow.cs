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
	
	public void SetQuestManager(QuestManager qm)
	{
		questManager = qm;
		questList.questManager = qm;
	}
	
	/**
	 * Включить/выключить список квестов
	 */
	public void ToggleWindow()
	{
		gameObject.SetActive( !gameObject.activeSelf );
	}
	
	/**
	 * Обновить список квестов
	 */
	public void Refresh()
	{
		questList.Refresh();
	}
	
} // class QuestWindow

} // namespace Nanosoft
