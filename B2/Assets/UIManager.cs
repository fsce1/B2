using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Camera UICamera;
    public RectTransform crosshair;
    //public RectTransform restCrosshair;
    public float crosshairSize;
    public Slider ammo;
    public TMP_Text zoomText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GM.player.swayController.isAiming)
        {
            crosshair.gameObject.SetActive(false);
        }
        else
        {
            if (Physics.Raycast(GameManager.GM.player.firearm.barrelPoint.position, GameManager.GM.player.firearm.transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                crosshair.gameObject.SetActive(true);
                crosshair.position = UICamera.WorldToScreenPoint(hit.point);
                crosshair.localScale = crosshairSize * Vector3.one;
            }
            else crosshair.gameObject.SetActive(false);

            //if (Physics.Raycast(GameManager.GM.player.firearm.barrelPoint.position, GameManager.GM.player.swayController.restRot, out RaycastHit hit2, Mathf.Infinity))
            //{
            //    restCrosshair.gameObject.SetActive(true);
            //    restCrosshair.position = UICamera.WorldToScreenPoint(hit2.point);
            //    restCrosshair.localScale = crosshairSize * Vector3.one;
            //}
            //else restCrosshair.gameObject.SetActive(false);
        }


        ammo.maxValue = GameManager.GM.player.firearm.info.magazineSize;
        ammo.value = GameManager.GM.player.firearm.roundsInMag;

        if(GameManager.GM.player.firearm.hasScope && GameManager.GM.player.swayController.isAiming)
        {
            zoomText.text = (Mathf.Round(GameManager.GM.player.firearm.curZoom *100f)/100).ToString() + "x";
        }
        else { zoomText.text = ""; }
    }
}
