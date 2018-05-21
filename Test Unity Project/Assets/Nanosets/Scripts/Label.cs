using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

public class Label: MonoBehaviour
{
	
	protected Transform label = null;
	
	void Start()
	{
		Debug.Log("Label start");
		if ( label == null )
		{
			label = CanvasScript.CreateLabel();
			label.GetComponent<Text>().text = transform.root.gameObject.name;
			TabletController.UpdateLabel(transform.position, label);
		}
	}
	
	void LateUpdate()
	{
		if ( label != null )
		{
			TabletController.UpdateLabel(transform.position, label);
		}
	}
	
} // class Label

} // namespace Nanosoft
