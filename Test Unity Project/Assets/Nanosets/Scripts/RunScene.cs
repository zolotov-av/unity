using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanosoft
{

/**
 * Скрипт загрузки сцены
 */
public class RunScene: MonoBehaviour
{
	
	/**
	 * Нужно ли остановить таймер (Time.timeScale)
	 */
	public bool freezeTimer = false;
	
	/**
	 * Имя сцены которую надо загрузить
	 */
	public string sceneName = "";
	
	void Start()
	{
		if ( sceneName == null ) return;
		if ( sceneName == "" ) return;
		
		if ( freezeTimer ) Time.timeScale = 0f;
		
		SceneManager.LoadScene(sceneName);
	}
	
} // class RunScene

} // namespace Nanosoft
