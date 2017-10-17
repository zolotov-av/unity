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
[System.Serializable]
public class Quest
{
	
	/**
	 * Имя квеста, которое будет отображаться в списке квестов
	 */
	public string questName;
	
	/**
	 * Краткое текстовое описание квеста
	 */
	public string description;
	
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
	
} // class Quest

} // namespace Nanosoft
