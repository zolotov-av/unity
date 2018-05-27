using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Метка для отображения урона
 */
public class DamageLabel: BasicLabel
{
	
	/**
	 * Время когда метка устаревает
	 */
	private float expireTime = 0f;
	
	/**
	 * Скорость с которой отлетает метка
	 */
	private Vector3 speed;
	
	/**
	 * Ссылка на текстовый элемент
	 */
	public Text damageText;
	
	public void SetDamage(int damage, Vector3 position)
	{
		damageText.text = damage.ToString();
		worldPosition = position;
	}
	
	void Start ()
	{
		expireTime = Time.time + 1f;
		speed = new Vector3(0f, 0.8f, 0f);
		gameObject.SetActive(true);
	}
	
	void Update ()
	{
		if ( Time.time > expireTime )
		{
			gameObject.SetActive(false);
			CanvasScript.FreeDamageLabel(this);
			return;
		}
		
		worldPosition += speed * Time.deltaTime;
		//TabletController.UpdateLabel(worldPosition, transform);
	}
	
}

} // namespace Nanosoft
