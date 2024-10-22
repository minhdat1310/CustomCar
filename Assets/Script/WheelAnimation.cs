using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WheelControl;
public class WheelAnimation : MonoBehaviour
{
    #region PROPERITIES
    [SerializeField] float _rotationSpeed = 180.0f;

    Rigidbody _rigi;
    [SerializeField] WHEEL_TYPE _wheelType;
    CarControl _car;
    #endregion

    #region UNITY CORE
    void Start()
    {
        _rigi = GetComponentInParent<Rigidbody>();
        _car = _rigi.transform.GetComponent<CarControl>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateWheel(); // Điều khiển quay trái/phải
        RotateAroundAxis(); // Điều khiển quay tròn theo vận tốc
    }
    #endregion

    #region MAIN
    void RotateWheel()
    {
        if (_rigi != null)
        {
            // Lấy góc hiện tại của đối tượng dưới dạng Quaternion
            Quaternion currentRotation = transform.localRotation;

            // Tách riêng góc quay trên trục y từ Quaternion
            Vector3 eulerRotation = currentRotation.eulerAngles;
            float currentAngle = eulerRotation.y;

            float targetAngle = 0f;  // Góc mục tiêu để quay bánh xe

            // Kiểm tra phím nhấn để xác định góc mục tiêu
            if (Input.GetKey(KeyCode.A) || _car.IsRotateLeft)
            {
                if (_wheelType == WHEEL_TYPE.TopWheel)
                {
                    targetAngle = -MAX_TOP_ROTATE_ANGLE;
                }
                else
                {
                    targetAngle = -MAX_BOTTOM_ROTATE_ANGLE;
                }
            }
            else if (Input.GetKey(KeyCode.D) || _car.IsRotateRight)
            {
                if (_wheelType == WHEEL_TYPE.TopWheel)
                {
                    targetAngle = MAX_TOP_ROTATE_ANGLE;
                }
                else
                {
                    targetAngle = MAX_BOTTOM_ROTATE_ANGLE;
                }
            }

            // Tính góc mới bằng cách chuyển đổi từ góc hiện tại đến góc mục tiêu
            float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.fixedDeltaTime * _rotationSpeed);

            // Tạo một Quaternion mới chỉ với góc quay trên trục y
            Quaternion targetRotation = Quaternion.Euler(eulerRotation.x, newAngle, eulerRotation.z);

            // Gán lại rotation cho bánh xe (đối tượng hiện tại)
            transform.localRotation = targetRotation;
        }
    }

    void RotateAroundAxis()
    {
        if (_rigi != null)
        {
            var velocity = _rigi.transform.InverseTransformVector(_rigi.velocity).z;

            float angularVelocity = velocity / 0.5f;

            transform.Rotate(Vector3.right * angularVelocity * _rotationSpeed * Time.fixedDeltaTime);
        }
    }
    #endregion
}
