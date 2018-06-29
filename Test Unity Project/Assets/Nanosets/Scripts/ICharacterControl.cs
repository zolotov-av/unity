using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Интерфейс контроллера персонажа
 */
public interface ICharacterControl
{
	
	/**
	 * Событие остановки навигации
	 */
	void OnNavigationStop();
	
	/**
	 * Начало фактического движения NavMeshAgent
	 */
	void OnNavigationStartMoving();
	
	/**
	 * Конец фактического движения NavMeshAgent
	 */
	void OnNavigationStopMoving();
	
}

} // namespace Nanosoft
