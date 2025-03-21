using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class AddSniperScopeWizard : EditorWindow
    {
        // Fields

        public GameObject Weapon;
        public PlayerWeapon PlayerWeaponScript;

        public bool CreateNewSniperScopeUIInCanvas = true;
        public Canvas Canvas2D;
        public GameObject SniperScopeSlider;
        public GameObject SniperScopePrefab;
        public GameObject SniperScopeSlidersPrefab;
        public bool AddReticleAdjusterToScope;

        private Vector2 scrollPosition; // For scroll view

        SniperScope SniperScopeScript;

        ReticleAdjusterScript ReticleAdjuster;

        [MenuItem("Tools/Mobile Action Kit/Player/FireArms/Add Sniper Scope", priority = 6)]
        public static void ShowWindow()
        {
            GetWindow<AddSniperScopeWizard>("Add Sniper Scope");
        }
        private void OnGUI()
        {
            // Set dynamic label width based on the window size (up to a max width of 250px)
            EditorGUIUtility.labelWidth = Mathf.Min(position.width * 0.3f, 250); // 30% of window width, capped at 250px

            // Begin Scroll View
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Draw Weapon Settings section
            EditorGUILayout.LabelField("Weapon Settings", EditorStyles.boldLabel);

            Weapon = (GameObject)EditorGUILayout.ObjectField("Weapon", Weapon, typeof(GameObject), true);
            PlayerWeaponScript = (PlayerWeapon)EditorGUILayout.ObjectField("Player Weapon Script", PlayerWeaponScript, typeof(PlayerWeapon), true);

            CreateNewSniperScopeUIInCanvas = EditorGUILayout.Toggle("Create New Sniper Scope UI In Canvas", CreateNewSniperScopeUIInCanvas);

            // Handle Sniper Scope UI settings with foldout
            if (CreateNewSniperScopeUIInCanvas)
            {
                EditorGUILayout.BeginVertical("box"); // Box around related fields
                Canvas2D = (Canvas)EditorGUILayout.ObjectField("Canvas 2D", Canvas2D, typeof(Canvas), true);
                SniperScopePrefab = (GameObject)EditorGUILayout.ObjectField("Sniper Scope Prefab", SniperScopePrefab, typeof(GameObject), false);
                SniperScopeSlidersPrefab = (GameObject)EditorGUILayout.ObjectField("Sniper Scope Sliders Prefab", SniperScopeSlidersPrefab, typeof(GameObject), false);
                EditorGUILayout.EndVertical(); // End box
            }
            else
            {
                SniperScopePrefab = (GameObject)EditorGUILayout.ObjectField("Sniper Scope Prefab", SniperScopePrefab, typeof(GameObject), false);
                SniperScopeSlider = (GameObject)EditorGUILayout.ObjectField("Sniper Scope Slider", SniperScopeSlider, typeof(GameObject), true);
            }

            // Toggle for Reticle Adjuster
            AddReticleAdjusterToScope = EditorGUILayout.Toggle("Add Reticle Adjuster To Scope", AddReticleAdjusterToScope);
           

            // Create Weapon Button
            if (GUILayout.Button("Add Sniper Scope To Weapon"))
            {
                AddSniperScopeToWeapon();
            }

            // End Scroll View
            EditorGUILayout.EndScrollView();

            // Reset label width back to default
            EditorGUIUtility.labelWidth = 0;
        }

        //private void OnGUI()
        //{
        //    // Set a dynamic label width based on the window size
        //    EditorGUIUtility.labelWidth = Mathf.Min(position.width * 0.4f, 200); // 40% of window width, capped at 200px

        //    // Begin Scroll View
        //    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        //    // Draw fields
        //    EditorGUILayout.LabelField("Weapon Settings", EditorStyles.boldLabel);

        //    Weapon = (GameObject)EditorGUILayout.ObjectField("Weapon", Weapon, typeof(GameObject), true);
        //    PlayerWeaponScript = (PlayerWeapon)EditorGUILayout.ObjectField("Player Weapon Script", PlayerWeaponScript, typeof(PlayerWeapon), true);

        //    CreateNewSniperScopeUIInCanvas = EditorGUILayout.Toggle("Create New Sniper Scope UI In Canvas", CreateNewSniperScopeUIInCanvas);

        //    if (CreateNewSniperScopeUIInCanvas == true)
        //    {
        //        Canvas2D = (Canvas)EditorGUILayout.ObjectField("Canvas 2D", Canvas2D, typeof(Canvas), true);
        //        SniperScopePrefab = (GameObject)EditorGUILayout.ObjectField("Sniper Scope Prefab", SniperScopePrefab, typeof(GameObject), false);
        //        SniperScopeSlidersPrefab = (GameObject)EditorGUILayout.ObjectField("Sniper Scope Sliders Prefab", SniperScopeSlidersPrefab, typeof(GameObject), false);
        //    }
        //    else
        //    {
        //        SniperScopePrefab = (GameObject)EditorGUILayout.ObjectField("Sniper Scope Prefab", SniperScopePrefab, typeof(GameObject), false);
        //        SniperScopeSlider = (GameObject)EditorGUILayout.ObjectField("Sniper Scope Slider", SniperScopeSlider, typeof(GameObject), true);
        //    }

        //    AddReticleAdjusterToScope = EditorGUILayout.Toggle("Add Reticle Adjuster To Scope", AddReticleAdjusterToScope);

        //    // Create Weapon Button
        //    if (GUILayout.Button("Add Sniper Scope To Weapon"))
        //    {
        //        AddSniperScopeToWeapon();
        //    }


        //    // End Scroll View
        //    EditorGUILayout.EndScrollView();

        //    // Reset label width back to default
        //    EditorGUIUtility.labelWidth = 0;
        //}
        private void AddSniperScopeToWeapon()
        {
            if (Weapon.GetComponent<SniperScope>() == null)
            {
                Weapon.AddComponent<SniperScope>();
                SniperScopeScript = Weapon.GetComponent<SniperScope>();
            }
            else
            {
                SniperScopeScript = Weapon.GetComponent<SniperScope>();
            }
            if (AddReticleAdjusterToScope == true)
            {
                if (Weapon.GetComponent<ReticleAdjusterScript>() == null)
                {
                    Weapon.AddComponent<ReticleAdjusterScript>();
                    ReticleAdjuster = Weapon.GetComponent<ReticleAdjusterScript>();
                }
                else
                {
                    ReticleAdjuster = Weapon.GetComponent<ReticleAdjusterScript>();
                }
            }
            CreateWeapon();
        }
        private void CreateWeapon()
        {
            GameObject spawnedscope = Instantiate(SniperScopePrefab, Weapon.transform);
            if (PrefabUtility.IsPartOfPrefabInstance(spawnedscope))
            {
                PrefabUtility.UnpackPrefabInstance(spawnedscope, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }

            spawnedscope.transform.localPosition = Vector3.zero;
            spawnedscope.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
            spawnedscope.transform.localScale = new Vector3(1f, 1f, 1f);

            if(CreateNewSniperScopeUIInCanvas == true)
            {
                SniperScopeSlider = Instantiate(SniperScopeSlidersPrefab, Canvas2D.transform);
                if (PrefabUtility.IsPartOfPrefabInstance(SniperScopeSlider))
                {
                    PrefabUtility.UnpackPrefabInstance(SniperScopeSlider.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
            }
          

            if (SniperScopeScript != null)
            {

                SniperScopeScript.ScopeConfig.PlayerWeaponScript = PlayerWeaponScript;
                SniperScopeScript.ScopeConfig.ScopeUIOverlay = spawnedscope;

                SniperScopeScript.ZoomConfig.EnableZoomControls = true;
                SniperScopeScript.ZoomConfig.UseZoomSlider = true;
                SniperScopeScript.ZoomConfig.ZoomSlider = SniperScopeSlider.transform.GetChild(2).GetComponent<Slider>();

                PlayerWeaponScript.ShootingFeatures.UseSniperScopeUI = true;
                PlayerWeaponScript.ShootingFeatures.SniperScopeScript = Weapon.GetComponent<SniperScope>();

                if (AddReticleAdjusterToScope == true)
                {
                    ReticleAdjuster.PlayerWeaponScript = PlayerWeaponScript;
                    ReticleAdjuster.ReticleConfig.Reticle = spawnedscope.transform.GetChild(1).transform;
                    ReticleAdjuster.ReticleConfig.XSliderSettings.Slider = SniperScopeSlider.transform.GetChild(1).GetComponent<Slider>();
                    ReticleAdjuster.ReticleConfig.YSliderSettings.Slider = SniperScopeSlider.transform.GetChild(0).GetComponent<Slider>();
                    ReticleAdjuster.SliderAdjusterSound = spawnedscope.transform.GetChild(0).GetComponent<AudioSource>();
                }

                if(CreateNewSniperScopeUIInCanvas == true && AddReticleAdjusterToScope == false)
                {
                    if(SniperScopeSlider.transform.GetChild(0) != null)
                    {
                        Object.DestroyImmediate(SniperScopeSlider.transform.GetChild(0));
                    }
                    if (SniperScopeSlider.transform.GetChild(1) != null)
                    {
                        Object.DestroyImmediate(SniperScopeSlider.transform.GetChild(1));
                    }

                   
                }
            }
  

            spawnedscope.transform.name = "Sniper Scope";
            SniperScopeSlider.transform.name = "Sniper Scope Slider";


            Debug.Log("Sniper Scope Added To Weapon.");
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