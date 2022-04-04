using System;
using UnityEngine;


public class CrosshairSwitching : MonoBehaviour
{
    [SerializeField] private Crosshair[] crosshairs;
    [SerializeField] private WeaponSwitching wepSwitch;
    private SpriteRenderer spriteRenderer;
    private int currCrosshair = 0;

    [System.Serializable]
    public class Crosshair
    {
        [SerializeField] private string wepName;
        public int wepId;
        public Transform crosshairPrefab;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChangeCrosshair();
        
        wepSwitch.onWeaponChangeDelegate += WeaponSwitching_OnWeaponChange;
    }

    private void OnDestroy()
    {
        wepSwitch.onWeaponChangeDelegate -= WeaponSwitching_OnWeaponChange;
    }

    private void WeaponSwitching_OnWeaponChange()
    {
        ChangeCrosshair();
    }

    private void ChangeCrosshair()
    {
        currCrosshair = wepSwitch.selectedWeapon;
        spriteRenderer.sprite = crosshairs[currCrosshair].crosshairPrefab.GetComponent<SpriteRenderer>().sprite;
    }
}