﻿using UnityEngine;

public class FlareSpawner : MonoBehaviour 
{
    [SerializeField]
    [Tooltip("Refers to flare game object")]
    private GameObject flareObject;
    
    [SerializeField]
    [Tooltip("Refers to the flare spwan zone")]
    private Transform flareSpawnObject;
    
    [SerializeField]
    [Tooltip("Time between flare shoots")]
    private float cooldownTime;
    
    [SerializeField]
    [Tooltip("Cost to use your flare")]
    private float flareEnergyCost;
    
    [SerializeField]
    [Tooltip("Percentage of energy needed to use flare. 1 = 100%")]
    private float flareCostPercentage;
    
    [SerializeField]
    [Tooltip("The amount of recoil applied on the player when shooting the flare")]
    private float recoilForce;
        
    private float timer;
    private LightSource lightSource;
    private ControllerRumble controllerRumble;  // Caches the component that rumbles the controller
    private new Rigidbody rigidbody;    
    private SmoothCamera smoothCamera;
	

    void Start()
    {
        this.timer = cooldownTime;
        this.lightSource = GetComponent<LightSource>();
        this.controllerRumble = GetComponent<ControllerRumble>();
        this.rigidbody = GetComponent<Rigidbody>();
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            this.smoothCamera = mainCamera.GetComponent<SmoothCamera>();
        }
    }

    void Update() 
    {
        if (Input.GetButtonDown("UseFlare"))
        {
            float cost = flareEnergyCost * flareCostPercentage;
            if (((timer += Time.deltaTime) >= cooldownTime) && (lightSource.LightEnergy.CurrentEnergy > (flareEnergyCost + cost)))
            {
                Instantiate(flareObject, flareSpawnObject.position, flareSpawnObject.rotation);
                lightSource.LightEnergy.Deplete(flareEnergyCost);
                // Apply recoil in the opposite direction the flare was shot
                rigidbody.AddForce(-flareSpawnObject.right * recoilForce, ForceMode.Impulse);
                controllerRumble.ShotFlare();   // Rumble the controller
                timer = 0.0f;
                
                AkSoundEngine.PostEvent("Flare", this.gameObject);
                // STELLA if eaten shut off

                //reset all values for the zoom when ever a player fires a flare
                if (smoothCamera != null)
                {
                    smoothCamera.FlareShoot();
                    smoothCamera.ResetTimer();   
                }                    
            }
            else
            {
                AkSoundEngine.PostEvent("LowEnergy", this.gameObject);
            }
        }
    }
}