using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanosoft
{

/**
 * Простой пример класса хранящего глобальное состояние игры, которое не
 * затирается при переходах между сценами.
 *
 * Данный класс хранит в себе ссылку на персонажа управляемого с помощью
 * простого контроллера (DumbController) и камеру управляемую с помощью
 * скрипта CameraScript (DumbController требует чтобы камера управлялась
 * скриптом CameraScript).
 */
public class SimpleGameState: GameStateBehaviour
{
	
	/**
	 * Ссылка на префаб для загрузки персонажа
	 */
	public GameObject playerPrefab;
	
	/**
	 * Ссылка на префаб камеры
	 */
	public GameObject cameraPrefab;
	
	/**
	 * Ссылка на персонажа (загруженный экземляр)
	 */
	public GameObject player;
	
	/**
	 * Ссылка на контроллер персонажа
	 */
	public DumbController playerCtl;
	
	/**
	 * Ссылка на камеру
	 *
	 * Камера конструируется динамически, без использования префаба
	 */
	public GameObject playerCamera;
	
	/**
	 * Ссылка на контроллер камеры
	 */
	public CameraScript cameraCtl;
	
	/**
	 * Событие инициализации
	 *
	 * Пользователь должен переопределить этот метод и реализовать в нём
	 * инициализацию глобального состояния.
	 */
	protected override void OnInit()
	{
		Debug.Log("SimpleGameState.OnInit()");
		
		// создаем персонажа
		player = Instantiate(playerPrefab);
		player.name = playerPrefab.name;
		
		// создаем камеру
		playerCamera = Instantiate(cameraPrefab);
		playerCamera.name = cameraPrefab.name;
		
		// персонаж и камера не должны удаляться при переключении сцены
		DontDestroyOnLoad(player);
		DontDestroyOnLoad(playerCamera);
		
		playerCtl = player.GetComponent<DumbController>();
		playerCtl.playerCamera = playerCamera;
		
		cameraCtl = playerCamera.GetComponent<CameraScript>();
		cameraCtl.target = player;
		
		Debug.Log("SimpleGameState.OnInit() leave");
	}
	
	/**
	 * События загрузки новой сцены
	 *
	 * Если пользователю нужно производить какие-то действия при смене сцены,
	 * например чтобы передвинуть персонажа в определенную позицию, то можно
	 * переопределить этот метод.
	 */
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("SimpleGameState.OnSceneLoaded(" + scene.name + ")");
		
		playerCtl.TeleportPlayer();
	}
	
} // class SimpleGameState

} // namespace Nanosoft
