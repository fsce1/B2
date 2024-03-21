using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GM")]
    public static GameManager GM;

    [Header("Player")]
    public Player player;
    public GameObject firearmPrefab;
    public Camera playCamera;
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
        infilCamera.gameObject.SetActive(false);
        foreach (Transform t in infilLocations)
        {
            Destroy(t.gameObject);
        }
        GameObject g = Instantiate(firearmPrefab, player.swayController.transform);
        player.firearm = g.GetComponent<Firearm>();

        player.transform.position = curInfilPoint.position;
        player.Initialize();
        player.gameObject.SetActive(true);

    }

}
