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
        [NonSerialized] public float invulnTime = 1f;
        [NonSerialized] public bool isInvuln = false;
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
            if (GameManager.s.blockInput)
            {
                return;
            }
            if (GameManager.s.isGameOver)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    GameManager.s.Restart();
                }
            }
            if (GameManager.s.isGameStart)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    GameManager.s.isGameStart = false;
                    GameManager.s.startScreen.SetActive(false);
                    GameManager.s.SetupNextLevel();
                }
            }
            if (isGrabbing)
            {
                foreach (Vector3Int sewerCell in MapManager.s.sewerCells)
                {
                    if (Vector3.Distance(transform.position, MapManager.s.sewerCellData[sewerCell]) < 8)
                    {
                        // eat dat kid
                        isGrabbing = false;
                        grabTarget.isGrabbed = false;
                        currentSpeed = baseSpeed;
                        GameManager.s.kidsEaten += 1;
                        GameManager.s.eatenText.text = GameManager.s.kidsEaten + "/" + GameManager.s.kidsToEat + " Kids Delivered";
                        GameManager.s.mobsToClear.Add(grabTarget);
                        grabTarget = null;
                        Destroy(joint);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (isGrabbing)
                {
                    DropChild();
                }
            }
            if (Input.GetKeyDown(KeyCode.Z))
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
            if (GameManager.s.blockInput)
            {
                return;
            }
            float movementX = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
            float movementY = Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;
            rb2D.MovePosition(new Vector2(transform.position.x + movementX, transform.position.y + movementY));
        }

        public void DropChild()
        {
            // drop dat kid
            isGrabbing = false;
            grabTarget.isGrabbed = false;
            currentSpeed = baseSpeed;
            grabTarget.pathNodes = MapManager.s.FindPath(MapManager.s.tilemap.WorldToCell(grabTarget.transform.position), grabTarget.targetCell);
            while (grabTarget.pathNodes.Count == 0)
            {
                // Oh shit this is bad ABORT ABORT GET A NEW TARGET
                grabTarget.SetTargetAndPath();
            }
            grabTarget.movePosition = grabTarget.pathNodes.Dequeue();
            grabTarget.currentSpeed = grabTarget.baseSpeed * 2f;
            grabTarget = null;
            Destroy(joint);
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