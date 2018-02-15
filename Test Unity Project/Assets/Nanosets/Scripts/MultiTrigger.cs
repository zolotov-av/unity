using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class MultiTrigger: MonoBehaviour
{
	public ZoneTrigger mainTrigger;
	
	void OnTriggerEnter(Collider other)
	{
		if ( mainTrigger != null )
		{
			mainTrigger.OnTriggerEnter(other);
		}
	}
	
}

} // namespace Nanosoft
