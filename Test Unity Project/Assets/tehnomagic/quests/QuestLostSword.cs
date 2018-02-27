using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nanosoft
{

/**
 * Квест "Меч героя"
 */
public class QuestLostSword: Quest
{
	
	private string found = "start";
	
	public string GetFoundVar()
	{
		return found;
	}
	
	public void SetFoundVar(string value)
	{
		found = value;
	}
	
	public string GetSceneVar()
	{
		return TabletController.sceneName;
	}
	
	public void SetLoadMainlandTrigger()
	{
		TabletController.LoadScene("mainland");
	}
	
	public void SetLoadStartlandTrigger()
	{
		TabletController.LoadScene("startland");
	}
	
	public void SetAcceptTrigger()
	{
		found = "no";
		active = true;
		logEvent("Гоблины-строители утащили куда-то \"Меч героя\", Квери-чан просит нас его найти");
		QuestManager.Refresh();
		LostSwordTrigger.Enable();
	}
	
	public void SetEndTrigger()
	{
		found = "done";
		CanvasScript.Zone("Квест завершен");
		active = false;
		QuestManager.WriteSound();
		QuestManager.Refresh();
		LostSwordTrigger.Finish();
	}
	
	protected void Awake()
	{
		base.Awake();
		InitTriggers(this.GetType());
	}
	
}

} // namespace Nanosoft
