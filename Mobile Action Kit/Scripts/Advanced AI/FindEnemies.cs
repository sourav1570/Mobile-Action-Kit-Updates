using System.Collections.Generic;
using UnityEngine;

// This Script is Responsible to Find Enemies Closest To Current Ai 

namespace MobileActionKit
{
    public class FindEnemies : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This component is necessary for the core Ai behaviour script in order to achieve target selection functionality. " +
            "This script stores all detected enemies and selects nearest enemy by comparing distances from enemy detector gameObject to all detected enemies. " +
            "There are two ways of detecting enemies." +
            "1.Using detection radius i.e automatically detecting all Ai agents and discriminating enemies from friendlies." +
            "This method is used as a simplest detection solution and can be used in situations where more complex solutions are not required. " +
            "2.Using field of view within detection radius size to imitate human vision. " +
            "This is more computationally complex solution and used in cases where first solution would not suffice developers due to its primitive nature that is more suitable for arcade gameplay. ";

        [Space(10)]

        //[Tooltip("Drag and drop empty gameobject named enemy detector into this field. Make sure enemy detector is the child of the root gameobject of the Ai agent.")]
        //public Transform EnemyDetector;

        [Tooltip("Radius within which AI agent can detect its enemies. AI agent will sort out closest enemy in any direction within this radius regardless of obstacles in-between" +
            " like buildings, vehicles, terrain, vegetation etc. If 'Enable Field Of View' checkbox is checked then AI agent will not act upon detected enemies within this radius" +
            " unless they get within his unobstructed field of view.")]
        public float DetectionRadius = 300f;
        [Tooltip("Minimum time between closest enemy visibility checks.")]
        public float MinDetectionInterval = 0.2f;
        [Tooltip("Maximum time between closest enemy visibility checks.")]
        public float MaxDetectionInterval = 0.4f;

        [Tooltip("This checkbox is conditioning successful detection to a specified angle within detection radius and absence of visual obstacles between AI agent and its enemy.")]
        public bool EnableFieldOfView = false;

        [Tooltip("Drag and drop Visual_FieldOfView(just FieldOfView) game object from Ai agent's head bone hierarchy into this field.")]
        public Transform FieldOfViewGameObject;
        [Tooltip("Field of view angle within which Ai agent will see it's targets.")]
        public float FieldOfViewValue = 120f;

        [Tooltip("Attach the 'Enemy Raycaster' GameObject as a child of this AI agent in the hierarchy and assign it to this field. " +
            "Position the Enemy Raycaster further out from the head bone to ensure proper  raycasting for the AI agent towards the closest enemy, " +
            "allowing it to determine visibility and to abandon current closest enemy if he is he is obscured(not in direct line of sight) and switch to another closest unobscured enemy.")]
        public Transform HidToUnhidEnemySwitcher;

        [Tooltip("If Checked then visibility raycast checks for detected enemies will be drawn in the scene view.")]
        public bool DebugEnemyVisibilityRaycastChecks = true;

        public Color EnemyVisibilityRaycastColor = Color.green; 

        [Tooltip("Name of the nearest visible of the detected enemies will be displayed inside this field during runtime.")]
        public string DebugNearestVisibleEnemyName;

        [Tooltip("If Checked then will debug the collider name and its root name hit by 'HidToUnhidEnemySwitcher' raycast.")]
        public bool DebugRaycastHitColliderNameAndRootName = true;

        [Tooltip("Display the name of raycast hit collider and its root name.")]
        public string DisplayRaycastHitColliderNameAndRootName = "";
           
        //[HideInInspector]
        public List<Transform> enemy = new List<Transform>();
        [HideInInspector]
        public Targets friendly;
        [HideInInspector]
        public string nametolookfor;
        [HideInInspector]
        public int CurrentEnemy = -1;
        float LastDist;
        [HideInInspector]
        public bool ContainThisTransform = false;
        bool removeThisTransformNow = false;
        [HideInInspector]
        public bool FindedEnemies;

