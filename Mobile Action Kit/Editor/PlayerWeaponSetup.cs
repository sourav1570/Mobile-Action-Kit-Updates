using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class PlayerWeaponSetup : EditorWindow
    {
        // Fields
        public GameObject WeaponPrefab;
        public GameObject WeaponsRoot;
        //public Canvas Canvas2D;
        public GameObject WeaponModel;
        public Animator WeaponAnimatorComponent;
        public RuntimeAnimatorController RuntimeAnimatorController;
        public string WeaponName;
        public GameObject Player;
        public PlayerManager PlayerManagerScript;
        public Camera PlayerCamera;
        public Camera WeaponCamera;
        public GameObject Supressor;
        public GameObject ShootingPoint;
        public GameObject BulletShellSpawnPoint;
        public bool UseMuzzleMesh;
        public GameObject MuzzleMesh;
        public float MuzzleMeshFlashTime = 0.1f;
        public ParticleSystem MuzzleFlashParticles;
        //public bool AddSniperScope;
        //public Canvas Canvas2D;
        //public GameObject SniperScopePrefab;
        //public GameObject SniperScopeSlidersPrefab;
        //public bool AddReticleAdjusterToSniperScope;
        public bool EnableProceduralBobbing = true;
        public AudioClip WieldAudioClip;
        public AudioClip FireAudioClip;
        public AudioClip ReloadEmptyAudioClip;
        public AudioClip ReloadAudioClip;


        public bool IsWeaponUIReady = false;
        public PlayerWeapon PlayerWeaponScript;
        public Button FireButtonUI;
        public GameObject Crosshair;
        public TextMeshProUGUI TotalAmmoText;
        public TextMeshProUGUI ShotsInMagCountText;

        private Vector2 scrollPosition; // For scroll view

        PlayerWeapon PlayerWeaponScriptTemporary;

        [MenuItem("Tools/Mobile Action Kit/Player/FireArms/Setup Weapon", priority = 1)]
        public static void ShowWindow()
        {
            GetWindow<PlayerWeaponSetup>("Player Weapon Setup");
        }
        private void OnGUI()
        {
            // Set a dynamic label width based on the window size
            EditorGUIUtility.labelWidth = Mathf.Min(position.width * 0.4f, 200); // 40% of window width, capped at 200px

            // Begin Scroll View
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Draw fields
            EditorGUILayout.LabelField("Weapon Settings", EditorStyles.boldLabel);
            WeaponPrefab = (GameObject)EditorGUILayout.ObjectField("Weapon Prefab", WeaponPrefab, typeof(GameObject), false);
            WeaponsRoot = (GameObject)EditorGUILayout.ObjectField("Weapons Root", WeaponsRoot, typeof(GameObject), true);
            WeaponModel = (GameObject)EditorGUILayout.ObjectField("Weapon Model", WeaponModel, typeof(GameObject), true);

            WeaponAnimatorComponent = (Animator)EditorGUILayout.ObjectField("Weapon Animator Component", WeaponAnimatorComponent, typeof(Animator), true);
            RuntimeAnimatorController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Runtime Animator Controller", RuntimeAnimatorController, typeof(RuntimeAnimatorController), true);

            //EditorGUILayout.LabelField("Sniper Scope Settings", EditorStyles.boldLabel);
            //AddSniperScope = EditorGUILayout.Toggle("Add Sniper Scope", AddSniperScope);

            //if (AddSniperScope)
            //{
            //    Canvas2D = (Canvas)EditorGUILayout.ObjectField("Canvas 2D", Canvas2D, typeof(Canvas), true);
            //    SniperScopePrefab = (GameObject)EditorGUILayout.ObjectField("Sniper Scope Prefab", SniperScopePrefab, typeof(GameObject), false);
            //    SniperScopeSlidersPrefab = (GameObject)EditorGUILayout.ObjectField("Sniper Scope Sliders Prefab", SniperScopeSlidersPrefab, typeof(GameObject), false);
            //    AddReticleAdjusterToSniperScope = EditorGUILayout.Toggle("Add Reticle Adjuster To Sniper Scope", AddReticleAdjusterToSniperScope);
            //}

            EnableProceduralBobbing = EditorGUILayout.Toggle("Enable Procedural Bobbing", EnableProceduralBobbing);
            WeaponName = EditorGUILayout.TextField("Weapon Name", WeaponName);

            EditorGUILayout.LabelField("Transform Settings", EditorStyles.boldLabel);
            Player = (GameObject)EditorGUILayout.ObjectField("Player", Player, typeof(GameObject), true);
            PlayerManagerScript = (PlayerManager)EditorGUILayout.ObjectField("Player Manager Script", PlayerManagerScript, typeof(PlayerManager), true);
            PlayerCamera = (Camera)EditorGUILayout.ObjectField("Player Camera", PlayerCamera, typeof(Camera), true);
            WeaponCamera = (Camera)EditorGUILayout.ObjectField("Weapon Camera", WeaponCamera, typeof(Camera), true);
            ShootingPoint = (GameObject)EditorGUILayout.ObjectField("Shooting Point", ShootingPoint, typeof(GameObject), true);
            BulletShellSpawnPoint = (GameObject)EditorGUILayout.ObjectField("Bullet Shell Spawn Point", BulletShellSpawnPoint, typeof(GameObject), true);


            EditorGUILayout.LabelField("Muzzle Flash Mesh Settings", EditorStyles.boldLabel);
            UseMuzzleMesh = EditorGUILayout.Toggle("Use Muzzle Mesh", UseMuzzleMesh);
            if (UseMuzzleMesh)
            {
                MuzzleMesh = (GameObject)EditorGUILayout.ObjectField("Muzzle Mesh", MuzzleMesh, typeof(GameObject), true);
                MuzzleMeshFlashTime = EditorGUILayout.FloatField("Muzzle Mesh Flash Time", MuzzleMeshFlashTime);
            }
            else
            {
                MuzzleFlashParticles = (ParticleSystem)EditorGUILayout.ObjectField("Muzzle Flash Particles", MuzzleFlashParticles, typeof(ParticleSystem), true);
            }

            EditorGUILayout.LabelField("Weapon Sound Settings", EditorStyles.boldLabel);
            WieldAudioClip = (AudioClip)EditorGUILayout.ObjectField("Wield Audio Clip", WieldAudioClip, typeof(AudioClip), false);
            FireAudioClip = (AudioClip)EditorGUILayout.ObjectField("Fire Audio Clip", FireAudioClip, typeof(AudioClip), false);
            ReloadEmptyAudioClip = (AudioClip)EditorGUILayout.ObjectField("Reload Empty Audio Clip", ReloadEmptyAudioClip, typeof(AudioClip), false);
            ReloadAudioClip = (AudioClip)EditorGUILayout.ObjectField("Reload Audio Clip", ReloadAudioClip, typeof(AudioClip), false);


            //if (AddSniperScope == true)
            //{
            //    if (GUILayout.Button("Add Required Sniper Scope Scripts"))
            //    {
            //        AddRequiredSniperScopeComponents();
            //    }
            //}

            // Create Weapon Button
            if (GUILayout.Button("Add Weapon To Player"))
            {
                CreateWeapon();


            }

            if (GUILayout.Button("Edit the Weapon Animations"))
            {
                EditAnimations();
            }

            if (GUILayout.Button("Tweak Weapon Positions & Rotations"))
            {
                TweakWeaponPosition();
            }


            IsWeaponUIReady = EditorGUILayout.Toggle("Is Weapon UI Ready", IsWeaponUIReady);
           
            if(IsWeaponUIReady == true)
            {
                PlayerWeaponScript = (PlayerWeapon)EditorGUILayout.ObjectField("Player Weapon Script", PlayerWeaponScript, typeof(PlayerWeapon), true);
                FireButtonUI = (Button)EditorGUILayout.ObjectField("Fire Button UI", FireButtonUI, typeof(Button), true);
                Crosshair = (GameObject)EditorGUILayout.ObjectField("Crosshair", Crosshair, typeof(GameObject), true);
                TotalAmmoText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Total Ammo Text", TotalAmmoText, typeof(TextMeshProUGUI), true);
                ShotsInMagCountText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Shots In Mag Count Text", ShotsInMagCountText, typeof(TextMeshProUGUI), true);

                if (GUILayout.Button("Assign UI Components To Player Weapon Script"))
                {
                    UIAlreadySetup();
                }
            }
                

            // End Scroll View
            EditorGUILayout.EndScrollView();

            // Reset label width back to default
            EditorGUIUtility.labelWidth = 0;
        } 
        private void EditAnimations()
        {
            EditorApplication.ExecuteMenuItem("Tools/Mobile Action Kit/Player/FireArms/Edit Weapon Animations");
        }
        private void TweakWeaponPosition()
        {
            EditorApplication.ExecuteMenuItem("Tools/Mobile Action Kit/Player/FireArms/Weapon View");
        }
        private void UIAlreadySetup()
        {
            if(PlayerWeaponScript != null)
            {
                PlayerWeaponScript.RequiredComponents.FireButton = FireButtonUI;
                PlayerWeaponScript.ShootingFeatures.WeaponCrosshair = Crosshair;
                PlayerWeaponScript.Reload.TotalAmmoText = TotalAmmoText;
                PlayerWeaponScript.Reload.ShotsInMagCountText = ShotsInMagCountText;
            }
        }
        //private void AddRequiredSniperScopeComponents()
        //{
        //    if (WeaponModel.GetComponent<SniperScope>() == null)
        //    {
        //        WeaponModel.AddComponent<SniperScope>();
        //    }
        //    if(AddReticleAdjusterToSniperScope == true && AddSniperScope == true)
        //    {
        //        if (WeaponModel.GetComponent<ReticleAdjusterScript>() == null)
        //        {
        //            WeaponModel.AddComponent<ReticleAdjusterScript>();
        //        }
        //    }
          
        //}
        private void CreateWeapon()
        {
            if (!WeaponPrefab || !WeaponsRoot || !WeaponModel || string.IsNullOrEmpty(WeaponName))
            {
                Debug.LogError("Please fill all required fields before creating the weapon.");
                return;
            }

            GameObject spawnedWeapon = Instantiate(WeaponPrefab, WeaponsRoot.transform);
            if (PrefabUtility.IsPartOfPrefabInstance(spawnedWeapon))
            {
                PrefabUtility.UnpackPrefabInstance(spawnedWeapon, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }

            string weaponModelName = WeaponModel.name;
            spawnedWeapon.name = weaponModelName + " Weapon";
             
            spawnedWeapon.transform.GetChild(0).name = weaponModelName + " Recoil";
            spawnedWeapon.transform.GetChild(0).transform.GetChild(0).name = weaponModelName + " Pivot";

            WeaponModel.transform.SetParent(spawnedWeapon.transform.GetChild(0).transform.GetChild(0));
            WeaponAnimatorComponent.runtimeAnimatorController = RuntimeAnimatorController;

            var weaponID = spawnedWeapon.transform.GetChild(0).GetComponent<WeaponId>();
            if (weaponID)
            {
                weaponID.WeaponName = WeaponName;
            }

            //var recoilScript = spawnedWeapon.transform.GetChild(0).GetComponent<RecoilScript>();
            //if (recoilScript)
            //{
            //    recoilScript.RecoilPosition = WeaponsRoot.transform;
            //}

            if(EnableProceduralBobbing == true)
            {
                if (WeaponModel.GetComponent<ProceduralBobbing>() == null)
                {
                    WeaponModel.AddComponent<ProceduralBobbing>();
                }

                if (WeaponModel.GetComponent<ProceduralIdleBobbing>() == null)
                {
                    WeaponModel.AddComponent<ProceduralIdleBobbing>();
                }

                WeaponModel.GetComponent<ProceduralBobbing>().PlayerManagerScript = PlayerManagerScript;
                WeaponModel.GetComponent<ProceduralBobbing>().FirstPersonControllerScript = Player.GetComponent<FirstPersonController>();
                WeaponModel.GetComponent<ProceduralBobbing>().BobObject = WeaponCamera.gameObject;

                WeaponModel.GetComponent<ProceduralIdleBobbing>().PlayerManagerScript = PlayerManagerScript;
                WeaponModel.GetComponent<ProceduralIdleBobbing>().BobObject = spawnedWeapon;

                PlayerManagerScript.BobbingScripts.Add(WeaponModel);
               
            }
           


            spawnedWeapon.transform.GetChild(0).GetComponent<RecoilScript>().CrouchScript = Player.GetComponent<Crouch>();

            var helper = spawnedWeapon.GetComponent<PlayerWeaponEditorScriptHelper>();

            var playerWeaponScript = helper.PlayerWeaponScript;

            if (helper)
            {
                
                GameObject[] AllSoundsGameObject = helper.Sounds;

                Transform ShootingPointParent = ShootingPoint.transform.parent;

                if (playerWeaponScript)
                {
                    playerWeaponScript.transform.SetParent(ShootingPoint.transform);

                    for(int x = 0; x < AllSoundsGameObject.Length; x++)
                    {
                        AllSoundsGameObject[x].transform.SetParent(spawnedWeapon.transform.GetChild(0).transform.GetChild(0));
                    }
                }

                playerWeaponScript.transform.SetParent(ShootingPointParent.transform);


                playerWeaponScript.GetComponent<PlayerWeapon>().ShootingFeatures.UseMuzzleFlashMesh = UseMuzzleMesh;

                if (MuzzleMesh != null && UseMuzzleMesh == true)
                {
                    MuzzleMesh.transform.parent = playerWeaponScript.transform.GetChild(0).transform;
                    playerWeaponScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashMesh = MuzzleMesh;
                    playerWeaponScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashMeshActiveTime = MuzzleMeshFlashTime;
                }
                if (MuzzleFlashParticles != null && UseMuzzleMesh == false)
                {
                    MuzzleFlashParticles.transform.parent = playerWeaponScript.transform.GetChild(0).transform;
                    playerWeaponScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashParticleFX = MuzzleFlashParticles;


                }
                playerWeaponScript.GetComponent<PlayerWeapon>().RequiredComponents.PlayerCamera = PlayerCamera;
                playerWeaponScript.GetComponent<PlayerWeapon>().RequiredComponents.WeaponCamera = WeaponCamera;
                playerWeaponScript.GetComponent<PlayerWeapon>().RequiredComponents.WeaponAnimatorComponent = WeaponAnimatorComponent;

                playerWeaponScript.GetComponent<PlayerWeapon>().WeaponPositionsRotationsDurations.WeaponPivot = spawnedWeapon.transform.GetChild(0).transform.GetChild(0).gameObject;

                playerWeaponScript.GetComponent<PlayerWeapon>().BulletShell.BulletShellSpawnPoint = BulletShellSpawnPoint;

                playerWeaponScript.GetComponent<PlayerWeapon>().WeaponSounds.FireSoundAudioSource.clip = FireAudioClip;
                playerWeaponScript.GetComponent<PlayerWeapon>().WeaponSounds.ReloadSoundAudioSource.clip = ReloadAudioClip;
                playerWeaponScript.GetComponent<PlayerWeapon>().WeaponSounds.ReloadEmptySoundAudioSource.clip = ReloadEmptyAudioClip;
                playerWeaponScript.GetComponent<PlayerWeapon>().WeaponSounds.WieldSoundAudioSource.clip = WieldAudioClip;

                PlayerWeaponScriptTemporary = playerWeaponScript.GetComponent<PlayerWeapon>();

                playerWeaponScript.transform.localPosition = ShootingPoint.transform.localPosition;
                playerWeaponScript.transform.localEulerAngles = ShootingPoint.transform.localEulerAngles;
                playerWeaponScript.transform.localScale = ShootingPoint.transform.localScale;

                PlayerManagerScript.PlayerWeaponScripts.Add(playerWeaponScript);

                
            }


            //if (AddSniperScope == true)
            //{
            //    GameObject spawnedscope = Instantiate(SniperScopePrefab, WeaponModel.transform);
            //    if (PrefabUtility.IsPartOfPrefabInstance(spawnedscope))
            //    {
            //        PrefabUtility.UnpackPrefabInstance(spawnedscope, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            //    }

            //    spawnedscope.transform.localPosition = Vector3.zero;
            //    spawnedscope.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
            //    spawnedscope.transform.localScale = new Vector3(1f, 1f, 1f);

            //    GameObject SniperScopeSlider = Instantiate(SniperScopeSlidersPrefab, Canvas2D.transform);
            //    if (PrefabUtility.IsPartOfPrefabInstance(SniperScopeSlider))
            //    {
            //        PrefabUtility.UnpackPrefabInstance(SniperScopeSlider.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            //    }

              

            //    if (WeaponModel.GetComponent<SniperScope>() != null)
            //    {
                    
            //        WeaponModel.GetComponent<SniperScope>().ScopeActivationProperties.WeaponGunscript = playerWeaponScript.GetComponent<PlayerWeapon>();
            //        WeaponModel.GetComponent<SniperScope>().ScopeActivationProperties.ScopeOverlay = spawnedscope;

            //        WeaponModel.GetComponent<SniperScope>().ZoomProperties.ControlZoomUsingMobileUI = true;
            //        WeaponModel.GetComponent<SniperScope>().ZoomProperties.UseAZoomSlider = true;
            //        WeaponModel.GetComponent<SniperScope>().ZoomProperties.ZoomSlider = SniperScopeSlider.transform.GetChild(2).GetComponent<Slider>();

            //        spawnedWeapon.GetComponent<PlayerWeaponEditorScriptHelper>().PlayerWeaponScript.ShootingFeatures.UseSniperScopeUI = true;
            //        spawnedWeapon.GetComponent<PlayerWeaponEditorScriptHelper>().PlayerWeaponScript.ShootingFeatures.SniperScopeScript = WeaponModel.GetComponent<SniperScope>();

            //        if (AddReticleAdjusterToSniperScope == false)
            //        {
            //            Object.DestroyImmediate(SniperScopeSlider.transform.GetChild(0));
            //            Object.DestroyImmediate(SniperScopeSlider.transform.GetChild(1));
            //        }
            //        else
            //        {
            //            WeaponModel.GetComponent<ReticleAdjusterScript>().SniperScopeGunscript = playerWeaponScript.GetComponent<PlayerWeapon>();
            //            WeaponModel.GetComponent<ReticleAdjusterScript>().ReticleAdjusterProperties.Reticle = spawnedscope.transform.GetChild(0).transform;
            //            WeaponModel.GetComponent<ReticleAdjusterScript>().ReticleAdjusterProperties.XSlider = SniperScopeSlider.transform.GetChild(1).GetComponent<Slider>();
            //            WeaponModel.GetComponent<ReticleAdjusterScript>().ReticleAdjusterProperties.YSlider = SniperScopeSlider.transform.GetChild(0).GetComponent<Slider>();
            //        }
            //    }

            //    spawnedscope.transform.name = "Sniper Scope";
            //    SniperScopeSlider.transform.name = "Sniper Scope Slider";
            //}



            if (helper != null)
            {
                DestroyImmediate(helper);
            }

            WeaponModel.SetActive(true);
            spawnedWeapon.gameObject.SetActive(true);

            Object.DestroyImmediate(ShootingPoint);
            Object.DestroyImmediate(spawnedWeapon.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject);
           
            Debug.Log("Weapon setup complete.");
        }
    }
}


