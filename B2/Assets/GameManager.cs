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

    [Header("Player")]
    public GameObject playerPrefab;
    public Player player;
    public GameObject firearmPrefab;
    public List<Camera> playCameras;

    [Header("InfilScreen")]
    public bool skipInfilScreen;
    public int defaultInfilPoint;
    public Transform curInfilPoint;
    public TMP_Text curFirearmText;
    public List<Transform> infilLocations;
    public List<GameObject> infilIcons;
    public Camera infilCamera;
    public GameObject infilPointIcon;

    [Header("Enemies")]
    public GameObject enemyPrefab;
    public List<Transform> enemySpawns;
    public Vector2Int enemyCountBounds;
    public List<Enemy> enemies;

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
        firearmPrefab = _firearm;
        curFirearmText.text = firearmPrefab.name;
    }
    public void ChangeInfilPoint(Transform infilPoint)
    {

    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        foreach (Transform t in infilLocations)
        {
            infilIcons.Add(Instantiate(infilPointIcon, t));
        }
        curInfilPoint = infilLocations[defaultInfilPoint];

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

        GameObject p = Instantiate(playerPrefab, curInfilPoint.position, Quaternion.identity);
        //p.transform.parent = null;

        player = p.GetComponent<Player>();
        playCameras = player.GetComponentsInChildren<Camera>().ToList<Camera>();
        GameObject g = Instantiate(firearmPrefab, player.swayController.transform);
        player.firearm = g.GetComponent<Firearm>();
        player.transform.position = curInfilPoint.position;
        player.Initialize();
        player.gameObject.SetActive(true);

        infilCamera.gameObject.SetActive(false);

        int enemiesToSpawn = Random.Range(enemyCountBounds.x, enemyCountBounds.y);
        for (int i = enemiesToSpawn; i > 0; i--)
        {
            int spawnPos = Random.Range(0, enemySpawns.Count - 1);

            Enemy e = Instantiate(enemyPrefab, enemySpawns[spawnPos]).GetComponent<Enemy>();
            enemySpawns.Remove(enemySpawns[spawnPos]);
            enemies.Add(e);
        }

        foreach (Enemy e in enemies)
        {
            e.Initialize();
            e.transform.parent = null;
            
        }
    }

}
