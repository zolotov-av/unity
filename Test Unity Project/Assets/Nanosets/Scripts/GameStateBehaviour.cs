using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanosoft
{

/**
 * Базовый класс состояния игры (глобального), которое не затирается
 * при смене сцены.
 *
 * Чтобы реализовать обработку глобального состояния игры, пользователь
 * должен создать свой класс, наследующий от GameStateBehaviour, который
 * определит все нужные поля и обработчики событий.
 */
public class GameStateBehaviour: MonoBehaviour
{
	
	/**
	 * Ссылка на экземляр
	 */
	private static GameStateBehaviour instance;
	
	/**
	 * Пробуждение объекта
	 *
	 * Пользователь не должен переопределять этот метод или любые другие
	 * обработчики MonoBehaviour. Для инициализации состояния используйте
	 * переопределения метода OnInit().
	 */
	void Awake()
	{
		if ( !instance )
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			InitInstance();
		}
	}
	
	/**
	 * Инициализация состояния игры
	 */
	private void InitInstance()
	{
		SceneManager.sceneLoaded += handleSceneLoaded;
		OnInit();
	}
	
	/**
	 * Обработчик загрузки новой сцены
	 *
	 * Вызывается когда новая сцена загружена, здесь мы перемещаем заранее
	 * подготовленные объекты на новую сцену
	 */
	private void handleSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		OnSceneLoaded(scene, mode);
	}
	
	/**
	 * Событие инициализации
	 *
	 * Пользователь должен переопределить этот метод и реализовать в нём
	 * инициализацию глобального состояния.
	 */
	protected virtual void OnInit()
	{
	}
	
	/**
	 * События загрузки новой сцены
	 *
	 * Если пользователю нужно производить какие-то действия при смене сцены,
	 * например чтобы передвинуть персонажа в определенную позицию, то можно
	 * переопределить этот метод.
	 */
	protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
	}
	
} // class GameStateBehaviour

} // namespace Nanosoft
