using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class MainMenuScript: WindowBehaviour
{
	
	public bool inMenu = true;
	public GameObject mainPanel;
	public GameObject introPanel;
	private GameObject currentPanel = null;
	
	public void showMainMenu()
	{
		Debug.Log("showMainMenu");
		inMenu = true;
		gameObject.SetActive(true);
		if ( currentPanel != null )
		{
			currentPanel.SetActive(false);
		}
		currentPanel = mainPanel;
		mainPanel.SetActive(true);
	}
	
	public void showIntro()
	{
		Debug.Log("showIntro");
		inMenu = true;
		gameObject.SetActive(true);
		if ( currentPanel != null )
		{
			currentPanel.SetActive(false);
		}
		currentPanel = introPanel;
		introPanel.SetActive(true);
	}
	
	public void HandleInput()
	{
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			if ( currentPanel != mainPanel )
			{
				showMainMenu();
				return;
			}
		}
	}
	
	public void OnNewGameButton()
	{
		SoundManager.instance.FadePlay("town");
		gameObject.SetActive(false);
		inMenu = false;
	}
	
	public void OnLoadGameButton()
	{
	}
	
	public void OnOptionsButton()
	{
	}
	
	public void OnIntroButton()
	{
		showIntro();
	}
	
	public void OnStaffButton()
	{
	}
	
	public void OnExitButton()
	{
		Application.Quit();
	}
	
}

} // namespace Nanosoft
