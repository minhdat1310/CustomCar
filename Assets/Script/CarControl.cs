using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Cần thiết để sử dụng EventTrigger

public class CarControl : MonoBehaviour
{
    // Mảng chứa các WheelControl
    WheelControl[] wheelsList;
    [SerializeField] Button _accelButton, _brakeButton, _leftButton, _rightButton;
    [SerializeField] bool _isAccel = false, _isBrake = false, _isRotateLeft = false, _isRotateRight = false;
    [SerializeField] GameObject _endGameCanvas, _inGameCanvas; // Kéo Canvas Win vào từ Unity Inspector
    public bool IsAccel { get { return _isAccel; } set { _isAccel = value; } }
    public bool IsBrake { get { return _isBrake; } set { _isBrake = value; } }
    public bool IsRotateLeft { get { return _isRotateLeft; } set { _isRotateLeft = value; } }
    public bool IsRotateRight { get { return _isRotateRight; } set { _isRotateRight = value; } }
    float currTime;
    void Awake()
    {
        // Lấy tất cả các children có WheelControl component
        wheelsList = GetComponentsInChildren<WheelControl>();

        // Gán sự kiện click cho các nút sử dụng EventTrigger
        AddEventTrigger(_accelButton, OnAccelDown, OnAccelUp);
        AddEventTrigger(_brakeButton, OnBrakeDown, OnBrakeUp);
        AddEventTrigger(_leftButton, OnLeftDown, OnLeftUp);
        AddEventTrigger(_rightButton, OnRightDown, OnRightUp);
    }

    private void Start()
    {
        currTime = 0;
    }

    private void FixedUpdate()
    {
        currTime += Time.fixedDeltaTime;
        UIControl.Instance.TimeText.text = "Time: " + Mathf.FloorToInt(currTime) + " s";
    }

    private void Update()
    {
        CheckHitFinishLine();
        
    }

    // Hàm tiện ích để thêm sự kiện OnPointerDown và OnPointerUp
    void AddEventTrigger(Button button, UnityEngine.Events.UnityAction onDown, UnityEngine.Events.UnityAction onUp)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Tạo và thêm sự kiện cho OnPointerDown
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { onDown.Invoke(); });
        trigger.triggers.Add(pointerDownEntry);

        // Tạo và thêm sự kiện cho OnPointerUp
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { onUp.Invoke(); });
        trigger.triggers.Add(pointerUpEntry);
    }

    // Hàm xử lý khi nhấn và nhả nút Accel
    void OnAccelDown()
    {
        for (int i = 0; i < wheelsList.Length; i++)
        {
            _isAccel = true;
        }
    }

    void OnAccelUp()
    {
        for (int i = 0; i < wheelsList.Length; i++)
        {
            _isAccel = false;
        }
    }

    // Hàm xử lý khi nhấn và nhả nút Brake
    void OnBrakeDown()
    {
        for (int i = 0; i < wheelsList.Length; i++)
        {
            _isBrake = true;
        }
    }

    void OnBrakeUp()
    {
        for (int i = 0; i < wheelsList.Length; i++)
        {
            _isBrake = false;
        }
    }

    // Hàm xử lý khi nhấn và nhả nút Left
    void OnLeftDown()
    {
        for (int i = 0; i < wheelsList.Length; i++)
        {
            _isRotateLeft = true;
        }
    }

    void OnLeftUp()
    {
        for (int i = 0; i < wheelsList.Length; i++)
        {
            _isRotateLeft = false;
        }
    }

    // Hàm xử lý khi nhấn và nhả nút Right
    void OnRightDown()
    {
        for (int i = 0; i < wheelsList.Length; i++)
        {
            _isRotateRight = true;
        }
    }

    void OnRightUp()
    {
        for (int i = 0; i < wheelsList.Length; i++)
        {
            _isRotateRight = false;
        }
    }

    void CheckHitFinishLine()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = -transform.up;
        float rayLength = 2.0f;
        int layerMask = 1 << 6;
        Debug.DrawRay(rayOrigin, rayDirection);
        bool _rayDidHit = Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, layerMask);

        if (_rayDidHit)
        {
            int bestTime = PlayerPrefs.GetInt("BestTime");
            Debug.Log(bestTime);
            int intCurrTime = Mathf.FloorToInt(currTime);
            UIControl.Instance.EndTimeText.text = "Time: " + intCurrTime.ToString() + " s";
            if (intCurrTime < bestTime)
            {
                bestTime = intCurrTime;
                PlayerPrefs.SetInt("BestTime", bestTime);
                PlayerPrefs.Save();
            }
            UIControl.Instance.BestTimeText.text = "Best Time: " + bestTime + " s";
           
            _endGameCanvas.SetActive(true);
            _inGameCanvas.SetActive(false);
            Time.timeScale = 0f; // Dừng thời gian, dừng toàn bộ game


        } else
        {
            Time.timeScale = 1f;
        }
    }
    
}
