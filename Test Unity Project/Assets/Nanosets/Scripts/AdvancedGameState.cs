using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanosoft
{

/**
 * Класс представляющий глобальное состояние игры, которое не затирается при
 * переходадах между сценами и управляющий всеми подсистемами.
 */
public class AdvancedGameState: GameStateBehaviour
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
	 * Ссылка на префаб канвы пользовательского интерфейса
	 */
	public GameObject canvasPrefab;
	
	/**
	 * Ссылка на префаб менеджера квестов
	 */
	public GameObject questManagerPrefab;
	
	/**
	 * Ссылка на персонажа (загруженный экземляр)
	 */
	[HideInInspector]
	public GameObject player;
	
	/**
	 * Ссылка на контроллер персонажа
	 */
	[HideInInspector]
	public PlayerController playerCtl;
	
	/**
	 * Ссылка на камеру
	 *
	 * Камера конструируется динамически, без использования префаба
	 */
	[HideInInspector]
	public GameObject playerCamera;
	
	/**
	 * Ссылка на контроллер камеры
	 */
	[HideInInspector]
	public CameraScript cameraCtl;
	
	/**
	 * Канва пользовательского интерфейса (загруженный экземляр)
	 */
	[HideInInspector]
	public GameObject canvas;
	
	/**
	 * Ссылка на контроллер канвы
	 */
	[HideInInspector]
	public CanvasScript canvasCtl;
	
	/**
	 * Ссылка на объект с менеджером квестов
	 */
	[HideInInspector]
	public GameObject questManagerObject;
	
	/**
	 * Ссылка на менеджер квестов
	 */
	[HideInInspector]
	public QuestManager questManager;
	
	/**
	 * Событие инициализации
	 *
	 * Пользователь должен переопределить этот метод и реализовать в нём
	 * инициализацию глобального состояния.
	 */
	protected override void OnInit()
	{
		// создаем персонажа
		var t = gameObject.transform;
		player = Instantiate(playerPrefab, t.position, t.rotation);
		player.name = playerPrefab.name;
		
		// создаем камеру
		playerCamera = Instantiate(cameraPrefab);
		playerCamera.name = cameraPrefab.name;
		
		// создаем канву
		canvas = Instantiate(canvasPrefab);
		canvas.name = canvasPrefab.name;
		
		// создаем менеджер квестов
		questManagerObject = Instantiate(questManagerPrefab, t);
		questManagerObject.name = questManagerPrefab.name;
		
		// персонаж и камера не должны удаляться при переключении сцены
		DontDestroyOnLoad(player);
		DontDestroyOnLoad(playerCamera);
		DontDestroyOnLoad(canvas);
		
		playerCtl = player.GetComponent<PlayerController>();
		playerCtl.playerCamera = playerCamera;
		
		cameraCtl = playerCamera.GetComponent<CameraScript>();
		cameraCtl.target = player;
		
		questManager = questManagerObject.GetComponent<QuestManager>();
		
		canvasCtl = canvas.GetComponent<CanvasScript>();
		canvasCtl.questManager = questManager;
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
		Debug.Log("AdvancedGameState.OnSceneLoaded(" + scene.name + ")");
		
		playerCtl.TeleportPlayer();
	}
	
} // class AdvancedGameState

} // namespace Nanosoft
