using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static WheelControl;
using TMPro;
public class UIControl : Singleton<UIControl>
{
    #region PROPERITIES
    [SerializeField] Rigidbody _rigi;

    [SerializeField] TextMeshProUGUI _speedText, _timeText, _fpsText, _endTimeText, _bestTimeText;
    public TextMeshProUGUI TimeText { get { return _timeText; } set { _timeText = value; }  }
    public TextMeshProUGUI EndTimeText { get { return _endTimeText; } set { _endTimeText = value; } }
    public TextMeshProUGUI BestTimeText { get { return _bestTimeText; } set { _bestTimeText = value; } }
    [SerializeField, Range(180, 300)] float MAX_MAGNITUDE_SPEED = 180.0f;
    float _speed = 0.0f;
    #endregion

    #region UNITY CORE
    // Start is called before the first frame update
    void Start()
    {
        //if (PlayerPrefs.HasKey("BestTime"))
        //{
        //    PlayerPrefs.SetInt("BestTime", int.MaxValue);
        //    PlayerPrefs.Save();
        //}
        _speed = _rigi.velocity.magnitude / MAX_SPEED * MAX_MAGNITUDE_SPEED;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int speed = Mathf.FloorToInt(_rigi.velocity.magnitude / MAX_SPEED * MAX_MAGNITUDE_SPEED);
        _speedText.text = "Speed: " + speed.ToString() + " km/h";

    }

    private void Update()
    {
        float fps = 1.0f/Time.deltaTime;
        _fpsText.text = "FPS: " + fps.ToString();

    }
    #endregion

    #region MAIN

    #endregion
}
