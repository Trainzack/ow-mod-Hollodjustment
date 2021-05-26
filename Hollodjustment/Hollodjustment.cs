using OWML.ModHelper;
using OWML.Utils;
using OWML.Common;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hollodjustment
{


    public class Hollodjustment : ModBehaviour
    {
        // Whether we have already cloned Hollow's Lantern. This is set to prevent an infinite loop.
        private Boolean cloned = false;

        // This is the delay that's built in to meteorLauncher's launch routine. We record it here so that we can counteract it.
        private const float _builtInDelay = 2.3f;

        private float _delayMult = 1f;

        // This array contains the values for the delay mult that we use for each setting in the "Meteor Frequency" slider.
        private float[] _delayMultSettings = 
            /* 0 */ {float.PositiveInfinity,
            /* 1 */ 8.0f,  
            /* 2 */ 3.0f,
            /* 3 */ 2.0f,
            /* 4 */ 1.5f,
            /* 5 */ 1.0f,
            /* 6 */ 0.75f,
            /* 7 */ 0.5f,
            /* 8 */ 0.3f,
            /* 9 */ 0.1f,
            /* 10*/ 0.05f };

        // This array contains the values for the damage mult that we use for each setting in the "Meteor Damage" slider.
        private float[] _damageMultSettings = 
            /* 0 */ {0,
            /* 1 */ 0.05f,  
            /* 2 */ 0.1f,
            /* 3 */ 0.2f,
            /* 4 */ 0.5f,
            /* 5 */ 1.0f,
            /* 6 */ 1.5f,
            /* 7 */ 2f,
            /* 8 */ 3f,
            /* 9 */ 8.0f,
            /* 10*/ 100.0f };



        private float _damageMult = 1f;

        private void Awake()
        {
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.

            ModHelper.Events.Subscribe<MeteorLauncher>(Events.AfterStart);
            ModHelper.Events.Subscribe<MeteorController>(Events.AfterAwake);



            //ModHelper.Console.WriteLine($"My mod {nameof(Hollodjustment)} is loaded!", MessageType.Success);

            ModHelper.Events.Event += OnEvent;
            
            // Cloning Hollow's Lantern is broken, so it has been disabled.
            //LoadManager.OnCompleteSceneLoad += OnSceneLoaded;

        }

        private void OnSceneLoaded(OWScene originalScene, OWScene scene)
        {
            if (scene == OWScene.SolarSystem)
            {
                AstroObject[] astroObjects = Resources.FindObjectsOfTypeAll<AstroObject>();

                AstroObject hL = null;

                foreach (AstroObject a in astroObjects)
                {
                    if (a.GetAstroObjectName() == AstroObject.Name.VolcanicMoon)
                    {
                        hL = a;
                        break;
                    }
                }

                //Todo throw error if hL is still null

                cloneHollowsLantern(hL);
            }
        }

        private void OnEvent(MonoBehaviour behaviour, Events ev)
        {
            switch (behaviour)
            {
                case MeteorLauncher meteorLauncher when ev == Events.AfterStart:
                    // This event is fired at the start of each loop, once per each meteor launcher.
                    // Here, we adjust the properties of the each meteorlauncher.
                    ModHelper.Console.WriteLine("Changing meteor launcher interval; multiplying by " + _delayMult + ".", MessageType.Message);
                    meteorLauncher.SetValue("_minInterval", (meteorLauncher.GetValue<float>("_minInterval") + _builtInDelay) * _delayMult - _builtInDelay);
                    meteorLauncher.SetValue("_maxInterval", (meteorLauncher.GetValue<float>("_maxInterval") + _builtInDelay) * _delayMult - _builtInDelay);
                    break;
                case MeteorController meteorController when ev == Events.AfterAwake:
                    // This event is fired at the start of each loop, once per each meteor. (Old meteors are reused)
                    // Here, we adjust the damage of each meteor to be what we want.
                    ModHelper.Console.WriteLine("Changing launched meteor damage; multiplying by " + _damageMult + ".", MessageType.Message);
                    meteorController.SetValue("_minDamage", meteorController.GetValue<float>("_minDamage") * _damageMult);
                    meteorController.SetValue("_maxDamage", meteorController.GetValue<float>("_maxDamage") * _damageMult);
                    break;

            }
        }

        public override void Configure(IModConfig config)
        {
            
            _delayMult = _delayMultSettings[config.GetSettingsValue<int>("delay")];
            _damageMult = _damageMultSettings[config.GetSettingsValue<int>("damage")];
        }

        private void cloneHollowsLantern(AstroObject hollowsLantern)
        {
            // TODO: This method of cloning leaves a lot of things broken; likely many refrences need to be updated to get proper functionality.
            // Most notably, the clone does not orbit brittle hollow.

            ModHelper.Console.WriteLine("Cloning Hollow's Lantern.", MessageType.Message);
            AstroObject clone = AstroObject.Instantiate(hollowsLantern);

        }
    }
}
