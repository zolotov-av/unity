﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class TabletSettings: MonoBehaviour
{
	
	public void ToggleJumps()
	{
		TabletController.bigJump = !TabletController.bigJump;
	}
	
	public void ExitButton()
	{
		Application.Quit();
	}
}

} // namespace Nanosoft
