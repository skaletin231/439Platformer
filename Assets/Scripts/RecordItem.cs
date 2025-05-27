using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordItem : MonoBehaviour
{

    public Image background;

    public TMP_Text name;

    public TMP_Text time;

    public TMP_Text scrore;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string name_value;
    public float time_value;
    public float score_value;
    
    public void init(string name, float time, float score)
    {
        name_value = name;
        time_value = time;
        score_value = score;

        if (name_value.Equals(SystemInfo.deviceUniqueIdentifier))
        {
            background.color = new Color(1.0f, 0.84f, 0.40f);
        }
        
        this.name.text = name.Substring(5, 5);
        this.time.text = "Time:" + time.ToString("0.00");
        this.scrore.text = "Score:" + score.ToString();
    }
}
