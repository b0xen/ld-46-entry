using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Clown
{
    public class GameManager : MonoBehaviour
    {
        [NonSerialized] public static GameManager s;
        [NonSerialized] public GameObject playerObject;
        [NonSerialized] public Player player;
        [NonSerialized] public List<Mob> mobs = new List<Mob>();
        [NonSerialized] public List<Mob> mobsToClear = new List<Mob>();
        [NonSerialized] public Camera ppCamera;
        [NonSerialized] public GameObject mobHolder;
        [NonSerialized] public bool blockInput = false;
        [NonSerialized] public bool isGameStart = false;
        [NonSerialized] public bool isGameOver = false;

        public GameObject playerPrefab;
        public GameObject childPrefab;
        public GameObject copPrefab;

        public GameObject startScreen;
        public GameObject setupScreen;
        public GameObject gameOverScreen;
        public Text levelText;

        public Text eatenText;
        public Text timerText;
        public int level = 0;
        public int kidsToEat = 5;
        public int kidsEaten = 0;
        public int timer;

        void Awake()
        {
            if (s == null)
            {
                s = this;
            }
            else if (s != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            ppCamera = FindObjectOfType<Camera>();
            mobHolder = new GameObject("MobHolder");
        }

        void Start()
        {
            startScreen.SetActive(true);
            setupScreen.SetActive(false);
            gameOverScreen.SetActive(false);

            playerObject = Instantiate(playerPrefab, MapManager.s.tilemap.GetCellCenterWorld(new Vector3Int(2, 2, 0)), Quaternion.identity);
            DontDestroyOnLoad(playerObject);
            player = playerObject.GetComponent<Player>();

            ppCamera.transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -5);
            ppCamera.transform.SetParent(playerObject.transform);

            startScreen.SetActive(true);
            isGameStart = true;
            Invoke("AllowButtonPress", 2f);
        }

        public void AllowButtonPress()
        {
            blockInput = false;
        }

        public void SetupNextLevel()
        {
            blockInput = true;
            CancelInvoke("SpawnChild");
            CancelInvoke("SpawnCop");
            CancelInvoke("DecrementTimer");

            kidsEaten = 0;
            level += 1;
            kidsToEat += 2;

            levelText.text = "Level " + level;
            setupScreen.SetActive(true);
            MapManager.s.CreateMap(level);
            eatenText.text = kidsEaten + "/" + kidsToEat + " Kids Delivered";
            mobs = new List<Mob>();
            mobsToClear = new List<Mob>();
            ResetTimer();

            playerObject.transform.position = MapManager.s.tilemap.GetCellCenterWorld(new Vector3Int(20, 13, 0));
            Invoke("FinishSetup", 2f);
        }

        void FinishSetup()
        {
            InvokeRepeating("SpawnChild", .5f, .7f);
            InvokeRepeating("SpawnCop", 5f, 10f);
            InvokeRepeating("DecrementTimer", 1f, 1f);
            setupScreen.SetActive(false);
            blockInput = false;
        }

        void ResetTimer()
        {
            timer = 120;
        }

        void DecrementTimer()
        {
            timer -= 1;
            if (timer <= 0)
            {
                gameOverScreen.SetActive(true);
                isGameOver = true;
            }
            timerText.text = "Time: " + timer;
        }

        public void DamagePlayer()
        {
            player.invulnTime -= Time.deltaTime;
            if (player.invulnTime < 0)
            {
                player.isInvuln = false;
            }
            if (!player.isInvuln)
            {
                timer -= 5;
                timerText.text = "Time: " + timer;
                if (timer <= 0)
                {
                    gameOverScreen.SetActive(true);
                    isGameOver = true;
                }
                player.isInvuln = true;
                player.invulnTime = 1f;
                if (player.isGrabbing)
                {
                    player.DropChild();
                }
            }
        }
        
        public void Restart()
        {
            gameOverScreen.SetActive(false);
            isGameOver = false;
            level = 0;
            kidsToEat = 5;
            SetupNextLevel();
        }

        public void SpawnChild()
        {
            int max = UnityEngine.Random.Range(0, 4);
            for (int i = 0; i < max; i++)
            {
                GameObject childInstance = Instantiate(childPrefab, MapManager.s.GetRandomChildSpawnWorld(), Quaternion.identity);
                childInstance.transform.SetParent(mobHolder.transform);
            }
        }

        public void SpawnCop()
        {
            int max = UnityEngine.Random.Range(0, 2);
            max = 1;
            for (int i = 0; i < max; i++)
            {
                GameObject copInstance = Instantiate(copPrefab, MapManager.s.GetRandomCopSpawnWorld(), Quaternion.identity);
                copInstance.transform.SetParent(mobHolder.transform);
            }
        }

        void FixedUpdate()
        {
            foreach (Mob mob in mobs)
            {
                mob.MoveMob();
            }
            foreach (Mob mob in mobsToClear)
            {
                mobs.Remove(mob);
                Destroy(mob.gameObject);
            }
            mobsToClear.Clear();
        }
    }
}