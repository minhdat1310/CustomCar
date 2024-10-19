using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    #region PROPERTIES
    Rigidbody _rigi;

    [SerializeField] float _speed = 50.0f;
    [SerializeField] float _acceleration = 10.0f;

    Vector3 _rotationRight = new Vector3(0, -30, 0);
    Vector3 _rotationLeft = new Vector3(0, 30, 0);

    Vector3 _forward = new Vector3(1, 0, 0);
    Vector3 _backward = new Vector3(-1, 0, 0);

    #endregion

    #region UNITY CORE
    // Start is called before the first frame update
    void Awake()
    {
        _rigi = GetComponent<Rigidbody>();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move();
    }
    #endregion

    #region MAIN
    void Move()
    {
        if (_rigi != null)
        {
            if (Input.GetKey(KeyCode.W))
            {
                // Applying forward force
                _rigi.AddForce(_forward * _speed * _acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
            if (Input.GetKey(KeyCode.S))
            {
                // Applying backward force
                _rigi.AddForce(_backward * _speed * _acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
            }
        }
    }
    #endregion
}
