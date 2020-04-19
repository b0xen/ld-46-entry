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
        public int level;
        public int kidsToEat;
        public int kidsEaten;
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
            level = 0;
            kidsToEat = 0;
            kidsEaten = 0;
        }

        void Start()
        {
            QualitySettings.vSyncCount = 1;
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
            MapManager.s.tilemap.ClearAllTiles();
            MapManager.s.homeCells = new List<Vector3Int>();
            MapManager.s.homeCellData = new Dictionary<Vector3Int, CellData>();
            MapManager.s.sewerCells = new List<Vector3Int>();
            MapManager.s.sewerCellData = new Dictionary<Vector3Int, Vector3>();
            MapManager.s.nodes = new List<Vector3Int>();
            MapManager.s.adjacentNodes = new List<List<Vector3Int>>();
            player.currentSpeed = player.baseSpeed;
            player.isGrabbing = false;
            player.grabTarget = null;
            if (player.joint != null)
            {
                Destroy(player.joint);
            }
            player.invulnTime = 1f;
            player.isInvuln = false;

            StartCoroutine(WaitOneSecond());
            ResetTimer();
            kidsEaten = 0;
            level += 1;
            kidsToEat += 2;

            levelText.text = "Level " + level;
            setupScreen.SetActive(true);
            MapManager.s.CreateMap();
            eatenText.text = kidsEaten + "/" + kidsToEat + " Kids Delivered";
            foreach (Mob mob in mobs)
            {
                if (mob != null)
                {
                    Destroy(mob.gameObject);
                }
            }
            foreach (Mob mob in mobsToClear)
            {
                if (mob != null)
                {
                    Destroy(mob.gameObject);
                }
            }
            mobs = new List<Mob>();
            mobsToClear = new List<Mob>();

            playerObject.transform.position = MapManager.s.tilemap.GetCellCenterWorld(new Vector3Int(20, 13, 0));
            Invoke("FinishSetup", 2f);
        }

        IEnumerator WaitOneSecond()
        {
            yield return new WaitForSeconds(1f);
        }
        void FinishSetup()
        {
            InvokeRepeating("SpawnChild", .5f, .7f + .3f * level);
            InvokeRepeating("SpawnCop", 5f, Math.Max(10f - 1f * level, 2f));
            InvokeRepeating("DecrementTimer", 1f, 1f);
            setupScreen.SetActive(false);
            blockInput = false;
        }

        void ResetTimer()
        {
            timer = 120;
            timerText.text = "Time: " + timer;
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
                timerText.text = "-5 Time: " + timer;
                if (timer <= 0)
                {
                    gameOverScreen.SetActive(true);
                    isGameOver = true;
                }
                player.isInvuln = true;
                player.invulnTime = 1f;
            }
        }

        public void EatChild()
        {
            kidsEaten += 1;
            if (kidsEaten >= kidsToEat)
            {
                SetupNextLevel();
            }
            eatenText.text = kidsEaten + "/" + kidsToEat + " Kids Delivered";
        }
        
        public void Restart()
        {
            gameOverScreen.SetActive(false);
            isGameOver = false;
            level = 0;
            kidsToEat = 0;
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