//using UnityEngine;
//using UnityEditor;
//using UnityEngine.UI;
//using MobileActionKit;


//public class PlayerWeaponSetup : EditorWindow
//{
//    public GameObject WeaponToCopyFrom;
//    private GameObject duplicatedObject;

//    public Animator WeaponModel;
//    public Avatar ModelAvatar;
//    public GameObject WeaponShootingPoint;

//    public GameObject WeaponMetarig;
//    public bool UseMuzzleMesh;
//    public ParticleSystem MuzzleFlashParticles;
//    public GameObject MuzzleMesh;

//    public GameObject BulletShellSpawnPoint;

//    public Vector3 AimedPosition = Vector3.zero;
//    public Vector3 AimedRotation = Vector3.zero;

//    public Vector3 HipFirePosition = Vector3.zero;
//    public Vector3 HipFireRotation = Vector3.zero;

//    // Fields for SwitchToGrenadeUsingSwipe option
//    public int weaponID;
//    public string weaponName;

//    public bool IsThisDefaultWeapon;

//    private const int minWidth = 300;
//    private const int minHeight = 400;

//    private string RootObjName;

//    [MenuItem("Tools/MobileActionKit/Player/Weapon/Weapon Setup", priority = 0)]
//    public static void ShowWindow()
//    {
//        PlayerWeaponSetup window = GetWindow<PlayerWeaponSetup>("Weapon Setup");
//        window.minSize = new Vector2(minWidth, minHeight);
//    }
//    void OnGUI()
//    {
//        GUILayout.Label("Weapon Setup Settings", EditorStyles.boldLabel);

