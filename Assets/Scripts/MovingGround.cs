using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    public float speed;
    private Vector3 _startPosition;
    public Vector3 targetPosition;
    private bool _isMoving;
    private bool _isForward;
    public float staySeconds;
    // Start is called before the first frame update
    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        _startPosition = transform.position;
        _isForward = true;
        _isMoving = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isMoving) return;
        
        if (_isForward)
        {
            transform.position += Vector3.forward * (speed * Time.deltaTime);
            if (transform.position.z >= targetPosition.z)
            {
                StartCoroutine(Move());
            }
        }
        else if (!_isForward)
        {
            transform.position -= Vector3.forward * (speed * Time.deltaTime);
            if (transform.position.z <= _startPosition.z)
            {
                StartCoroutine(Move());
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

    private IEnumerator Move()
    {
        _isMoving = false;
        yield return new WaitForSeconds(staySeconds);
        _isForward = !_isForward;
        _isMoving = true;
        

    }
}
