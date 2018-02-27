using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanosoft
{

public class MainMenuScript: WindowBehaviour
{
	
	public GameObject mainPanel;
	public GameObject introPanel;
	public GameObject loadingPanel;
	public GameObject backgroundPanel;
	private GameObject currentPanel = null;
	
	/**
	 * Отобразить панель
	 */
	protected void ShowPanel(GameObject panel)
	{
		if ( currentPanel != null && currentPanel != panel )
		{
			currentPanel.SetActive(false);
		}
		
		currentPanel = panel;
		
		if ( !panel.activeSelf )
		{
			panel.SetActive(true);
		}
	}
	
	/**
	 * Отобразить главное меню
	 */
	public void ShowMain()
	{
		ShowPanel(mainPanel);
		ShowModal();
	}
	
	/**
	 * Отобразить вступление
	 */
	public void ShowIntro()
	{
		ShowPanel(introPanel);
		ShowModal();
	}
	
	/**
	 * Отобразить заставку загрузки
	 */
	public void ShowLoading()
	{
		ShowPanel(loadingPanel);
		backgroundPanel.SetActive(true);
		ShowModal();
	}
	
	/**
	 * Завершить загрузку сцены
	 */
	public void EndLoading()
	{
		backgroundPanel.SetActive(false);
		Hide();
	}
	
	/**
	 * Обработка ввода
	 */
	public override void HandleInput()
	{
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			if ( currentPanel != mainPanel )
			{
				ShowPanel(mainPanel);
				return;
			}
		}
	}
	
	public void OnNewGameButton()
	{
		//ShowLoading();
		//SceneManager.LoadScene(1);
		TabletController.LoadScene(1);
		/*
		SoundManager.instance.FadePlay("town");
		gameObject.SetActive(false);
		inMenu = false;
		Time.timeScale = 1f;
		*/
	}
	
	public void OnLoadGameButton()
	{
	}
	
	public void OnOptionsButton()
	{
	}
	
	public void OnIntroButton()
	{
		ShowIntro();
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