//        EditorGUIUtility.labelWidth = 200;

//        WeaponToCopyFrom = (GameObject)EditorGUILayout.ObjectField("Weapon To Copy From", WeaponToCopyFrom, typeof(GameObject), true);
//        WeaponModel = (Animator)EditorGUILayout.ObjectField("Weapon Model", WeaponModel, typeof(Animator), true);
//        ModelAvatar = (Avatar)EditorGUILayout.ObjectField("Avatar Mask", ModelAvatar, typeof(Avatar), true);

//        WeaponMetarig = (GameObject)EditorGUILayout.ObjectField("Weapon Metarig", WeaponMetarig, typeof(GameObject), true);
//        WeaponShootingPoint = (GameObject)EditorGUILayout.ObjectField("Weapon Shooting Point", WeaponShootingPoint, typeof(GameObject), true);

//        UseMuzzleMesh = EditorGUILayout.Toggle("Use Muzzle Mesh", UseMuzzleMesh);

//        if(UseMuzzleMesh == false)
//        {
//            MuzzleFlashParticles = (ParticleSystem)EditorGUILayout.ObjectField("Muzzle Flash Particles", MuzzleFlashParticles, typeof(ParticleSystem), true);
//        }
//        else
//        {
//            MuzzleMesh = (GameObject)EditorGUILayout.ObjectField("Muzzle Mesh", MuzzleMesh, typeof(GameObject), true);
//        }