        [HideInInspector]
        public float FOV;
        Vector3 dir;
        float Angle;
        bool EnemyLocked = false;
        [HideInInspector]
        public int LockedTarget;

        [HideInInspector]
        public bool IsSet = false;
        bool Check = false;
        int LastDisValue = int.MaxValue;
        int ThisDistanceValue;
        bool CanCheckNow = false;
        [HideInInspector]
        public bool NewEnemyLocked = false;
        Vector3 StoreCoordinates;

        [HideInInspector]
        public float OriginalFov;

        [HideInInspector]
        public bool IsEnemyAvailable = false;

        [HideInInspector]
        public bool IsEnemyRemoved = false;

        // Added VisibleEnemiesAtFirst list which is created on 6th August 2024 when doing tutorial. Basically when the combat starts we store all the enemies which are visible(by doing raycast)and which are
        // within the field of view of AI agent. after this when AI agent kill an enemy than in case other enemy is behind the cover or run away from his location but still alive than AI agent will try to locate and kill that AI agent as
        // he was visible when the combat begins. In case the AI agent who was behind the cover or run away from that location or got killed in that case AI agent update his list and do not move to kill the agent as he is already been killed.
        [HideInInspector]
        public List<Transform> VisibleEnemiesAtFirst = new List<Transform>();

