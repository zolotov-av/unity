using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

namespace Nanosoft
{

/**
 * Класс описывающий пакет диалогов
 */
[XmlRoot("dialog-bundle")]
public class DialogBundle
{
	
	[XmlElement("bundle-name")]
	public string name;
	
	/**
	 * Список элементов диалога
	 */
	[XmlArray("items")]
    [XmlArrayItem("dialog-item")]
	public DialogItem[] items;
	
	private Dictionary<string, DialogItem> map;
	
	public static DialogBundle Load(TextAsset asset)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(DialogBundle));
		StringReader reader = new StringReader(asset.text);
		DialogBundle dial = serializer.Deserialize(reader) as DialogBundle;
		if ( dial != null ) dial.prepare();
		return dial;
	}
	
	protected void prepare(DialogAction action)
	{
		/*
		if ( action.nextId == null )
		{
			//Debug.Log("goto=null");
		}
		else
		{
			Debug.Log("goto=[" + action.nextId + "]");
		}
		*/
		
		if ( action.nextDialog != null )
		{
			if ( action.nextId != null )
			{
				Debug.LogError("either goto or next-dialog should be used, goto is ignored");
			}
			prepare(action.nextDialog);
		}
		else
		{
			if ( action.nextId == null || action.nextId == "" )
			{
				action.nextDialog = null;
			}
			else
			{
				Debug.Log("goto=[" + action.nextId + "]");
				
				DialogItem item;
				if ( map.TryGetValue(action.nextId, out item) )
				{
					action.nextDialog = item;
				}
				else
				{
					action.nextDialog = null;
					Debug.LogError("referred dialog[" + action.nextId + "] not found");
				}
			}
		}
	}
	
	protected void prepare(DialogItem item)
	{
		if ( item.id == null )
		{
			//Debug.Log("id=null");
		}
		else
		{
			//Debug.Log("id=[" + item.id + "]");
			if ( item.id != "" )
			{
				Debug.Log("add item[" + item.id + "]");
				map.Add(item.id, item);
			}
		}
		
		foreach(DialogAction action in item.replies)
		{
			prepare(action);
		}
	}
	
	protected void prepare()
	{
		map = new Dictionary<string, DialogItem>();
		
		foreach(DialogItem item in items)
		{
			prepare(item);
		}
	}
	
} // class DialogBundle

} // namespace Nanosoft