//        AimedPosition = EditorGUILayout.Vector3Field("Aimed Position", AimedPosition);

//        AimedRotation = EditorGUILayout.Vector3Field("Aimed Rotation", AimedRotation);

//        HipFirePosition = EditorGUILayout.Vector3Field("Hip Fire Position", HipFirePosition);

//        HipFireRotation = EditorGUILayout.Vector3Field("Hip Fire Rotation", HipFireRotation);


//        IsThisDefaultWeapon = EditorGUILayout.Toggle("Is This Default Weapon", IsThisDefaultWeapon);

//        BulletShellSpawnPoint = (GameObject)EditorGUILayout.ObjectField("Bullet Shell Spawn Point", BulletShellSpawnPoint, typeof(GameObject), true);
//        GUILayout.Space(10);

//        weaponID = EditorGUILayout.IntField("Weapon ID", weaponID);
//        weaponName = EditorGUILayout.TextField("Weapon Name", weaponName);

//        // Add more fields as per your requirement

//        GUILayout.Space(10);

//        if (GUILayout.Button("Configure the weapon"))
//        {
//            SetupWeaponController();
//        }
//        if (GUILayout.Button("Edit the animations"))
//        {
//            EditorApplication.ExecuteMenuItem("Tools/MobileActionKit/Player/Weapon/Edit Weapon Animations");
//        }
//    }
//    private void DestroyChildrenWithoutGunscript(Transform parent)
//    {
//        // Iterate through all children of the current parent
//        for (int i = parent.childCount - 1; i >= 0; i--)
//        {
//            Transform childTransform = parent.GetChild(i);
//            // If the child does not have a Gunscript component, destroy it
//            DestroyImmediate(childTransform.gameObject);
//        }
//    }
//    private void ChangeParent(Transform parent)
//    {
//        // Iterate through all children of the current parent
//        for (int i = parent.childCount - 1; i >= 0; i--)
//        {
//            Transform childTransform = parent.GetChild(i);

