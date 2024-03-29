using UnityEngine;

public class HandPlacement : MonoBehaviour
{
    //Components
    [Header("RigLayer Targets")]
    [SerializeField] private Transform leftHandIKTarget;
    [SerializeField] private Transform rightHandIKTarget;
 
    private WeaponSwitching wepSwitch;
    

    [Header("Positions for each guns")]
    [SerializeField] private HandPositions[] positionsArray;

    private Transform currLeftHandPos;
    private Transform currRightHandPos;

    

    [System.Serializable]
    private class HandPositions
    {
        [SerializeField] private string name;
        public Transform leftHandPos;
        public Transform rightHandPos;
    }

    void Start()
    {
        wepSwitch = GetComponent<WeaponSwitching>();

        //If have weapon switching, means its the player
        if (wepSwitch != null)
        {
            WeaponSwitching.OnWeaponChange += WeaponSwitching_OnWeaponChange;
        }
        
       
        InitialPos();
    }

    void OnDestroy()
    {
        if (wepSwitch != null)
        {
            WeaponSwitching.OnWeaponChange -= WeaponSwitching_OnWeaponChange;
        }
    }

    
    private void Update()
    {
        if (leftHandIKTarget != null)
        {
            leftHandIKTarget.position = currLeftHandPos.position;
        }

        if (rightHandIKTarget != null)
        {
            rightHandIKTarget.position = currRightHandPos.position;
        }
        
    }

    private void WeaponSwitching_OnWeaponChange()
    {
        HandPositions currPositions = positionsArray[wepSwitch.selectedWeapon];

        currLeftHandPos = currPositions.leftHandPos;
        currRightHandPos = currPositions.rightHandPos;
    }

    void InitialPos()
    {
        HandPositions currPositions = positionsArray[0];

        if (leftHandIKTarget != null)
        {
            currLeftHandPos = currPositions.leftHandPos; 
        }

        if (rightHandIKTarget != null)
        {
            currRightHandPos = currPositions.rightHandPos;
        }
    }
}
