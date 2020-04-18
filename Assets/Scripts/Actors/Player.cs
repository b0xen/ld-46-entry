using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Clown
{
    public class Player : MonoBehaviour
    {
        public float speed;
        private Rigidbody2D rigidBody;

        void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            float movementX = Input.GetAxis("Horizontal") * speed;
            float movementY = Input.GetAxis("Vertical") * speed;

            rigidBody.MovePosition(new Vector2(transform.position.x + movementX, transform.position.y + movementY));
        }
    }
}