//            childTransform.transform.parent = duplicatedObject.transform.transform.GetChild(0).transform.GetChild(0).transform;
//        }
//    }
//    private void SetupWeaponController()
//    {      
//        RootObjName = WeaponModel.name;

//        WeaponModel.gameObject.layer = LayerMask.NameToLayer("Weapon");
//        WeaponShootingPoint.gameObject.layer = LayerMask.NameToLayer("Weapon");

//        WeaponToCopyFrom.gameObject.SetActive(true);
//        duplicatedObject = Instantiate(WeaponToCopyFrom);

//        //duplicatedObject = Instantiate(WeaponToCopyFrom, WeaponModel.transform.position, WeaponModel.transform.rotation);
//        duplicatedObject.transform.name = RootObjName;

//        Vector3 StoredMetarigPosition = WeaponMetarig.transform.localPosition;
//        Vector3 StoredMetarigLocalEulerAngles = WeaponMetarig.transform.localEulerAngles;
//        Vector3 StoredMetarigLocalScale = WeaponMetarig.transform.localScale;

//        Vector3 StoredShootingPointPosition = WeaponShootingPoint.transform.localPosition;
//        Vector3 StoredShootingPointLocalEulerAngles = WeaponShootingPoint.transform.localEulerAngles;

//        Vector3 StoredBulletShellPointPosition = BulletShellSpawnPoint.transform.localPosition;
//        Vector3 StoredBulletShellPointLocalEulerAngles = BulletShellSpawnPoint.transform.localEulerAngles;

