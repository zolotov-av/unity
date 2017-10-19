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
	
	public static DialogBundle Load(TextAsset asset)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(DialogBundle));
		StringReader reader = new StringReader(asset.text);
		DialogBundle dial = serializer.Deserialize(reader) as DialogBundle;
		return dial;
	}
	
} // class DialogBundle

} // namespace Nanosoft
