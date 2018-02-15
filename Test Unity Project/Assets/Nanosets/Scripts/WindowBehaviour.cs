using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class WindowBehaviour: MonoBehaviour
{
	
	/**
	 * Если true, то показать окно при загрузке
	 */
	public bool initShow = false;
	
	/**
	 * Если true, то спрятать окно при загрузке
	 */
	public bool initHide = false;
	
	public void ToggleShow()
	{
		gameObject.SetActive(!gameObject.activeSelf);
	}
	
	void Start()
	{
		if ( initShow )
		{
			gameObject.SetActive(true);
		}
		else if ( initHide )
		{
			gameObject.SetActive(false);
		}
	}
	
}

} // namespace Nanosoft