//        Transform ExtractedGunscriptFromPrefab = duplicatedObject.transform.GetChild(0).gameObject.GetComponent<PickAndDropWeapon>().ShootingScript.transform;

//        ExtractedGunscriptFromPrefab.transform.parent = null;

//        duplicatedObject.transform.transform.GetChild(0).transform.name = RootObjName + " " + "Parent";

//        duplicatedObject.transform.transform.GetChild(0).transform.GetChild(0).transform.name = RootObjName + " " + "Animator Controller";

//        DestroyChildrenWithoutGunscript(duplicatedObject.transform.GetChild(0).transform.GetChild(0).transform);

//        ChangeParent(WeaponModel.transform);

//        duplicatedObject.transform.parent = WeaponModel.transform.parent;

//        duplicatedObject.transform.localPosition = Vector3.zero;

//        duplicatedObject.transform.localEulerAngles = Vector3.zero;

//        duplicatedObject.transform.transform.GetChild(0).localPosition = Vector3.zero;

//        duplicatedObject.transform.transform.GetChild(0).localEulerAngles = Vector3.zero;

//        duplicatedObject.transform.transform.GetChild(0).transform.GetChild(0).transform.localPosition = WeaponModel.transform.localPosition;

//        duplicatedObject.transform.transform.GetChild(0).transform.GetChild(0).transform.localEulerAngles = WeaponModel.transform.localEulerAngles;

