using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Скрипт для управления камеры
 *
 * Заставляет камеру следить за указанным объектом
 */
public class CameraScript: MonoBehaviour {

    public GameObject target;
    public Text camdbg;
    
    [Header("Camera Settings")]
    public float height = 1.4f;
    public float startDistance = 3f;
	public float startAngle = 20f;

    private float camAngle = 0f;
    private float distance = 0f;
    
    private const float angleSensitivity = 3f;
    private const float minAngle = -40f;
    private const float maxAngle = 80f;

    #region Utils

    protected string coord(Vector3 v)
    {
        return ((int)v.x).ToString() + ", " + ((int)v.y).ToString() + ", " + ((int)v.z).ToString();
    }

    protected void DbgUpdate()
    {
        if (camdbg)
        {
            camdbg.text = "Nanosoft.Camera v22\n" +
				"CamPos: " + coord(transform.position) + "\n" +
                "TargetPos: " + coord(target.transform.position) + "\n" +
                "distance: " + string.Format("{0:0.00}", distance) + "\n" +
                "angle: " + string.Format("{0:0.00}", camAngle);
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        while (angle > 360) angle -= 360;
        while (angle < -360) angle += 360;
        return Mathf.Clamp(angle, min, max);
    }

    #endregion

    void Start()
	{
		distance = startDistance;
		camAngle = startAngle;
		if ( !camdbg )
		{
			var obj = GameObject.FindWithTag("CameraDebug");
			if ( obj ) camdbg = obj.GetComponent<Text>();
		}
    }
	
	public void UpdateOptions(float distanceDelta, float angleDelta)
	{
		distance -= distanceDelta;
        distance = Mathf.Clamp(distance, 1f, 5f);
		
        camAngle -= angleDelta * angleSensitivity;
		camAngle = ClampAngle(camAngle, minAngle, maxAngle);
	}

    // Update is called once per frame
    protected virtual void LateUpdate()
    {
        Vector3 tp = target.transform.position + target.transform.up * height;

        float rad = camAngle * Mathf.PI / 180;
        Vector3 cv = target.transform.up * (Mathf.Sin(rad) * distance);
        Vector3 sv = target.transform.forward * (-Mathf.Cos(rad) * distance);
        transform.position = tp + cv + sv;

        transform.LookAt(tp);

        DbgUpdate();
    }
	
} // class CameraScript

} // namespace Nanosoft
