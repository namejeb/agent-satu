using UnityEngine;

public class ArmToPlayerTracking : MonoBehaviour
{
    //reference to other scripts
    [SerializeField] private Transform pivotTransform;
    [SerializeField] private OverallHp overallHp;
    [SerializeField] private Enemy_Agro enemyAgro;
    private PlayerMovement playerMovement;
    
    //[SerializeField] private Enemy_Flipped enemyFlipped;


    
    [SerializeField] private float followAngleOffset;
    //[SerializeField] private bool isFacingRight = false;
    private Vector2 playerPosition;


 
    
    void Start()
    {
        playerMovement  = GameObject.FindGameObjectWithTag("PlayerBody").GetComponent<PlayerMovement>();
        
        overallHp.onDeathDelegate += OverallHp_OnDeath;
    }

    //track player's Vector x and y
    void Update()
    {
        if (!enemyAgro.GetInRange()) return;

        playerPosition = playerMovement.GetPlayerPos();
        PointToPlayer();
    }

    void OverallHp_OnDeath()
    {
        overallHp.onDeathDelegate -= OverallHp_OnDeath;
        enabled = false;
    }
    
    private void PointToPlayer()
    {
        Vector2 lookDir = (Vector2)pivotTransform.position - playerPosition;
        float angleTowards = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        
        if (angleTowards > 90f || angleTowards < -90f)
        {
            //Inverted rotation
            pivotTransform.eulerAngles = new Vector3(180f, 0f, -angleTowards - followAngleOffset);
        }
        else
        {
            //Normal rotation
            pivotTransform.eulerAngles = new Vector3(0f, 0f, angleTowards - followAngleOffset);
        }
    }
    
    //jake ver
    //If pointing to left side, invert the pivot's x rotation and angleTowards
    //to accomodate sprite rotations
    // if (isFacingRight)
    // {
    //     if (angleTowards > 90f || angleTowards < -90f)
    //     {
    //         //Inverted rotation
    //         pivotTransform.eulerAngles = new Vector3(180f, 0f, -angleTowards - followAngleOffset);
    //     }
    // }
    // else
    // {
    //     if (!(angleTowards > 90f || angleTowards < -90f))
    //     {
    //         //Normal rotation
    //         pivotTransform.eulerAngles = new Vector3(0f, 0f, angleTowards - followAngleOffset);
    //     }
    // }
}