//        GameObject WeaponShootingPointParent = WeaponShootingPoint.transform.parent.gameObject;
//        PickAndDropWeapon FirstChild = duplicatedObject.transform.GetChild(0).gameObject.GetComponent<PickAndDropWeapon>();

//        if (FirstChild.ShootingScript.GetComponent<PlayerWeapon>() != null)
//        {
//            FirstChild.ShootingScript.transform.parent = WeaponShootingPointParent.transform;
//            FirstChild.ShootingScript.transform.localPosition = StoredShootingPointPosition;
//            FirstChild.ShootingScript.transform.localEulerAngles = StoredShootingPointLocalEulerAngles;
//        }
//        if(UseMuzzleMesh == true)
//        {
//            if(FirstChild.ShootingScript.GetComponent<PlayerWeapon>() != null)
//            {
//                Vector3 StoredMuzzleMeshLocalPosition = MuzzleMesh.transform.localPosition;
//                Vector3 StoredMuzzleMeshLocaleulerangles = MuzzleMesh.transform.localEulerAngles;

//                MuzzleMesh.gameObject.layer = LayerMask.NameToLayer("Weapon");
//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().transform.name = WeaponShootingPoint.name;
//                MuzzleMesh.transform.parent = FirstChild.ShootingScript.transform;
//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().ShootingFeatures.UseMuzzleFlashMesh = true;
//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashMesh = MuzzleMesh;
//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashMesh.transform.parent = FirstChild.ShootingScript.GetComponent<PlayerWeapon>().transform;

