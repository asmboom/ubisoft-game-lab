﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour 
{
    /// <summary>
    /// The position at which the character ejects mass. 
    /// </summary> 
    public Transform massEjectionTransform;
   
    /// <summary>
    /// The light ball ejected by the player when thrusting
    /// </summary>
    public GameObject lightBallPrefab;
    
    /// <summary>
    /// The amount of force applied on the player when ejecting one piece of mass.
    /// </summary>
    [Tooltip("The amount of force applied on the player when ejecting one piece of mass.")]
    public float thrustForce;
    
    /// <summary>
    /// The amount of light energy spent when ejecting one piece of mass.
    /// </summary>
    [Tooltip("The amount of light energy spent when ejecting one piece of mass.")]
    public float thrustEnergyCost = 1;

    /** Caches the player's components */
    private new Transform transform;
    private new Rigidbody rigidbody;
    private LightEnergy lightEnergy;

	// Use this for initialization
    void Start () 
    {
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        lightEnergy = GetComponent<LightEnergy>();
    }
	
	// Update is called once per frame
    void Update () 
    {
        if (Input.GetButtonDown("Thrust"))
        {
            // Eject mass in the direction of the left stick
            EjectMass(massEjectionTransform.up);
        }
       
        // Makes the character follow the left stick's rotation
        FollowLeftStickRotation();
        
        // Ensure that the rigidbody never spins
        rigidbody.angularVelocity = Vector3.zero;
       
        //Debug.Log("Eject Mass in direction: " + InputManager.GetLeftStick());
    }
    
    /// <summary>
    /// Makes the character follow the left stick's rotation.
    /// </summary>
    private void FollowLeftStickRotation()
    {
        // Get the direction the left stick is pointing to
        Vector2 leftStickDirection = InputManager.GetLeftStick();
        
        if(leftStickDirection.sqrMagnitude > 0.01f)
        {
            //Debug.Log("Move the player " + Time.time);
            
            // Get the angle the left stick is pointing in
            float leftStickAngle = 0.0f;
            if (leftStickDirection.x != 0 || leftStickDirection.y != 0)
            {
                // 90-degree offset to ensure angle is relative to +y-axis
                leftStickAngle = Mathf.Atan2(leftStickDirection.y,leftStickDirection.x) * Mathf.Rad2Deg - 90;
            }
            
            // Make the character face in the direction of the left stick
            transform.localEulerAngles = new Vector3(0,0,leftStickAngle);
        }
    }
    
    /// <summary>
    /// Ejects mass in the given direction, pushing the gameObject in the opposite direction.
    /// </summary>
    private void EjectMass(Vector2 direction)
    {
        // Spawn a light mass at the position of the character's mass ejector
        GameObject lightMass = GameObject.Instantiate(lightBallPrefab);
        lightMass.transform.position = massEjectionTransform.position;
        // Push the light mass in the given direction
        lightMass.GetComponent<Rigidbody>().AddForce(thrustForce * direction, ForceMode.Impulse);
        
        // Push the character in the opposite direction that the mass was ejected
        rigidbody.AddForce(-thrustForce * direction, ForceMode.Impulse);
        // Deplete energy from the player for each ejection
        lightEnergy.Deplete(thrustEnergyCost);
    }
}
