﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Скрипт для управления канвой
 *
 * Управляет всеми окнами пользовательского интерфейса
 */
public class CanvasScript: MonoBehaviour
{
	
	/**
	 * Ссылка на менеджер квестов
	 */
	[HideInInspector]
	public QuestManager questManager;
	
	private GameObject actionPanel;
	private GameObject lootPanel;
	private Text message;
	private Text actionMessage;
	private Text lootMessage;
	
	private GameObject zoneInfo;
	private Text zoneMessage;
	private float zoneTimer = 0f;
	
	private GameObject raycastInfo;
	private Text raycastMessage;
	private TargetInfo raycastTarget;
	
	/**
	 * Ссылка на скрипт управляющий диалоговым окном
	 */
	public DialogWindow dialogWindow;
	
	/**
	 * Ссылка на скрипт управляющий окном списка квестов
	 */
	public QuestWindow questWindow;
	
	public GameObject labels;
	public Transform labelPrefab;
	public Transform damagePrefab;
	
	public Image healthBar;
	
	// TODO убрать костыль...
	private bool actionActive = false;
	
	public static CanvasScript instance;
	
	public void RunActionTouch()
	{
		TabletController.RunActionTouch();
	}
	
	void Awake()
	{
		if ( instance == null )
		{
			instance = this;
		}
	}
	
	void Start()
	{
		foreach(Transform t in transform)
		{
			WindowBehaviour w = t.GetComponent<WindowBehaviour>();
			if ( w != null )
			{
				if ( w.initShow ) w.Show();
				else if ( w.initHide ) w.Hide();
			}
		}
		
		GameObject actionGroup = transform.Find("ActionGroup").gameObject;
		
		actionPanel = actionGroup.transform.Find("ActionPanel").gameObject;
		actionPanel.SetActive(false);
		actionMessage = actionPanel.transform.Find("ActionMessage").gameObject.GetComponent<Text>();
		
		lootPanel = actionGroup.transform.Find("LootPanel").gameObject;
		lootPanel.SetActive(false);
		lootMessage = lootPanel.transform.Find("LootMessage").gameObject.GetComponent<Text>();
		lootMessage.text = "lootMessage";
		
		zoneInfo = transform.Find("ZoneInfo").gameObject;
		zoneInfo.SetActive(false);
		zoneMessage = zoneInfo.transform.Find("Text").GetComponent<Text>();
		zoneMessage.text = null;
		
		raycastInfo = transform.Find("RaycastInfo").gameObject;
		raycastInfo.SetActive(false);
		raycastMessage = raycastInfo.transform.Find("Text").GetComponent<Text>();
		raycastTarget = null;
		
		dialogWindow.Init();
	}
	
	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.L) )
		{
			questWindow.ToggleMain();
		}
		
		if ( zoneTimer != 0f )
		{
			if ( Time.unscaledTime > zoneTimer )
			{
				zoneTimer = 0f;
				zoneInfo.SetActive(false);
			}
		}
	}
	
	public static void ShowMessage(string msg)
	{
		instance.dialogWindow.ShowMessage(msg);
	}
	
	public static void SetAvatar(string avatarName, Sprite avatarPhoto)
	{
		instance.dialogWindow.SetAvatar(avatarName, avatarPhoto);
	}
	
	public static void ShowDialog(DialogItem dialog)
	{
		instance.dialogWindow.ShowDialog(dialog);
		instance.actionPanel.SetActive(false);
	}
	
	public static void CloseDialog()
	{
		instance.dialogWindow.CloseDialog();
		instance.actionPanel.SetActive(instance.actionActive);
	}
	
	public static void ShowAction(IAction action)
	{
		instance.actionActive = true;
		instance.actionMessage.text = action.GetActionMessage();
		instance.actionPanel.SetActive(true);
	}
	
	public static void HideAction()
	{
		instance.actionActive = false;
		instance.actionPanel.SetActive(false);
	}
	
	public static void Zone(string message, float duration = 1.8f)
	{
		instance.zoneInfo.SetActive(true);
		instance.zoneMessage.text = message;
		instance.zoneTimer = Time.unscaledTime + duration;
	}
	
	public static void ZoneLeave(string message)
	{
		if ( instance.zoneMessage.text == message )
		{
			instance.zoneInfo.SetActive(false);
			instance.zoneMessage.text = null;
		}
	}
	
	public static void RaycastInfo(GameObject obj)
	{
		if ( obj == null )
		{
			if ( instance.raycastTarget != null )
			{
				instance.raycastTarget = null;
				instance.raycastInfo.SetActive(false);
			}
			return;
		}
		
		TargetInfo info = obj.GetComponent<TargetInfo>();
		if ( info == null )
		{
			if ( instance.raycastTarget != null )
			{
				instance.raycastTarget = null;
				instance.raycastInfo.SetActive(false);
			}
			return;
		}
		
		if ( instance.raycastTarget == null )
		{
			instance.raycastInfo.SetActive(true);
		}
		
		if ( info != instance.raycastTarget )
		{
			instance.raycastTarget = info;
			instance.raycastMessage.text = info.objectName;
		}
	}
	
	public static Transform CreateLabel()
	{
		Transform label = Instantiate(instance.labelPrefab, instance.labels.transform);
		return label;
	}
	
	public static void DestroyLabel(Transform label)
	{
		Destroy(label);
	}
	
	/**
	 * Выделить метку дамага
	 */
	public static DamageLabel AllocDamageLabel()
	{
		// TODO сделать пул меток
		Transform t = Instantiate(instance.damagePrefab, instance.labels.transform);
		DamageLabel label = t.GetComponent<DamageLabel>();
		if ( label == null )
		{
			Debug.LogError("AllocDamageLabel(): prefab haven't DamageLabel script");
			Destroy(t);
			return null;
		}
		
		return label;
	}
	
	public static void FreeDamageLabel(DamageLabel label)
	{
		// TODO сделать пул меток
		if ( label != null )
		{
			Destroy(label.gameObject);
		}
	}
	
	public static void ThrowLabel(int damage, Vector3 position)
	{
		DamageLabel label = AllocDamageLabel();
		if ( label != null )
		{
			label.SetDamage(damage, position);
		}
	}
	
	/**
	 * Обновить позиции меток
	 */
	public static void UpdateLabels(Camera camera)
	{
		// TODO лучше сделать свой связный список для BasicLabel,
		// чтобы  не тратить время на GetComponent
		foreach(Transform t in instance.labels.transform)
		{
			BasicLabel label = t.GetComponent<BasicLabel>();
			if ( label != null )
			{
				t.position = camera.WorldToScreenPoint(label.worldPosition);
			}
		}
	}
	
	public static void UpdateHealth(PlayerBehaviour player)
	{
		instance.healthBar.fillAmount = ((float)player.GetCurrentHealth()) / ((float)player.maxHealth);
	}
	
} // class CanvasScript

} // namespace Nanosoft
