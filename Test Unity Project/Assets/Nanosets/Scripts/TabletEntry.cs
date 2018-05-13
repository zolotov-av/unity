using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Точка входа в сцену для TabletController
 */
public class TabletEntry: MonoBehaviour
{
	
	/**
	 * Префаб с контроллером который надо инстанцировать
	 */
	public GameObject prefab;
	
	/**
	 * Пустышка которую чье отображение надо отключить
	 */
	public GameObject dummy;
	
	public string backgroundMusic;
	
	void Awake()
	{
		if ( TabletController.instance == null )
		{
			var t = gameObject.transform;
			var obj = Instantiate(prefab, t.position, t.rotation);
			obj.name = prefab.name;
		}
		
		if ( dummy != null )
		{
			dummy.SetActive(false);
		}
	}
	
} // class TabletEntry

} // namespace Nanosoft
