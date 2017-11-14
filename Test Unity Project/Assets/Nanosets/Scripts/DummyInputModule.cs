using UnityEngine;
using UnityEngine.EventSystems;

namespace Nanosoft
{

/**
 * Фиктивный модуль ввода для EventSystem
 */
[AddComponentMenu("Event/Dummy Input Module")]
public class DummyInputModule: BaseInputModule
{
	
	/**
	 * Обработка ввода (ничего не делать)
	 */
	public override void Process()
	{
	}
	
} // class DummyInputModule

} // namespace Nanosoft