        public void ResettingDescription()
        {
            ScriptInfo = "This component is necessary for the core Ai behaviour script in order to achieve target selection functionality. This script stores all detected enemies and selects nearest enemy by comparing distances from enemy detector gameobject to all detected enemies. " +
            "There are two ways of detecting enemies." +
            "1.Using detection radius i.e automatically detecting all Ai agents and discriminating enemies from friendlies.This method is used as a simpliest detection solution and can be used in situations where more complex solutions are not required. " +
            "2.Using field of view within detection radius size to imitate human vision. This is more computationally complex solution and used in cases where first solution would not suffice developers due to its primitive nature that is more suitable for arcade gameplay but insufficient for tactical behaviour." +
            "Combining these detection methods to optimise mobile performance will have noticeable affect. In case of PC target platform difference in perfomance will have less noticeable.";
        }
        private void Awake()
        {
            friendly = GetComponent<Targets>();
            nametolookfor = friendly.MyTeamID;
        }
        public void FindingEnemies() // Add All the Enemies in the List in The Start Of The Game Or Whenever a new Enemy is Spawned
        {
            // List<GameObject> go = new List<GameObject>(GameObject.FindGameObjectsWithTag("AI"));

            // Changed Completely on 14th Jan 2025 and why it is changed so it can find turret and player of anything which have Targets gameObject attached.
            Targets[] AllTargets = FindObjectsOfType<Targets>();

            for (int i = 0; i < AllTargets.Length; ++i)
            {
                if (AllTargets[i].GetComponent<Targets>() != null)
                {
                    if(AllTargets[i].GetComponent<Targets>().enabled == true)
                    {
                        if (AllTargets[i].GetComponent<Targets>().MyTeamID != nametolookfor)
                        {
                            if (!enemy.Contains(AllTargets[i].GetComponent<Targets>().MyBodyPartToTarget))
                            {
                                enemy.Add(AllTargets[i].GetComponent<Targets>().MyBodyPartToTarget);
                                CurrentEnemy = 0;
                                if (ContainThisTransform == true)
                                {
                                    removeThisTransformNow = true;
                                }
                            }
                        }
                    }                 
                }
            }
            
            // Till here 
        }
        public int FindClosestEnemy() // When Field of View is not Enabled we use this function to find the closest Enemy From us
        {
            if (enemy.Count == 0) return -1;
            int Closest = 0;
            if (enemy[LockedTarget] != null)
            {
                LastDist = Vector3.Distance(this.transform.position, enemy[LockedTarget].position);
            }

            for (int i = 0; i < enemy.Count; i++) // Previously int i = 1 on 03/01/2022
            {
                if (enemy[i] != null)
                {
                    float thisdis = Vector3.Distance(this.transform.position, enemy[i].position);
                    if (LastDist >= thisdis)
                    {
                        IsSet = true;
                        Closest = i;
                        LockedTarget = i;
                        LastDist = Vector3.Distance(this.transform.position, enemy[LockedTarget].position);
                    }
                }
            }

            //if (enemy[LockedTarget] == null)
            //{
            //    enemy.Remove(enemy[LockedTarget]);
            //    EnemyLocked = true;
            //}

            CheckforEnemy(false);
            return Closest;
        }
        public void AllVisibleEnemiesDuringCombat()
        {
            if (enemy.Count >= 1)
            {
                enemy.Sort((enemy1, enemy2) =>
                Vector3.Distance(transform.position, enemy1.transform.position)
                .CompareTo(Vector3.Distance(transform.position, enemy2.transform.position))
                );
 
                for (int i = 0; i < enemy.Count; i++)
                {
                    if (enemy[i] != null)
                    {
                        if (FieldOfViewGameObject != null)
                        {
                            dir = enemy[i].position - FieldOfViewGameObject.position;
                            Angle = Vector3.Angle(FieldOfViewGameObject.forward, dir);
                        }

                        if (Angle < FOV)
                        {
                            HidToUnhidEnemySwitcher.LookAt(enemy[i]);
                            Ray ray = new Ray(HidToUnhidEnemySwitcher.transform.position, HidToUnhidEnemySwitcher.forward);
                            if (DebugEnemyVisibilityRaycastChecks == true)
                            {
                                Debug.DrawLine(HidToUnhidEnemySwitcher.position, HidToUnhidEnemySwitcher.position + HidToUnhidEnemySwitcher.forward * dir.magnitude, EnemyVisibilityRaycastColor, 0.1f);
                            }
                            if (Physics.Raycast(ray, out RaycastHit hit, dir.magnitude))
                            {
                                if (hit.transform.root == enemy[i].transform.root)
                                {
                                    if (!VisibleEnemiesAtFirst.Contains(enemy[i]))
                                    {
                                        VisibleEnemiesAtFirst.Add(enemy[i]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        public int FindClosestEnemyInAngle()
        {
            if (enemy.Count == 0) return -1;
            int closest = -1; // Initialize to an invalid index
            float closestDistance = float.MaxValue; // Initialize to a large value
            float thisDistance = 0f;

            //if(enemy[LockedTarget] != null)
            //{
            //}

            if (IsEnemyRemoved == true)
            {
                Check = false;
                IsSet = false;
                IsEnemyAvailable = false;
                IsEnemyRemoved = false;
                CheckforEnemy(true);
            }

            enemy.Sort((enemy1, enemy2) =>
            Vector3.Distance(transform.position, enemy1.transform.position)
            .CompareTo(Vector3.Distance(transform.position, enemy2.transform.position))
              );

            bool shouldBreak = false;
            for (int i = 0; i < enemy.Count; i++)
            {
                if (enemy[i] != null && shouldBreak == false)
                {
                    if (FieldOfViewGameObject != null)
                    {
                        dir = enemy[i].position - FieldOfViewGameObject.position;
                        Angle = Vector3.Angle(FieldOfViewGameObject.forward, dir);
                        thisDistance = Vector3.Distance(FieldOfViewGameObject.position, enemy[i].position);
                    }

                    if (Angle < FOV)
                    {
                        if (thisDistance < closestDistance)
                        {
                            //if(IsSet == true && LockedTarget == i)
                            //{
                            //    if (DebugRaycastWithNearestEnemy == true)
                            //    {
                            //        Debug.Log("NO Raycast");
                            //    }
                            //    Check = false;
                            //    closest = i;
                            //    LockedTarget = i;
                            //    EnemyLocked = false;
                            //    IsSet = true;
                            //    closestDistance = thisDistance; // Update the closest distance
                            //    CheckforEnemy(false);
                            //    NewEnemyLocked = true;
                            //    IsEnemyAvailable = true;
                            //    break;
                            //}
                            //else
                            //{
                            if (DebugEnemyVisibilityRaycastChecks == true)
                            {
                                //Debug.Break();

                                //Debug.Log("YES Raycast");
                            }
                            HidToUnhidEnemySwitcher.LookAt(enemy[i]);
                            Ray ray = new Ray(HidToUnhidEnemySwitcher.transform.position, HidToUnhidEnemySwitcher.forward);
                            if (DebugEnemyVisibilityRaycastChecks == true)
                            {
                                Debug.DrawLine(HidToUnhidEnemySwitcher.position, HidToUnhidEnemySwitcher.position + HidToUnhidEnemySwitcher.forward * dir.magnitude, EnemyVisibilityRaycastColor, 0.1f);
                            }
                            if (Physics.Raycast(ray, out RaycastHit hit, dir.magnitude))
                            {
                                if (hit.transform.root == enemy[i].transform.root)
                                {
                                    // Debug.Log(hit.transform.root.transform.name);
                                    // Debug.Break();
                                    Check = false;
                                    closest = i;
                                    LockedTarget = i;
                                    EnemyLocked = false;
                                    IsSet = true;
                                    closestDistance = thisDistance; // Update the closest distance
                                    CheckforEnemy(false);
                                    NewEnemyLocked = true;
                                    IsEnemyAvailable = true;
                                    shouldBreak = true;
                                }

                                if (DebugRaycastHitColliderNameAndRootName == true)
                                {
                                    DisplayRaycastHitColliderNameAndRootName = hit.transform.name + " + " + hit.transform.root.transform.name;
                                }

                            }
                        }

                    }
                }
            }
            if(shouldBreak == false)
            {
                for (int i = 0; i < enemy.Count; i++)
                {
                    for (int x = 0; x < VisibleEnemiesAtFirst.Count; x++)
                    {
                        if (VisibleEnemiesAtFirst[x] == null)
                        {
                            VisibleEnemiesAtFirst.Remove(VisibleEnemiesAtFirst[x]);
                        }
                    }
                    if (enemy[i] != null && shouldBreak == false)
                    {
                        if (VisibleEnemiesAtFirst.Contains(enemy[i]))
                        {
                            Check = false;
                            closest = i;
                            LockedTarget = i;
                            EnemyLocked = false;
                            IsSet = true;
                            closestDistance = thisDistance;
                            CheckforEnemy(false);
                            NewEnemyLocked = true;
                            IsEnemyAvailable = true;
                            shouldBreak = true;
                        }
                    }
                }
            }
            if (closest != -1)
            {
                DebugNearestVisibleEnemyName = enemy[closest].transform.root.name;
            }
            else
            {
                closest = 0;
                DebugNearestVisibleEnemyName = "None";
            }

            if (EnemyLocked == false)
            {
                if (IsSet == true)
                {
                    closest = LockedTarget;
                    CanCheckNow = true;
                   // PreviousEnemy = enemy[closest].transform.root;
                }
            }
            return closest;
        }
        //public int FindClosestEnemyInAngle() // When Field of View is Enabled we use this function to find the closest Enemy From us
        //{
        //    if (enemy.Count == 0) return -1;
        //    int Closest = 0;

        //    if (IsEnemyRemoved() == true && CanCheckNow == true)
        //    {
        //        Check = false;
        //        IsSet = false;
        //        CheckforEnemy(true);
        //    }


        //    //if (enemy[LockedTarget] != null)
        //    //{
        //    //    LastDist = Vector3.Distance(this.transform.position, enemy[LockedTarget].position);
        //    //    LastDisValue = Mathf.FloorToInt(LastDist);
        //    //}

        //    //LastDist = Vector3.Distance(this.transform.position, StoreCoordinates);
        //    //LastDisValue = Mathf.FloorToInt(LastDist);


        //    for (int i = 0; i < enemy.Count; i++)
        //    {
        //        if (enemy[i] != null)
        //        {
        //            dir = enemy[i].position - this.transform.position;
        //            Angle = Vector3.Angle(this.transform.forward, dir);

        //            //Vector3 enemyPosition = enemy[i].position;
        //            //Vector3 localEnemyPosition = transform.InverseTransformPoint(enemyPosition);

        //            //Vector3 dir = localEnemyPosition - transform.localPosition;
        //            //Angle = Vector3.Angle(dir, transform.forward);


        //            float thisdis = Vector3.Distance(this.transform.position, enemy[i].position);
        //            ThisDistanceValue = Mathf.FloorToInt(thisdis);

        //            if (Angle < FOV)
        //            {
        //                EnemyDetector.LookAt(enemy[i].position);

        //                if (LastDisValue >= ThisDistanceValue)
        //                {
        //                    RaycastHit hitObj;

        //                    if (Physics.Raycast(EnemyDetector.position, EnemyDetector.forward, out hitObj, 1000))
        //                    {
        //                        if (hitObj.transform.root.transform == enemy[i].transform.root.transform)
        //                        {                               
        //                            Check = false;
        //                            Closest = i;
        //                            LockedTarget = i;
        //                            EnemyLocked = false;
        //                            IsSet = true;
        //                            LastDist = Vector3.Distance(this.transform.position, enemy[LockedTarget].position);
        //                            LastDisValue = Mathf.FloorToInt(LastDist);
        //                            CheckforEnemy(false);
        //                            NewEnemyLocked = true;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        //else
        //        //{
        //        //    enemy.Remove(enemy[i]);
        //        //}
        //    }
        //    DebugNearestEnemy = enemy[LockedTarget].transform.root.name;

        //    //if (enemy[LockedTarget] == null)
        //    //{
        //    //    enemy.Remove(enemy[LockedTarget]);
        //    //    EnemyLocked = true;
        //    //}

        //    if (EnemyLocked == false)
        //    {
        //        if (IsSet == true)
        //        {
        //            Closest = LockedTarget;
        //            CanCheckNow = true;
        //        }
        //    }
        //    return Closest;
        //}
        public void FindClosestEnemyNow(Transform trans) // We use This function to check for the enemies and choose functions according to the Ai settings 
        {
            EnemyLocked = false;

            for (int x = 0; x < enemy.Count; x++)
            {
                if (enemy[x] == null)
                {
                    enemy.Remove(enemy[x]);
                }
            }

            if (enemy.Count <= 0 || CurrentEnemy < 0)
            {
                if (!enemy.Contains(trans))
                {
                    enemy.Add(trans);
                }
                ContainThisTransform = true;
                CurrentEnemy = 0;
            }
            else
            {
                if (ContainThisTransform == true)
                {
                    if (removeThisTransformNow == true)
                    {
                        if (enemy.Contains(trans))
                        {
                            enemy.Remove(trans);
                        }
                        ContainThisTransform = false;
                        removeThisTransformNow = false;
                    }
                }
                else
                {
                    if (friendly.EnabledFOV == true)
                    {
                        CurrentEnemy = FindClosestEnemyInAngle();
                    }
                    else
                    {
                        CurrentEnemy = FindClosestEnemy();
                    }

                }
            }

            FindedEnemies = true;

        }
        public void CheckforEnemy(bool NoenemyinView) // Check if the enemy is in our view 
        {
            if (Check == false)
            {
                gameObject.SendMessage("EnemyView", NoenemyinView, SendMessageOptions.DontRequireReceiver);
                Check = true;
            }
        }       
    }
}