using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

namespace Clown
{
    public class Player : MonoBehaviour
    {
        public float baseSpeed;
        public float currentSpeed;
        private Rigidbody2D rb2D;
        [NonSerialized] public bool isGrabbing = false;
        [NonSerialized] public Child grabTarget;
        [NonSerialized] public DistanceJoint2D joint;
        private float slowSpeedMultiplier = .4f;

        void Awake()
        {
            currentSpeed = baseSpeed;
        }

        void Start()
        {
            rb2D = GetComponent<Rigidbody2D>();
            InvokeRepeating("LoseSpeed", 0f, .8f);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (isGrabbing)
                {
                    // drop dat kid
                    isGrabbing = false;
                    grabTarget.isGrabbed = false;
                    grabTarget.playerTransform = null;
                    currentSpeed = baseSpeed;
                    grabTarget.pathNodes = MapManager.s.FindPath(MapManager.s.tilemap.WorldToCell(grabTarget.transform.position), grabTarget.targetCell);
                    grabTarget.movePosition = grabTarget.pathNodes.Dequeue();
                    grabTarget.currentSpeed = grabTarget.baseSpeed * 2f;
                    grabTarget = null;
                    Destroy(joint);
                }
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (isGrabbing)
                {
                    // pull dat kid
                    if (currentSpeed < baseSpeed)
                    {
                        currentSpeed += 6;
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        float angle = Mathf.Deg2Rad * 45 * i;
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 8);
                        if (hit.collider != null)
                        {
                            Child child = hit.transform.GetComponent<Child>();
                            if (child != null)
                            {
                                // grab dat kid
                                isGrabbing = true;
                                grabTarget = child;
                                grabTarget.isGrabbed = true;
                                grabTarget.playerTransform = transform;
                                joint = gameObject.AddComponent<DistanceJoint2D>();
                                joint.connectedBody = grabTarget.rb2D;
                                currentSpeed = baseSpeed * slowSpeedMultiplier;
                                break;
                            }
                        }
                    }
                }
            }
        }

        void FixedUpdate()
        {
            float movementX = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
            float movementY = Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;
            rb2D.MovePosition(new Vector2(transform.position.x + movementX, transform.position.y + movementY));
        }

        void LoseSpeed()
        {
            if (isGrabbing && currentSpeed > baseSpeed * slowSpeedMultiplier)
            {
                currentSpeed -= 4;
            }
        }
    }
}