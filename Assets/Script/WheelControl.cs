using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    #region PROPERITIES
    [SerializeField] float _springStrength = 100.0f;
    [SerializeField] float _springDamper = 10.0f;
    [SerializeField] float _accelInput = 0.0f;

    static float suspensionRestDist = 0.55f;
    bool _rayDidHit = false;
    float hitDistance = 0.0f;

    Rigidbody _rigi;

    static float ACCEL_INPUT_VALUE = 2.0f;
    public static float MAX_SPEED = 20.0f;
    public static float MAX_TOP_ROTATE_ANGLE = 32.0f;
    public static float MAX_BOTTOM_ROTATE_ANGLE = MAX_TOP_ROTATE_ANGLE / 4f;


    public enum WHEEL_TYPE { TopWheel, BotWheel};
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
        CheckInputMove();
        CheckInputRotate();
        CheckHitGround();
        AddSuspensionForce();

    }
    #endregion

    #region MAIN
    void CheckInputMove()
    {
        if (_rigi != null)
        {
            if (_car.IsAccel || Input.GetKey(KeyCode.W))
            {
                _accelInput = ACCEL_INPUT_VALUE;
            } else if (_car.IsBrake || Input.GetKey(KeyCode.S))
            {
                _accelInput = -ACCEL_INPUT_VALUE * 2.0f;
            } else
            {
                _accelInput = 0.0f;
            }
            AddAccelerationForce();
        }
    }

    void CheckInputRotate()
    {
        if (_rigi != null)
        {
            Vector3 currentRotation = transform.localEulerAngles;
            float currentAngle = currentRotation.y;
            if (currentAngle > 180) currentAngle -= 360;

            if (_car.IsRotateLeft || Input.GetKey(KeyCode.A))
            {
                if (_wheelType == WHEEL_TYPE.TopWheel)
                {
                    if (currentAngle > -MAX_TOP_ROTATE_ANGLE)
                    {
                        float targetAngle = -MAX_TOP_ROTATE_ANGLE;
                        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, 1f * Time.fixedDeltaTime);
                        transform.localRotation = Quaternion.Euler(new Vector3(currentRotation.x, newAngle, currentRotation.z));
                    }
                }
                else
                {
                    if (currentAngle > -MAX_BOTTOM_ROTATE_ANGLE)
                    {
                        float targetAngle = -MAX_BOTTOM_ROTATE_ANGLE;
                        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, 0.25f * Time.fixedDeltaTime);
                        transform.localRotation = Quaternion.Euler(new Vector3(currentRotation.x, newAngle, currentRotation.z));
                    }
                }
            }
            else if (_car.IsRotateRight || Input.GetKey(KeyCode.D))
            {
                if (_wheelType == WHEEL_TYPE.TopWheel)
                {
                    if (currentAngle < MAX_TOP_ROTATE_ANGLE)
                    {
                        float targetAngle = MAX_TOP_ROTATE_ANGLE;
                        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, 1f * Time.fixedDeltaTime);
                        transform.localRotation = Quaternion.Euler(new Vector3(currentRotation.x, newAngle, currentRotation.z));
                    }
                }
                else
                {
                    if (currentAngle < MAX_BOTTOM_ROTATE_ANGLE)
                    {
                        float targetAngle = MAX_BOTTOM_ROTATE_ANGLE;
                        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, 0.25f * Time.fixedDeltaTime);
                        transform.localRotation = Quaternion.Euler(new Vector3(currentRotation.x, newAngle, currentRotation.z));
                    }
                }
            }
            else
            {
                // Khi không nhấn A hoặc D, bánh xe dần quay về vị trí thẳng (góc 0)
                float targetAngle = 0f;

                if (_wheelType == WHEEL_TYPE.TopWheel)
                {
                    // Đối với bánh trên, trả về nhanh hơn
                    float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, 4f * Time.fixedDeltaTime);
                    transform.localRotation = Quaternion.Euler(new Vector3(currentRotation.x, newAngle, currentRotation.z));
                }
                else
                {
                    // Đối với bánh dưới, trả về chậm hơn
                    float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, 1f * Time.fixedDeltaTime);
                    transform.localRotation = Quaternion.Euler(new Vector3(currentRotation.x, newAngle, currentRotation.z));
                }
            }
        }

        AddSteeringForce();
    }


    void CheckHitGround()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = -_rigi.transform.up;
        float rayLength = suspensionRestDist;

        Debug.DrawRay(rayOrigin, rayDirection);
        _rayDidHit = Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength);
        if (_rayDidHit)
        {
            hitDistance = hit.distance;

        }
    }

    void AddSuspensionForce()
    {
        if (_rayDidHit)
        {
            Vector3 springDir = _rigi.transform.up;

            Vector3 wheelWorldVel = _rigi.GetPointVelocity(transform.position);

            float offset = suspensionRestDist - hitDistance;
            float vel = Vector3.Dot(springDir, wheelWorldVel);

            float force = (offset * _springStrength) - (vel * _springDamper);
            _rigi.AddForceAtPosition(springDir * force, transform.position);
        }

    }

    void AddAccelerationForce()
    {
        var localVelocity = _rigi.transform.InverseTransformVector(_rigi.velocity);
        if (_rayDidHit)
        {
            Vector3 accelDir = transform.forward;
            if (_accelInput > 0.0f || (_accelInput < 0.0f && localVelocity.z > -3.0f))
            {
                float carSpeed = Vector3.Dot(_rigi.transform.forward, _rigi.velocity);
                float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / MAX_SPEED);
                float avaiableTorque = PowerCurve(normalizedSpeed) * _accelInput;
                _rigi.AddForceAtPosition(accelDir * avaiableTorque, transform.position);
                Debug.DrawRay(transform.position, accelDir * avaiableTorque);

            }
            if (localVelocity.z < -3.0f)
            {
                Vector3 currVel = localVelocity;
                currVel.z = -3.0f;
                localVelocity = currVel;
            }
        }
        
    }

    void AddSteeringForce()
    {
        if (_rayDidHit)
        {
            Vector3 steeringDir = transform.right;
            Vector3 wheelWorldVel = _rigi.GetPointVelocity(transform.position);
            float steeringVel = Vector3.Dot(steeringDir, wheelWorldVel);
            float desiredVelChange = -steeringVel * 0.3f;
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
            _rigi.AddForceAtPosition(steeringDir * 0.1f * desiredAccel, transform.position);
            Debug.DrawRay(transform.position, steeringDir * 0.1f * desiredAccel);
        }
    }

    float PowerCurve(float normalizedSpeed)
    {
        if (normalizedSpeed < 0.4f)
        {
            return Mathf.Lerp(0.5f, 1.0f, normalizedSpeed / 0.4f);
        }
        else if (normalizedSpeed < 0.7f)
        {
            return 1.0f;
        }
        else if (normalizedSpeed < 1.0f)
        {
            return Mathf.Lerp(1.0f, 0.4f, (normalizedSpeed - 0.7f) / (1.0f - 0.7f));
        } else
        {
            return 0.0f;
        }
    }
    #endregion
}
