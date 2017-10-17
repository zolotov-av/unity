using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Класс описывающий один из этапов квеста
 */
public class QuestStage
{
	
	/**
	 * Название этапа, которое будет отображаться в описании квеста
	 */
	public string stageName;
	
	/**
	 * Флаг доступности этапа
	 * Если true, то этап доступен игроку и отображается в описании квеста
	 */
	public bool visible;
	
	/**
	 * Флаг заверщения этапа
	 * Если true, то этап засчитан как выполнен
	 */
	public bool complited;
	
} // class QuestStage

} // namespace Nanosoft
