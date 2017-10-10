using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Простой телепорт
 *
 * Простой класс реализующий переход между сценами или телепортацию в пределах
 * сцены. Для работы класса требуется любой коллайдер в режиме треггера,
 * как только персонаж войдет в триггер, он будет перемещен на новую сцену
 * и телепортиван к точке заданной тегом.
 *
 * Скрипт SimpleTeleport работает совместно с классами PlayerController и
 * GameStateBehaviour. Для работы скрипта надо чтобы в игре присутствовал
 * экземпляр производный от GameStateBehaviour, а персонаж имел скрипт
 * производный от PlayerController.
 */
public class SimpleTeleport: MonoBehaviour
{
	
	/**
	 * Имя сцены на которую надо переместить персонажа
	 *
	 * Если имя сцены пустое, то телепортация будет произведена в пределах
	 * текущей сцены.
	 */
	public string targetScene = "";
	
	/**
	 * Имя тега для поиска объекта, на месте которого должен появиться
	 * персонаж.
	 */
	public string targetPointTag = "EntryPoint";
	
	/**
	 * Обработчик триггера, тут мы сохраняем ссылку на персонажа (и нужные ему
	 * объекты) в свое поле. Отмечаем все объекты которые надо переместить
	 * на новую карту как неудаляемые.
	 */
	public void OnTriggerEnter(Collider other)
	{
		// игрок сталкивается с триггером, так ссылку на игрока получаем
		// из аргумента
		var player = other.gameObject;
		
		// получим ссылку на скрипт, за одно проверим, это наш персожан
		// или что-то другое врезалось в триггер?
		var controller = player.GetComponent<PlayerController>();
		
		// если контроллер не найдет, значит это скорее всего не игрок,
		// а какой-то другой объект врезался в триггер
		if ( controller )
		{
			// телепортируем персонажа
			controller.TeleportPlayer(targetScene, targetPointTag);
		}
	}
	
} // SimpleTeleport

} // namespace Nanosoft
