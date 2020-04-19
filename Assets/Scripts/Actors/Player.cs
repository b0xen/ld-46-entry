using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Clown
{
    public class Player : MonoBehaviour
    {
        public float speed;
        private Rigidbody2D rb2D;

        void Start()
        {
            rb2D = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            float movementX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float movementY = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            rb2D.MovePosition(new Vector2(transform.position.x + movementX, transform.position.y + movementY));
        }
    }
}