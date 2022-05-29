using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    
    private PlayerInput _input;
    private Rigidbody2D _rb;
    private Animator _an;

    [SerializeField] private float baseSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _an = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(_input.currentActionMap["Move"].ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
        Vector2 moveDir = _input.currentActionMap["Move"].ReadValue<Vector2>();
        Debug.Log(moveDir);
        _rb.velocity = Vector2.ClampMagnitude(moveDir, 1) * baseSpeed;
        _an.SetFloat("facingX", moveDir.x);
        _an.SetFloat("facingY", moveDir.y);
    }
}