//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashMesh.transform.localPosition = StoredMuzzleMeshLocalPosition;
//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashMesh.transform.localEulerAngles = StoredMuzzleMeshLocaleulerangles;
//            }
//        }
//        else
//        {
//            if (FirstChild.ShootingScript.GetComponent<PlayerWeapon>() != null)
//            {
//                Vector3 StoredMuzzleFlashLocalPosition = MuzzleFlashParticles.transform.localPosition;
//                Vector3 StoredMuzzleFlashLocaleulerangles = MuzzleFlashParticles.transform.localEulerAngles;

//                MuzzleFlashParticles.gameObject.layer = LayerMask.NameToLayer("Weapon");
//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().transform.name = WeaponShootingPoint.name;
//                MuzzleMesh.transform.parent = FirstChild.ShootingScript.transform;
//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().ShootingFeatures.UseMuzzleFlashMesh = false;
//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashParticleFX = MuzzleFlashParticles;
//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashParticleFX.transform.parent = FirstChild.ShootingScript.GetComponent<PlayerWeapon>().transform;

//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashParticleFX.transform.localPosition = StoredMuzzleFlashLocalPosition;
//                FirstChild.ShootingScript.GetComponent<PlayerWeapon>().ShootingFeatures.MuzzleFlashParticleFX.transform.localEulerAngles = StoredMuzzleFlashLocaleulerangles;
//            }
//        }

//        if (FirstChild.ShootingScript.GetComponent<PlayerWeapon>() != null)
//        {
//            FirstChild.ShootingScript.GetComponent<PlayerWeapon>().WeaponPositionsRotationsDurations.AimedWeaponPivotPosition = AimedPosition;
//            FirstChild.ShootingScript.GetComponent<PlayerWeapon>().WeaponPositionsRotationsDurations.AimedWeaponPivotRotation = AimedRotation;
//            FirstChild.ShootingScript.GetComponent<PlayerWeapon>().WeaponPositionsRotationsDurations.HipFireWeaponPivotPosition = HipFirePosition;
//            FirstChild.ShootingScript.GetComponent<PlayerWeapon>().WeaponPositionsRotationsDurations.HipFireWeaponPivotRotation = HipFireRotation;

//            FirstChild.ShootingScript.GetComponent<PlayerWeapon>().BulletShell.BulletShellSpawnPoint = BulletShellSpawnPoint;
//            FirstChild.ShootingScript.GetComponent<PlayerWeapon>().BulletShell.BulletShellSpawnPoint.transform.localPosition = StoredBulletShellPointPosition;
//            FirstChild.ShootingScript.GetComponent<PlayerWeapon>().BulletShell.BulletShellSpawnPoint.transform.localPosition = StoredBulletShellPointLocalEulerAngles;
//        }

//        DestroyImmediate(WeaponShootingPoint.gameObject);

//        DestroyImmediate(WeaponModel.gameObject);

//        if(duplicatedObject.transform.transform.GetChild(0).GetComponent<WeaponId>() != null)
//        {
//            //duplicatedObject.transform.transform.GetChild(0).GetComponent<WeaponId>().Weaponid = weaponID;
//            duplicatedObject.transform.transform.GetChild(0).GetComponent<WeaponId>().WeaponName = weaponName;
//        }


//        // FirstChild.transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController =  WeaponModel.runtimeAnimatorController;
//        FirstChild.transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController = duplicatedObject.transform.transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController; 
//        FirstChild.transform.GetChild(0).GetComponent<Animator>().avatar = ModelAvatar;

//        WeaponMetarig.transform.localPosition = StoredMetarigPosition;
//        WeaponMetarig.transform.localEulerAngles = StoredMetarigLocalEulerAngles;
//        WeaponMetarig.transform.localScale = StoredMetarigLocalScale;

//        Selection.activeGameObject = duplicatedObject;
//        WeaponToCopyFrom.gameObject.SetActive(false);
//        //if (WeaponModel != null)
//        //{
//        //    DestroyImmediate(FirstChild);
//        //    DestroyImmediate(SecondChild);
//        //    DestroyImmediate(HumanoidModel);

//        //}
//    }
//}