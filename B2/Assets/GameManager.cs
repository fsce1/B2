using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public Transform curInfilPoint;
    public TMP_Text curFirearmText;
    public List<Transform> infilLocations;
    public List<GameObject> infilIcons;
    public Camera infilCamera;
    public GameObject infilPointIcon;

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
        infilCamera.gameObject.SetActive(true);

        foreach (Transform t in infilLocations)
        {
            infilIcons.Add(Instantiate(infilPointIcon, t));
        }
    }
    public void Infiltrate()
    {
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
    }

}
