using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class WindowBehaviour: MonoBehaviour
{
	
	public static WindowBehaviour current = null;
	
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
	
	public void ToggleMain()
	{
		if ( gameObject.activeSelf )
		{
			gameObject.SetActive(false);
			current = null;
		}
		else
		{
			if ( current != null ) current.gameObject.SetActive(false);
			current = this;
			gameObject.SetActive(true);
		}
	}
	
	public void Show()
	{
		gameObject.SetActive(true);
	}
	
	public void Hide()
	{
		gameObject.SetActive(false);
		if ( current == this ) current = null;
	}
	
}

} // namespace Nanosoft
