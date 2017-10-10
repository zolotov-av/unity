using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanosoft
{

/**
 * Базовый класс для контроллеров персонажей
 */
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController: MonoBehaviour
{
	
	/**
	 * Тег для телепортации
	 *
	 * Данный тег задает объект на место которого должен переместиться персонаж.
	 */
	private string teleportTag = "";
	
	/**
	 * Флаг блокировки курсора
	 *
	 * Используйте это поле только для чтения, для блокировки/разблокировки
	 * используйте метод lockCursor()
	 */
	protected bool cursorLocked = false;
	
	/**
	 * Заблокировать/разблокировать курсор
	 */
	public void lockCursor(bool state)
	{
		if ( state )
		{
			if ( ! cursorLocked )
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				cursorLocked = true;
			}
		}
		else
		{
			if ( cursorLocked )
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				cursorLocked = false;
			}
		}
	}
	
	/**
	 * Установить метку телепортации
	 *
	 * После переключения сцены персонаж будет телепортирован на указанное
	 * место. Место и разворот персонажа определяется объектом с указанным тегом
	 */
	public void SetTeleportTag(string tag)
	{
		teleportTag = tag;
	}
	
	/**
	 * Выполнить телепортацию персонажа на ранее сохраненную метку.
	 */
	public void TeleportPlayer()
	{
		if ( teleportTag == "" || teleportTag == null )
		{
			// если метки нет, то телепортировать не надо.
			return;
		}
		
		GameObject targetPoint = GameObject.FindWithTag(teleportTag);
		
		if ( !targetPoint )
		{
			// упс! мы не нашли объект, возможно мы забыли присвоить объекту
			// метку. Бывает... напишем об этом в лог, чтобы было понятнее
			// в чем проблема
			Debug.Log("Object with Tag='" + teleportTag + "' not found");
			
			// сбрасываем метку телепорта
			teleportTag = "";
			return;
		}
		
		// сбрасываем метку телепорта
		teleportTag = "";
		
		// перемещаем персонажа в точку где находиться объект EntryPoint
		transform.position = targetPoint.transform.position;
		
		// развернем персонажа, чтобы он был ориентирован в том же направлении,
		// что и объект EntryPoint
		transform.rotation = targetPoint.transform.rotation;
	}
	
	/**
	 * Переместить персонажа на другую сцену или телепортировать в пределах
	 * текущей сцены. targetScene задает имя сцены на которую надо
	 * переключиться, targetTag задает место в новой сцене куда надо
	 * переместить персонажа. Если targetScene пусто, то телепортация
	 * осуществляется в пределах текущей сцены без перезагрузки сцены.
	 * Если targetTag пустой, то персонаж не будет никуда перемещаться после
	 * загрузки новой сцены, а останется в тех координатах, что он был на
	 * старой сцене.
	 *
	 */
	public void TeleportPlayer(string targetScene, string targetTag)
	{
		// указать персонажу метку на которую он должен будет телепортироваться
		// после загрузки сцены.
		teleportTag = targetTag;
		
		// перемещаем персонажа
		if ( targetScene != "" || targetScene == null )
		{
			// если сцена указана, то загружаем сцену
			SceneManager.LoadScene(targetScene);
		}
		else
		{
			// если сцена не указана, то просто телепортируем персонажа
			// на указанную метку
			TeleportPlayer();
		}
	}
	
} // class PlayerController

} // namespace Nanosoft
