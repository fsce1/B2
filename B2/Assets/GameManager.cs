using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GM")]
    public static GameManager GM;
    public Terrain terrain;
    [Header("Player")]
    public GameObject playerPrefab;
    public GameObject curFirearm;
    public TMP_Text curFirearmText;
    public Player player;
    public List<Camera> playCameras;

    [Header("InfilScreen")]
    public bool skipInfilScreen;
    public Transform infilCanvas;
    public Camera infilCamera;
    public List<Transform> infilLocations;
    public GameObject infilPointIcon;
    public List<GameObject> infilIcons;
    public Transform curInfilPoint;

    [Header("Enemies")]
    public GameObject enemyPrefab;
    public List<Transform> enemySpawns;
    public Vector2Int enemyCountBounds;
    public List<EnemyStateMachine> enemies;

    void Awake()
    {
        if (GM == null)
            GM = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    public void ChangeCurWeapon(GameObject _firearm)
    {
        curFirearm = _firearm;
        curFirearmText.text = curFirearm.name;
    }
    public void ChangeCurInfilPoint()
    {

    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        foreach (Transform t in infilLocations)
        {
            GameObject g = Instantiate(infilPointIcon, infilCanvas, false);
            infilIcons.Add(g);
            g.GetComponent<RectTransform>().localPosition = new(t.position.x, t.position.z, 0);
            g.GetComponent<InfilPointButton>().infilPoint = t;
            //g.GetComponent<RectTransform>().position = new(t.position.x, t.position.z, 0);
            //infilCamera.WorldToScreenPoint();

        }
        curInfilPoint = infilLocations[0];

        if (skipInfilScreen)
        {
            Infiltrate();
        }
        else
        {
            infilCamera.gameObject.SetActive(true);
        }

    }
    public void Infiltrate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        foreach (Transform t in infilLocations) Destroy(t.gameObject);

        player = Instantiate(playerPrefab, curInfilPoint.position, curInfilPoint.localRotation).GetComponent<Player>();
        playCameras = player.GetComponentsInChildren<Camera>().ToList<Camera>();
        player.firearm = Instantiate(curFirearm, player.swayController.transform).GetComponent<Firearm>(); 
        //player.transform.position = curInfilPoint.position;
        player.transform.rotation = curInfilPoint.rotation;
        player.Initialize();
        player.gameObject.SetActive(true);

        infilCamera.gameObject.SetActive(false);

        int enemiesToSpawn = Random.Range(enemyCountBounds.x, enemyCountBounds.y);
        for (int i = enemiesToSpawn; i > 0; i--)
        {
            int spawnPos = Random.Range(0, enemySpawns.Count - 1);
            EnemyStateMachine e = Instantiate(enemyPrefab, enemySpawns[spawnPos]).GetComponent<EnemyStateMachine>();
            //transform.position = new Vector3(Random.Range(-256f, 256f), 0, Random.Range(-256f, 256f));

            //transform.position = new Vector3(transform.position.x, terrain.SampleHeight(transform.position), transform.position.z);
            //EnemyStateMachine e = Instantiate(enemyPrefab, transform).GetComponent<EnemyStateMachine>();
            //enemySpawns.Remove(enemySpawns[spawnPos]);
            //e.transform.SetParent(null);
            //transform.position = Vector3.zero;
            enemies.Add(e);
        }

        foreach (EnemyStateMachine e in enemies)
        {
            e.Initialize();
            e.transform.parent = null;
        }
    }

}
