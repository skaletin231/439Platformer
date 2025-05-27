using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class parallaxLayersController : MonoBehaviour
{

    public Camera mainCam;

    public GameObject[] bgTempletes;

    public float[] bgSpeed;

    // Start is called before the first frame update
    void Start()
    {
        if (!mainCam)
        {
            Debug.LogWarning("No camera found in parallax controller's outlet, using main camera");
            mainCam = Camera.main;
        }

        oldCameraPos = new Vector2(mainCam.transform.position.x, mainCam.transform.position.y);

        if (bgSpeed.Length < bgTempletes.Length)
        {
            for (int i = bgSpeed.Length; i < bgTempletes.Length; i++)
            {
                bgSpeed[i] = 1.0f;
            }
        }

        float cameraWidthInUnit = mainCam.orthographicSize * 2 * Screen.width/Screen.height;
        float cameraHeightInUnit = mainCam.orthographicSize * 2;
        for (int i = 0; i < bgTempletes.Length; i++)
        {
            GameObject bgTemplete = bgTempletes[i];
            bgTemplete.transform.position = new Vector3(
                mainCam.transform.position.x,
                bgTemplete.transform.position.y,
                0.0f
            );
        }
    }

    Vector2 oldCameraPos = Vector2.zero;
    Vector2 deltaCameraPos = Vector2.zero;
    public float bgUnit = 5.0f;
    
    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        Vector2 curCameraPos = mainCam.transform.position;
        deltaCameraPos = curCameraPos - oldCameraPos;
        float deltaX = deltaCameraPos.x;
        float cameraLeftStart = curCameraPos.x - mainCam.orthographicSize * mainCam.aspect;
        
        for (int j = 0; j < bgTempletes.Length; j++)
        {
            GameObject bg = bgTempletes[j];
            SpriteRenderer bgSprite = bg.GetComponent<SpriteRenderer>();
            bg.transform.position = new Vector3(bg.transform.position.x + bgSpeed[j] * deltaX, bg.transform.position.y, bg.transform.position.z);
            float bgX = bg.transform.position.x;
            float camX = mainCam.transform.position.x;
            if (Mathf.Abs(camX - bgX) >= 5 * bgUnit)
            {
                float offset = (camX - bgX) % bgUnit;
                bg.transform.position = new Vector3(camX + offset, bg.transform.position.y, bg.transform.position.z);
            }
        }
        oldCameraPos = curCameraPos;
    }
}
