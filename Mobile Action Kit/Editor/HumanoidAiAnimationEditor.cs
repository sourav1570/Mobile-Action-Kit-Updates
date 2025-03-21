using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MobileActionKit
{
    public enum ChooseBetweenZombieAndSoliderOption
    {
        Soldier,
        Zombie
    }
    public class HumanoidAiAnimationEditor : EditorWindow
    {
        private GameObject targetObject;
        [Header("Combat Animations")]
        private AnimationClip BaseStandAimingAnimationClip;
        private AnimationClip StandFireAnimationClip;
        private AnimationClip StandAimingAnimationClip;
        private AnimationClip StandReloadAnimationClip;
        private AnimationClip StandIdleAnimationClip;

        private AnimationClip BaseCrouchAimingAnimationClip;
        private AnimationClip CrouchFireAnimationClip;
        private AnimationClip CrouchReloadAnimationClip;
        private AnimationClip CrouchAimingAnimationClip;
        private AnimationClip SprintingAnimationClip;
        private AnimationClip StandCoverLeftAnimationClip;
        private AnimationClip StandCoverRightAnimationClip;
        private AnimationClip StandCoverNeutralAnimationClip;

        private AnimationClip RunForwardAnimationClip;
        private AnimationClip RunRightAnimationClip;
        private AnimationClip RunLeftAnimationClip;
        private AnimationClip RunBackwardAnimationClip;
        private AnimationClip RunForwardRightAnimationClip;
        private AnimationClip RunBackwardRightAnimationClip;
        private AnimationClip RunBackwardLeftAnimationClip;
        private AnimationClip RunForwardLeftAnimationClip;

        private AnimationClip WalkForwardAimingAnimationClip;
        private AnimationClip WalkRightAnimationClip;
        private AnimationClip WalkLeftAnimationClip;
        private AnimationClip WalkBackwardAnimationClip;
        private AnimationClip WalkForwardRightAnimationClip;
        private AnimationClip WalkBackwardRightAnimationClip;
        private AnimationClip WalkBackwardLeftAnimationClip;
        private AnimationClip WalkForwardLeftAnimationClip;
        private AnimationClip GrenadeThrowAnimationClip;

        private AnimationClip MeleeAttack1AnimationClip;
        private AnimationClip MeleeAttack2AnimationClip;
        private AnimationClip MeleeAttack3AnimationClip;

        [Header("NonCombat Animations")]
        private AnimationClip WalkIdleAnimationClip;
        private AnimationClip UpperBodyIdleAnimationClip;
        private AnimationClip UpperBodyScanAnimationClip;
        private AnimationClip TurnLeftAnimationClip;
        private AnimationClip TurnRightAnimationClip;
        private AnimationClip TurnForwardAnimationClip;
        private AnimationClip TurnBackwardAnimationClip;
        private AnimationClip UpperBodyHitAnimationClip1;
        private AnimationClip UpperBodyHitAnimationClip2;
        private AnimationClip UpperBodyHitAnimationClip3;
        private AnimationClip LowerBodyHitAnimationClip1;
        private AnimationClip LowerBodyHitAnimationClip2;
        private AnimationClip LowerBodyHitAnimationClip3;

        private AnimationClip LowerBodyHaltLeftAnimationClip;
        private AnimationClip LowerBodyHaltRightAnimationClip;
        private AnimationClip UpperBodyHaltLeftAnimationClip;
        private AnimationClip UpperBodyHaltRightAnimationClip;

        private List<AnimationClip> DeathAnimationClips = new List<AnimationClip>();

        private Vector2 scrollPosition;

        private Texture2D customButtonImage;

        private ChooseBetweenZombieAndSoliderOption selectedCharacter = ChooseBetweenZombieAndSoliderOption.Soldier; // Default to Soldier
        Animator anim;

        private Dictionary<string, AnimationClip> duplicatedClips = new Dictionary<string, AnimationClip>();

        [MenuItem("Tools/Mobile Action Kit/Humanoid AI/Edit Humanoid AI Animations", priority = 1)]
        private static void Init()
        {
            HumanoidAiAnimationEditor window = (HumanoidAiAnimationEditor)EditorWindow.GetWindow(typeof(HumanoidAiAnimationEditor));
            window.titleContent = new GUIContent("Edit Humanoid AI Animations");
            window.Show();
        }
        private void OnGUI()
        {
            customButtonImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Mobile Action Kit/Art/EditorWindows_Art/btn.png");

            float windowWidth = position.width;
            float windowHeight = position.height;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            float labelWidth = windowWidth * 0.4f;
            float fieldWidth = windowWidth * 0.5f;
            float spaceHeight = windowHeight * 0.02f;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.wordWrap = true;

            GUILayout.Label("Target Object", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Humanoid Ai Animator Controller", labelStyle, GUILayout.Width(labelWidth));
            targetObject = (GameObject)EditorGUILayout.ObjectField(targetObject, typeof(GameObject), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();


            //GUILayout.Label("Custom Animator", EditorStyles.boldLabel);
            //GUILayout.BeginHorizontal();
            //GUILayout.Label("Animator Controller Name", labelStyle, GUILayout.Width(labelWidth));
            //AnimatorControllerName = EditorGUILayout.TextField(AnimatorControllerName, GUILayout.Width(fieldWidth));
            //GUILayout.EndHorizontal();

            //if (GUILayout.Button(new GUIContent("Create New Animator")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
            //{
            //    DuplicateAnimator();
            //}

            GUILayout.Label("Character Type", EditorStyles.boldLabel);
            selectedCharacter = (ChooseBetweenZombieAndSoliderOption)EditorGUILayout.EnumPopup(selectedCharacter);

            GUILayout.Space(spaceHeight);

            if (selectedCharacter == ChooseBetweenZombieAndSoliderOption.Soldier)
            {
                GUILayout.Label("COMBAT ANIMATION CLIPS", EditorStyles.boldLabel);

                GUILayout.Space(spaceHeight);

                GUILayout.BeginHorizontal();
                GUILayout.Label("BaseStandAimingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                BaseStandAimingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(BaseStandAimingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Rifle+Aiming+Idle");
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("StandFireAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                StandFireAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandFireAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Firing+Rifle");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("StandReloadAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                StandReloadAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandReloadAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Reloading");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("StandIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                StandIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=+Rifle+Idle");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("StandAimingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                StandAimingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandAimingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Rifle+Aiming+Idle");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("BaseCrouchAimingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                BaseCrouchAimingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(BaseCrouchAimingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Crouch+Idle");
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("CrouchFireAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                CrouchFireAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(CrouchFireAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Crouch+Rapid+Fire");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("CrouchReloadAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                CrouchReloadAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(CrouchReloadAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Reload+Crouch");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("CrouchAimingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                CrouchAimingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(CrouchAimingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Crouch+Idle");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("SprintingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                SprintingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(SprintingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Rifle+Run");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("StandCoverLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                StandCoverLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandCoverLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Taking+Cover+Idle");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("StandCoverRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                StandCoverRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandCoverRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Taking+Cover+Idle");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("StandCoverNeutralAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                StandCoverNeutralAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandCoverNeutralAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=+Rifle+Idle");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("RunForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                RunForwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunForwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+Forward");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("RunBackwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                RunBackwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunBackwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+backward");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("RunLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                RunLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+Strafe+left");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("RunRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                RunRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+Strafe+Right+");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("RunForwardRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                RunForwardRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunForwardRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=+jog+forward++right");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("RunForwardLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                RunForwardLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunForwardLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=+jog+forward+left");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("RunBackwardRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                RunBackwardRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunBackwardRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+backward+left");
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("RunBackwardLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                RunBackwardLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunBackwardLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+backward+left");
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("WalkForwardAimingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                WalkForwardAimingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkForwardAimingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero%2CSkinning+Test&page=1&query=Walking+");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("WalkRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                WalkRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Strafe+Right");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("WalkLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                WalkLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Strafe+Left");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("WalkBackwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                WalkBackwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkBackwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Walking+Backwards");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("WalkForwardRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                WalkForwardRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkForwardRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Walk+Forward+Right");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("WalkBackwardRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                WalkBackwardRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkBackwardRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Walk+Backward+Right");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("WalkBackwardLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                WalkBackwardLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkBackwardLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Walk+Backward+Left");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("WalkForwardLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                WalkForwardLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkForwardLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Walk+Forward+Left");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("GrenadeThrowAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                GrenadeThrowAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(GrenadeThrowAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Throw");
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("MeleeAttack1AnimationClip", labelStyle, GUILayout.Width(labelWidth));
                MeleeAttack1AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack1AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Bayonet+Stab");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("MeleeAttack2AnimationClip", labelStyle, GUILayout.Width(labelWidth));
                MeleeAttack2AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack2AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Punch");
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("MeleeAttack3AnimationClip", labelStyle, GUILayout.Width(labelWidth));
                MeleeAttack3AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack3AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn+And+Kick");
                GUILayout.EndHorizontal();

                GUILayout.Space(spaceHeight);

                GUILayout.Label("NONCOMBAT ANIMATION CLIPS", EditorStyles.boldLabel);

                GUILayout.Space(spaceHeight);

                GUILayout.BeginHorizontal();
                GUILayout.Label("WalkIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                WalkIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Rifle+Walk");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UpperBodyIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                UpperBodyIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.google.com");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UpperBodyScanAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                UpperBodyScanAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyScanAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.google.com");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("TurnLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                TurnLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("TurnRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                TurnRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("TurnForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                TurnForwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnForwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("TurnBackwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                TurnBackwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnBackwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UpperBodyHitAnimationClip1", labelStyle, GUILayout.Width(labelWidth));
                UpperBodyHitAnimationClip1 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip1, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UpperBodyHitAnimationClip2", labelStyle, GUILayout.Width(labelWidth));
                UpperBodyHitAnimationClip2 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip2, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UpperBodyHitAnimationClip3", labelStyle, GUILayout.Width(labelWidth));
                UpperBodyHitAnimationClip3 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip3, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LowerBodyHitAnimationClip1", labelStyle, GUILayout.Width(labelWidth));
                LowerBodyHitAnimationClip1 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip1, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LowerBodyHitAnimationClip2", labelStyle, GUILayout.Width(labelWidth));
                LowerBodyHitAnimationClip2 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip2, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LowerBodyHitAnimationClip3", labelStyle, GUILayout.Width(labelWidth));
                LowerBodyHitAnimationClip3 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip3, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LowerBodyHaltLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                LowerBodyHaltLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHaltLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=cover&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LowerBodyHaltRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                LowerBodyHaltRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHaltRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=cover&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UpperBodyHaltLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                UpperBodyHaltLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHaltLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=cover&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UpperBodyHaltRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                UpperBodyHaltRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHaltRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=cover&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.Space(spaceHeight);
                GUILayout.FlexibleSpace();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("StandIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                StandIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+idle");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("TurnLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                TurnLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Turn&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("TurnRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                TurnRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Turn&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("TurnForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                TurnForwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnForwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Turn&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("TurnBackwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                TurnBackwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnBackwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Turn&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("WalkForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                WalkForwardAimingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkForwardAimingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+walk&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("RunForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                RunForwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunForwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Run&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("SprintingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
                SprintingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(SprintingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Run&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("MeleeAttack1AnimationClip", labelStyle, GUILayout.Width(labelWidth));
                MeleeAttack1AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack1AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Attack&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("MeleeAttack2AnimationClip", labelStyle, GUILayout.Width(labelWidth));
                MeleeAttack2AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack2AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Attack&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("MeleeAttack3AnimationClip", labelStyle, GUILayout.Width(labelWidth));
                MeleeAttack3AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack3AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Bite&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UpperBodyHitAnimationClip1", labelStyle, GUILayout.Width(labelWidth));
                UpperBodyHitAnimationClip1 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip1, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Hit+&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UpperBodyHitAnimationClip2", labelStyle, GUILayout.Width(labelWidth));
                UpperBodyHitAnimationClip2 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip2, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Hit+&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UpperBodyHitAnimationClip3", labelStyle, GUILayout.Width(labelWidth));
                UpperBodyHitAnimationClip3 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip3, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Hit+&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LowerBodyHitAnimationClip1", labelStyle, GUILayout.Width(labelWidth));
                LowerBodyHitAnimationClip1 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip1, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Hit+&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LowerBodyHitAnimationClip2", labelStyle, GUILayout.Width(labelWidth));
                LowerBodyHitAnimationClip2 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip2, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Hit+&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("LowerBodyHitAnimationClip3", labelStyle, GUILayout.Width(labelWidth));
                LowerBodyHitAnimationClip3 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip3, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Zombie+Hit+&type=Motion%2CMotionPack");
                GUILayout.EndHorizontal();

            }



            GUILayout.Label("Drag and Drop Death Animation Clips Here:", EditorStyles.boldLabel);

            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50f);
            GUI.Box(dropArea, "Drop Animation Clips Here");

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        DeathAnimationClips.AddRange(GetAnimationClipsFromDraggedObjects());
                        Repaint();
                    }

                    Event.current.Use();
                    break;
            }

            if (DeathAnimationClips.Count > 0)
            {
                GUILayout.Label("Dragged Animation Clips:");
                foreach (AnimationClip clip in DeathAnimationClips)
                {
                    EditorGUILayout.LabelField(clip.name);
                }

                GUILayout.Space(10);
            }

            if (GUILayout.Button(new GUIContent("Add Death Animation clips To Animator")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
            {
                if (targetObject != null)
                {
                    anim = targetObject.GetComponent<Animator>();
                }

                if (anim != null && DeathAnimationClips.Count > 0)
                {
                    if (selectedCharacter == ChooseBetweenZombieAndSoliderOption.Soldier)
                    {
                        AssignAnimationClipsToLayer(anim, 5, DeathAnimationClips.ToArray());
                    }
                    else
                    {
                        AssignAnimationClipsToLayer(anim, 3, DeathAnimationClips.ToArray());
                    }

                    DeathAnimationClips.Clear();
                    Repaint();
                }
                else
                {
                    Debug.LogWarning("Please select an Animator and drag Animation Clips before assigning.");
                }
            }

            if (GUILayout.Button("Remove Last Dragged Death Animation Clip"))//, buttonStyle))
            {
                RemoveLastUnassignedAnimationClip();
            }
            if (GUILayout.Button(new GUIContent("Duplicate Animation Clip and Modify Import Settings")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
            {
                if (selectedCharacter == ChooseBetweenZombieAndSoliderOption.Soldier)
                {
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref BaseStandAimingAnimationClip, "BaseStandAimingAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref StandFireAnimationClip, "StandFireAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref StandIdleAnimationClip, "StandIdleAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref StandAimingAnimationClip, "StandAimingAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref BaseCrouchAimingAnimationClip, "BaseCrouchAimingAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref CrouchFireAnimationClip, "CrouchFireAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref CrouchAimingAnimationClip, "CrouchAimingAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref SprintingAnimationClip, "SprintingAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref StandCoverLeftAnimationClip, "StandCoverLeftAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref StandCoverRightAnimationClip, "StandCoverRightAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref StandCoverNeutralAnimationClip, "StandCoverNeutralAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref RunForwardAnimationClip, "RunForwardAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref RunBackwardAnimationClip, "RunBackwardAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref RunLeftAnimationClip, "RunLeftAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref RunRightAnimationClip, "RunRightAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref RunForwardRightAnimationClip, "RunForwardRightAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref RunForwardLeftAnimationClip, "RunForwardLeftAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref RunBackwardRightAnimationClip, "RunBackwardRightAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref RunBackwardLeftAnimationClip, "RunBackwardLeftAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref WalkForwardAimingAnimationClip, "WalkForwardAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref WalkRightAnimationClip, "WalkRightAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref WalkLeftAnimationClip, "WalkLeftAnimationClip");

                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref WalkBackwardRightAnimationClip, "WalkBackwardRightAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref WalkForwardRightAnimationClip, "WalkForwardRightAnimationClip");


                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref WalkBackwardAnimationClip, "WalkBackwardAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref WalkBackwardLeftAnimationClip, "WalkBackwardLeftAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref WalkForwardLeftAnimationClip, "WalkForwardLeftAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref WalkIdleAnimationClip, "WalkIdleAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref UpperBodyIdleAnimationClip, "UpperBodyIdleAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref UpperBodyScanAnimationClip, "UpperBodyScanAnimationClip");

                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref LowerBodyHaltLeftAnimationClip, "LowerBodyHaltLeftAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref LowerBodyHaltRightAnimationClip, "LowerBodyHaltRightAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref UpperBodyHaltLeftAnimationClip, "UpperBodyHaltLeftAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref UpperBodyHaltRightAnimationClip, "UpperBodyHaltRightAnimationClip");


                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref MeleeAttack1AnimationClip, "MeleeAttack1AnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref MeleeAttack2AnimationClip, "MeleeAttack2AnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref MeleeAttack3AnimationClip, "MeleeAttack3AnimationClip");

                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(ref TurnLeftAnimationClip, "TurnLeftAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(ref TurnRightAnimationClip, "TurnRightAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(ref TurnForwardAnimationClip, "TurnForwardAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(ref TurnBackwardAnimationClip, "TurnBackwardAnimationClip");

                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref UpperBodyHitAnimationClip1, "UpperBodyHitAnimationClip1");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref UpperBodyHitAnimationClip2, "UpperBodyHitAnimationClip2");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref UpperBodyHitAnimationClip3, "UpperBodyHitAnimationClip3");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref LowerBodyHitAnimationClip1, "LowerBodyHitAnimationClip1");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref LowerBodyHitAnimationClip2, "LowerBodyHitAnimationClip2");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref LowerBodyHitAnimationClip3, "LowerBodyHitAnimationClip3");


                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref StandReloadAnimationClip, "StandReloadAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref GrenadeThrowAnimationClip, "GrenadeThrowAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref CrouchReloadAnimationClip, "CrouchReloadAnimationClip");
                }
                else
                {
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref StandIdleAnimationClip, "StandIdleAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref RunForwardAnimationClip, "RunForwardAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref WalkForwardAimingAnimationClip, "WalkForwardAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref SprintingAnimationClip, "SprintingAnimationClip");

                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref MeleeAttack1AnimationClip, "MeleeAttack1AnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref MeleeAttack2AnimationClip, "MeleeAttack2AnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref MeleeAttack3AnimationClip, "MeleeAttack3AnimationClip");

                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(ref TurnLeftAnimationClip, "TurnLeftAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(ref TurnRightAnimationClip, "TurnRightAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(ref TurnForwardAnimationClip, "TurnForwardAnimationClip");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(ref TurnBackwardAnimationClip, "TurnBackwardAnimationClip");

                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref UpperBodyHitAnimationClip1, "UpperBodyHitAnimationClip1");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref UpperBodyHitAnimationClip2, "UpperBodyHitAnimationClip2");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref UpperBodyHitAnimationClip3, "UpperBodyHitAnimationClip3");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref LowerBodyHitAnimationClip1, "LowerBodyHitAnimationClip1");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref LowerBodyHitAnimationClip2, "LowerBodyHitAnimationClip2");
                    AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref LowerBodyHitAnimationClip3, "LowerBodyHitAnimationClip3");

                }

                if (anim != null && DeathAnimationClips.Count > 0)
                {
                    for (int x = 0; x < DeathAnimationClips.Count; x++)
                    {
                        AnimationClip originalClip = DeathAnimationClips[x];
                        AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref originalClip, "DeathAnimationClips" + x.ToString());
                    }
                }

            }

            if (GUILayout.Button(new GUIContent("Modify Animation Clip Import Settings")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
            {

                if (selectedCharacter == ChooseBetweenZombieAndSoliderOption.Soldier)
                {
                    ModifyAnimationSettingsWithFullChanges(BaseStandAimingAnimationClip, "BaseStandAimingAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(StandFireAnimationClip, "StandFireAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(StandIdleAnimationClip, "StandIdleAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(StandAimingAnimationClip, "StandAimingAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(BaseCrouchAimingAnimationClip, "BaseCrouchAimingAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(CrouchFireAnimationClip, "CrouchFireAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(CrouchAimingAnimationClip, "CrouchAimingAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(SprintingAnimationClip, "SprintingAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(StandCoverLeftAnimationClip, "StandCoverLeftAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(StandCoverRightAnimationClip, "StandCoverRightAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(StandCoverNeutralAnimationClip, "StandCoverNeutralAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(RunForwardAnimationClip, "RunForwardAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(RunBackwardAnimationClip, "RunBackwardAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(RunLeftAnimationClip, "RunLeftAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(RunRightAnimationClip, "RunRightAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(RunForwardRightAnimationClip, "RunForwardRightAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(RunForwardLeftAnimationClip, "RunForwardLeftAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(RunBackwardRightAnimationClip, "RunBackwardRightAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(RunBackwardLeftAnimationClip, "RunBackwardLeftAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(WalkForwardAimingAnimationClip, "WalkForwardAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(WalkRightAnimationClip, "WalkRightAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(WalkLeftAnimationClip, "WalkLeftAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(WalkBackwardAnimationClip, "WalkBackwardAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(WalkBackwardLeftAnimationClip, "WalkBackwardLeftAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(WalkForwardLeftAnimationClip, "WalkForwardLeftAnimationClip");

                    ModifyAnimationSettingsWithFullChanges(WalkBackwardRightAnimationClip, "WalkBackwardRightAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(WalkForwardRightAnimationClip, "WalkForwardRightAnimationClip");

                    ModifyAnimationSettingsWithFullChanges(WalkIdleAnimationClip, "WalkIdleAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(UpperBodyIdleAnimationClip, "UpperBodyIdleAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(UpperBodyScanAnimationClip, "UpperBodyScanAnimationClip");

                    ModifyAnimationSettingsWithFullChanges(LowerBodyHaltLeftAnimationClip, "LowerBodyHaltLeftAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(LowerBodyHaltRightAnimationClip, "LowerBodyHaltRightAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(UpperBodyHaltLeftAnimationClip, "UpperBodyHaltLeftAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(UpperBodyHaltRightAnimationClip, "UpperBodyHaltRightAnimationClip");


                    ModifyAnimationSettingsWithNoLoop(MeleeAttack1AnimationClip, "MeleeAttack1AnimationClip");
                    ModifyAnimationSettingsWithNoLoop(MeleeAttack2AnimationClip, "MeleeAttack2AnimationClip");
                    ModifyAnimationSettingsWithNoLoop(MeleeAttack3AnimationClip, "MeleeAttack3AnimationClip");

                    ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnLeftAnimationClip, "TurnLeftAnimationClip");
                    ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnRightAnimationClip, "TurnRightAnimationClip");
                    ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnForwardAnimationClip, "TurnForwardAnimationClip");
                    ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnBackwardAnimationClip, "TurnBackwardAnimationClip");

                    ModifyAnimationSettingsWithNoLoop(UpperBodyHitAnimationClip1, "UpperBodyHitAnimationClip1");
                    ModifyAnimationSettingsWithNoLoop(UpperBodyHitAnimationClip2, "UpperBodyHitAnimationClip2");
                    ModifyAnimationSettingsWithNoLoop(UpperBodyHitAnimationClip3, "UpperBodyHitAnimationClip3");
                    ModifyAnimationSettingsWithNoLoop(LowerBodyHitAnimationClip1, "LowerBodyHitAnimationClip1");
                    ModifyAnimationSettingsWithNoLoop(LowerBodyHitAnimationClip2, "LowerBodyHitAnimationClip2");
                    ModifyAnimationSettingsWithNoLoop(LowerBodyHitAnimationClip3, "LowerBodyHitAnimationClip3");


                    ModifyAnimationSettingsWithNoLoop(StandReloadAnimationClip, "StandReloadAnimationClip");
                    ModifyAnimationSettingsWithNoLoop(GrenadeThrowAnimationClip, "GrenadeThrowAnimationClip");
                    ModifyAnimationSettingsWithNoLoop(CrouchReloadAnimationClip, "CrouchReloadAnimationClip");
                }
                else
                {
                    ModifyAnimationSettingsWithFullChanges(StandIdleAnimationClip, "StandIdleAnimationClip");

                    ModifyAnimationSettingsWithFullChanges(WalkForwardAimingAnimationClip, "WalkForwardAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(RunForwardAnimationClip, "RunForwardAnimationClip");
                    ModifyAnimationSettingsWithFullChanges(SprintingAnimationClip, "SprintingAnimationClip");

                    ModifyAnimationSettingsWithNoLoop(MeleeAttack1AnimationClip, "MeleeAttack1AnimationClip");
                    ModifyAnimationSettingsWithNoLoop(MeleeAttack2AnimationClip, "MeleeAttack2AnimationClip");
                    ModifyAnimationSettingsWithNoLoop(MeleeAttack3AnimationClip, "MeleeAttack3AnimationClip");

                    ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnLeftAnimationClip, "TurnLeftAnimationClip");
                    ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnRightAnimationClip, "TurnRightAnimationClip");
                    ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnForwardAnimationClip, "TurnForwardAnimationClip");
                    ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnBackwardAnimationClip, "TurnBackwardAnimationClip");

                    ModifyAnimationSettingsWithNoLoop(UpperBodyHitAnimationClip1, "UpperBodyHitAnimationClip1");
                    ModifyAnimationSettingsWithNoLoop(UpperBodyHitAnimationClip2, "UpperBodyHitAnimationClip2");
                    ModifyAnimationSettingsWithNoLoop(UpperBodyHitAnimationClip3, "UpperBodyHitAnimationClip3");
                    ModifyAnimationSettingsWithNoLoop(LowerBodyHitAnimationClip1, "LowerBodyHitAnimationClip1");
                    ModifyAnimationSettingsWithNoLoop(LowerBodyHitAnimationClip2, "LowerBodyHitAnimationClip2");
                    ModifyAnimationSettingsWithNoLoop(LowerBodyHitAnimationClip3, "LowerBodyHitAnimationClip3");

                }

                if (anim != null && DeathAnimationClips.Count > 0)
                {
                    for (int x = 0; x < DeathAnimationClips.Count; x++)
                    {
                        AnimationClip originalClip = DeathAnimationClips[x];
                        ModifyAnimationSettingsWithNoLoop(originalClip, "DeathAnimationClips" + x.ToString());
                    }
                }

            }

            if (GUILayout.Button(new GUIContent("Import Existing Animations From Animator")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
            {
                ExportAnimations();
            }
            if (GUILayout.Button(new GUIContent("Save New Animations To Animator")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
            {
                SaveAnimations();
                EditorWindow.GetWindow<HumanoidAiAnimationEditor>().Close();
            }
            if (GUILayout.Button(new GUIContent("Reset Animations Clips In this Window")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
            {
                ResetAnimations();
            }
            GUILayout.FlexibleSpace();

            GUILayout.EndScrollView();

        }
        //private void AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref AnimationClip originalClip, string FieldName)
        //{
        //    if (originalClip != null)
        //    {
        //        ModifyAnimationSettingsWithFullChanges(originalClip, FieldName);

        //        // Duplicate the original clip
        //        AnimationClip duplicatedClip = Instantiate(originalClip);

        //        // Set the name of the duplicated clip to the field name
        //        duplicatedClip.name = FieldName;

        //        // Create a new asset for the duplicated clip
        //        string assetPath = AssetDatabase.GetAssetPath(originalClip);
        //        // Get the folder path of the original clip's asset path
        //        string folderPath = System.IO.Path.GetDirectoryName(assetPath);

        //        // Create a new asset for the duplicated clip in the same folder
        //        string duplicatedClipAssetPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + FieldName + ".anim");

        //        AssetDatabase.CreateAsset(duplicatedClip, duplicatedClipAssetPath);
        //        AssetDatabase.SaveAssets();

        //        originalClip = duplicatedClip;

        //    }
        //}
        private void AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithFullChanges(ref AnimationClip originalClip, string FieldName)
        {
            // Check if the original animation clip has changed
            if (originalClip != null && duplicatedClips.ContainsKey(FieldName) && duplicatedClips[FieldName] != originalClip)
            {
                // Remove the entry for the old clip from the dictionary
                duplicatedClips.Remove(FieldName);
            }

            if (originalClip != null)
            {
                // Call the ModifyAnimationSettingsWithFullChanges method with the duplicated clip
                ModifyAnimationSettingsWithFullChanges(originalClip, FieldName);

                // Check if a duplicated clip for this field name already exists
                if (duplicatedClips.ContainsKey(FieldName))
                {
                    Debug.Log("Duplicated clip for " + FieldName + " has already been instantiated.");
                    return;
                }

                // Duplicate the original clip
                AnimationClip duplicatedClip = Instantiate(originalClip);

                // Set the name of the duplicated clip to the field name
                duplicatedClip.name = FieldName;

                // Get the folder path of the original clip's asset path
                string assetPath = AssetDatabase.GetAssetPath(originalClip);
                string folderPath = System.IO.Path.GetDirectoryName(assetPath);

                // Create a new asset for the duplicated clip in the same folder
                string duplicatedClipAssetPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + FieldName + ".anim");
                AssetDatabase.CreateAsset(duplicatedClip, duplicatedClipAssetPath);
                AssetDatabase.SaveAssets();

                // Assign the duplicated clip to the field
                originalClip = duplicatedClip;

                // Add the duplicated clip to the dictionary
                duplicatedClips.Add(FieldName, duplicatedClip);


            }
        }
        private void AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(ref AnimationClip originalClip, string FieldName)
        {
            // Check if the original animation clip has changed
            if (originalClip != null && duplicatedClips.ContainsKey(FieldName) && duplicatedClips[FieldName] != originalClip)
            {
                // Remove the entry for the old clip from the dictionary
                duplicatedClips.Remove(FieldName);
            }

            if (originalClip != null)
            {
                ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(originalClip, FieldName);

                // Check if a duplicated clip for this field name already exists
                if (duplicatedClips.ContainsKey(FieldName))
                {
                    Debug.Log("Duplicated clip for " + FieldName + " has already been instantiated.");
                    return;
                }

                // Duplicate the original clip
                AnimationClip duplicatedClip = Instantiate(originalClip);

                // Set the name of the duplicated clip to the field name
                duplicatedClip.name = FieldName;

                // Get the folder path of the original clip's asset path
                string assetPath = AssetDatabase.GetAssetPath(originalClip);
                string folderPath = System.IO.Path.GetDirectoryName(assetPath);

                // Create a new asset for the duplicated clip in the same folder
                string duplicatedClipAssetPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + FieldName + ".anim");
                AssetDatabase.CreateAsset(duplicatedClip, duplicatedClipAssetPath);
                AssetDatabase.SaveAssets();

                // Assign the duplicated clip to the field
                originalClip = duplicatedClip;

                // Add the duplicated clip to the dictionary
                duplicatedClips.Add(FieldName, duplicatedClip);
            }
        }
        private void AutoModifyAnimationSettingsWithDuplicateAndModifyAnimationSettingsWithNoLoop(ref AnimationClip originalClip, string FieldName)
        {
            // Check if the original animation clip has changed
            if (originalClip != null && duplicatedClips.ContainsKey(FieldName) && duplicatedClips[FieldName] != originalClip)
            {
                // Remove the entry for the old clip from the dictionary
                duplicatedClips.Remove(FieldName);
            }

            if (originalClip != null)
            {
                ModifyAnimationSettingsWithNoLoop(originalClip, FieldName);

                // Check if a duplicated clip for this field name already exists
                if (duplicatedClips.ContainsKey(FieldName))
                {
                    Debug.Log("Duplicated clip for " + FieldName + " has already been instantiated.");
                    return;
                }

                // Duplicate the original clip
                AnimationClip duplicatedClip = Instantiate(originalClip);

                // Set the name of the duplicated clip to the field name
                duplicatedClip.name = FieldName;

                // Get the folder path of the original clip's asset path
                string assetPath = AssetDatabase.GetAssetPath(originalClip);
                string folderPath = System.IO.Path.GetDirectoryName(assetPath);

                // Create a new asset for the duplicated clip in the same folder
                string duplicatedClipAssetPath = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + FieldName + ".anim");
                AssetDatabase.CreateAsset(duplicatedClip, duplicatedClipAssetPath);
                AssetDatabase.SaveAssets();

                // Assign the duplicated clip to the field
                originalClip = duplicatedClip;

                // Add the duplicated clip to the dictionary
                duplicatedClips.Add(FieldName, duplicatedClip);
            }
        }
        private AnimationClip[] GetAnimationClipsFromDraggedObjects()
        {
            Object[] draggedObjects = DragAndDrop.objectReferences;
            List<AnimationClip> clips = new List<AnimationClip>();

            foreach (Object obj in draggedObjects)
            {
                AnimationClip clip = obj as AnimationClip;
                if (clip != null)
                {
                    clips.Add(clip);
                }
            }

            return clips.ToArray();
        }
        private void OpenLinkButton(string link)
        {
            // Use custom button image for the button, without any text
            if (GUILayout.Button(customButtonImage, GUILayout.Width(21), GUILayout.Height(21))) // Adjust width and height as needed
            {
                Application.OpenURL(link);
            }
        }
        private void ModifyAnimationSettingsWithFullChanges(AnimationClip clip, string FieldName)
        {
            if (clip != null)
            {
                ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;

                if (modelImporter != null)
                {
                    // Set Humanoid settings
                    modelImporter.animationType = ModelImporterAnimationType.Human;
                    ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

                    foreach (var clipAnimation in clipAnimations)
                    {
                        clipAnimation.name = FieldName;
                        // Activate "Bake Into Pose"
                        clipAnimation.lockRootRotation = true;
                        clipAnimation.lockRootHeightY = true;

                        // Choose wrap mode based on original animation
                        clipAnimation.loop = clip.isLooping;

                        // Set Root Transform Rotation, Loop Pose, and Loop Time to Original
                        clipAnimation.loopTime = true;
                        clipAnimation.loopPose = true;

                        clipAnimation.keepOriginalOrientation = true;

                        // Set Based Upon to Original for Root Transform Rotation
                        clipAnimation.cycleOffset = 0f;
                    }

                    modelImporter.clipAnimations = clipAnimations;

                    modelImporter.SaveAndReimport();
                }
                else
                {
                    // Set wrap mode to loop if the original clip is looping
                    clip.wrapMode = clip.isLooping ? WrapMode.Loop : WrapMode.Once;

                    // Modify root transform settings directly on the clip
                    AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
                    clipSettings.loopTime = true;
                    clipSettings.loopBlend = true;

                    // Set Bake Into Pose equivalent
                    clipSettings.keepOriginalOrientation = true;
                    clipSettings.keepOriginalPositionY = true;
                   // clipSettings.keepOriginalPositionXZ = true;

                    // Set the root transform rotation and position
                    clipSettings.orientationOffsetY = 0f;
                    clipSettings.level = 0f;
                    clipSettings.cycleOffset = 0f;

                    // Apply the modified settings
                    AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

                    // Save the changes to the clip
                    EditorUtility.SetDirty(clip);
                    AssetDatabase.SaveAssets();
                } 
            }
        }
        private void ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(AnimationClip clip, string FieldName)
        {
            if (clip != null)
            {
                ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;

                if (modelImporter != null)
                {
                    // Set Humanoid settings
                    modelImporter.animationType = ModelImporterAnimationType.Human;
                    //modelImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                    ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

                    foreach (var clipAnimation in clipAnimations)
                    {
                        clipAnimation.name = FieldName;

                        clipAnimation.loop = clip.isLooping;
                        // Set Based Upon to Original for Root Transform Rotation
                        clipAnimation.cycleOffset = 0f;
                    }

                    modelImporter.clipAnimations = clipAnimations;

                    modelImporter.SaveAndReimport();
                }
                else
                {
                    // Set wrap mode to loop if the original clip is looping
                    clip.wrapMode = clip.isLooping ? WrapMode.Loop : WrapMode.Once;

                    // Modify root transform settings directly on the clip
                    AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
    
                    clipSettings.keepOriginalPositionY = true;
                  
                    clipSettings.cycleOffset = 0f;

                    // Apply the modified settings
                    AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

                    // Save the changes to the clip
                    EditorUtility.SetDirty(clip);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        private void ModifyAnimationSettingsWithNoLoop(AnimationClip clip, string FieldName)
        {
            if (clip != null)
            {
                ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;

                if (modelImporter != null)
                {
                    // Set Humanoid settings 
                    modelImporter.animationType = ModelImporterAnimationType.Human;
                    //modelImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

                    ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

                    foreach (var clipAnimation in clipAnimations)
                    {
                        clipAnimation.name = FieldName;
                        // Activate "Bake Into Pose"
                        clipAnimation.lockRootRotation = true;
                        clipAnimation.lockRootHeightY = true;

                        // Choose wrap mode based on original animation
                        clipAnimation.loop = clip.isLooping;

                        // Set Based Upon to Original for Root Transform Rotation
                        clipAnimation.cycleOffset = 0f;
                    }

                    modelImporter.clipAnimations = clipAnimations;

                    modelImporter.SaveAndReimport();

                }
                else
                {
                    // Set wrap mode to loop if the original clip is looping
                    clip.wrapMode = clip.isLooping ? WrapMode.Loop : WrapMode.Once;

                    // Modify root transform settings directly on the clip
                    AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);

                    // Set Bake Into Pose equivalent
                    clipSettings.keepOriginalOrientation = true;
                    clipSettings.keepOriginalPositionY = true;

                    // Set the root transform rotation and position
                    clipSettings.orientationOffsetY = 0f;
                    clipSettings.level = 0f;
                    clipSettings.cycleOffset = 0f;

                    // Apply the modified settings
                    AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

                    // Save the changes to the clip
                    EditorUtility.SetDirty(clip);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        private void AssignAnimationClipsToLayer(Animator animator, int layerIndex, AnimationClip[] clips)
        {
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller != null)
            {
                AnimatorControllerLayer[] layers = controller.layers;

                if (layerIndex >= 0 && layerIndex < layers.Length)
                {
                    AnimatorStateMachine stateMachine = layers[layerIndex].stateMachine;

                    foreach (AnimationClip clip in clips)
                    {
                        AnimatorState state = stateMachine.AddState(clip.name);
                        state.motion = clip;
                    }

                    Debug.Log("Animation clips assigned successfully.");
                }
                else
                {
                    Debug.LogWarning("Invalid layer index. Please enter a valid layer index.");
                }
            }
            else
            {
                Debug.LogWarning("Selected Animator does not have a valid Animator Controller.");
            }
        }
        private void RemoveLastUnassignedAnimationClip()
        {
            if (DeathAnimationClips.Count > 0)
            {
                AnimationClip lastUnassignedClip = DeathAnimationClips[DeathAnimationClips.Count - 1];
                DeathAnimationClips.RemoveAt(DeathAnimationClips.Count - 1);
                Debug.Log("Last unassigned animation clip removed successfully.");
            }
            else
            {
                Debug.LogWarning("No unassigned animation clips to remove.");
            }
        }
        private void ExportAnimations()
        {
            if (targetObject == null)
            {
                Debug.LogError("Target Object is not assigned.");
                return;
            }

            Animator animator = targetObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Target Object does not have an Animator component.");
                return;
            }

            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
            if (animatorController == null)
            {
                Debug.LogError("Target Object's Animator does not have an AnimatorController assigned.");
                return;
            }

            if (selectedCharacter == ChooseBetweenZombieAndSoliderOption.Soldier)
            {
                BaseStandAimingAnimationClip = GetAnimationClipByKeyword(animatorController, "BaseStandAimingAnimationClip");

                StandFireAnimationClip = GetAnimationClipByKeyword(animatorController, "Stand Weapon Recoil");
                StandAimingAnimationClip = GetAnimationClipByKeyword(animatorController, "Aiming Stand Posture");
                StandReloadAnimationClip = GetAnimationClipByKeyword(animatorController, "Stand Reload");
                StandIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "Rifle Idle");

                UpperBodyIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "UpperBody Idle");


                BaseCrouchAimingAnimationClip = GetAnimationClipByKeyword(animatorController, "BaseCrouchAimingAnimationClip");
                CrouchFireAnimationClip = GetAnimationClipByKeyword(animatorController, "Crouch Weapon Recoil");
                CrouchReloadAnimationClip = GetAnimationClipByKeyword(animatorController, "Crouch Reload");
                CrouchAimingAnimationClip = GetAnimationClipByKeyword(animatorController, "Crouch Aiming");
                SprintingAnimationClip = GetAnimationClipByKeyword(animatorController, "Rifle Run");
                StandCoverLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "StandCoverLeft");
                StandCoverRightAnimationClip = GetAnimationClipByKeyword(animatorController, "StandCoverRight");
                StandCoverNeutralAnimationClip = GetAnimationClipByKeyword(animatorController, "Stand Cover Neutral");


                RunForwardAnimationClip = GetAnimationClipByKeyword(animatorController, "Run Forward");
                RunBackwardAnimationClip = GetAnimationClipByKeyword(animatorController, "Run Backward");
                RunRightAnimationClip = GetAnimationClipByKeyword(animatorController, "RunRight");
                RunLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "RunLeft");
                RunForwardRightAnimationClip = GetAnimationClipByKeyword(animatorController, "RunForwardRight");
                RunForwardLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "RunForwardLeft");
                RunBackwardRightAnimationClip = GetAnimationClipByKeyword(animatorController, "RunBackwardRight");
                RunBackwardLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "RunBackwardLeft");


                WalkForwardAimingAnimationClip = GetAnimationClipByKeyword(animatorController, "WalkingForwardAiming");
                WalkRightAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Right");
                WalkLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Left");
                WalkBackwardAnimationClip = GetAnimationClipByKeyword(animatorController, "Walking Backwards");
                WalkForwardRightAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Forward Right");
                WalkBackwardRightAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Backward Right");
                WalkBackwardLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Backward Left");
                WalkForwardLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Forward Left");
                GrenadeThrowAnimationClip = GetAnimationClipByKeyword(animatorController, "Grenade Throw");

                MeleeAttack1AnimationClip = GetAnimationClipByKeyword(animatorController, "MeleeAttack1");
                MeleeAttack2AnimationClip = GetAnimationClipByKeyword(animatorController, "MeleeAttack2");
                MeleeAttack3AnimationClip = GetAnimationClipByKeyword(animatorController, "MeleeAttack3");

                WalkIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "Rifle Walk");

                UpperBodyScanAnimationClip = GetAnimationClipByKeyword(animatorController, "UB Scan Animation");
                TurnBackwardAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnBackward");
                TurnForwardAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnForward");
                TurnLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnLeft");
                TurnRightAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnRight");

                UpperBodyHitAnimationClip1 = GetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation1");
                UpperBodyHitAnimationClip2 = GetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation2");
                UpperBodyHitAnimationClip3 = GetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation3");
                LowerBodyHitAnimationClip1 = GetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation1");
                LowerBodyHitAnimationClip2 = GetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation2");
                LowerBodyHitAnimationClip3 = GetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation3");

                LowerBodyHaltLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "LowerBodyHaltLeftAnimationClip");
                LowerBodyHaltRightAnimationClip = GetAnimationClipByKeyword(animatorController, "LowerBodyHaltRightAnimationClip");
                UpperBodyHaltLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "UpperBodyHaltLeftAnimationClip");
                UpperBodyHaltRightAnimationClip = GetAnimationClipByKeyword(animatorController, "UpperBodyHaltRightAnimationClip");
            }
            else
            {
                StandIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "Stand Idle");
                WalkForwardAimingAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Forward");
                RunForwardAnimationClip = GetAnimationClipByKeyword(animatorController, "Run Forward");
                SprintingAnimationClip = GetAnimationClipByKeyword(animatorController, "Sprinting");


                MeleeAttack1AnimationClip = GetAnimationClipByKeyword(animatorController, "MeleeAttack1");
                MeleeAttack2AnimationClip = GetAnimationClipByKeyword(animatorController, "MeleeAttack2");
                MeleeAttack3AnimationClip = GetAnimationClipByKeyword(animatorController, "MeleeAttack3");

                TurnBackwardAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnBackward");
                TurnForwardAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnForward");
                TurnLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnLeft");
                TurnRightAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnRight");

                UpperBodyHitAnimationClip1 = GetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation1");
                UpperBodyHitAnimationClip2 = GetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation2");
                UpperBodyHitAnimationClip3 = GetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation3");
                LowerBodyHitAnimationClip1 = GetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation1");
                LowerBodyHitAnimationClip2 = GetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation2");
                LowerBodyHitAnimationClip3 = GetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation3");
            }
        }

        private void SaveAnimations()
        {
            if (targetObject == null)
            {
                Debug.LogError("Target Object is not assigned.");
                return;
            }

            Animator animator = targetObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Target Object does not have an Animator component.");
                return;
            }

            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
            if (animatorController == null)
            {
                Debug.LogError("Target Object's Animator does not have an AnimatorController assigned.");
                return;
            }

            if (selectedCharacter == ChooseBetweenZombieAndSoliderOption.Soldier)
            {
                SetAnimationClipByKeyword(animatorController, "BaseStandAimingAnimationClip", BaseStandAimingAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Stand Weapon Recoil", StandFireAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Aiming Stand Posture", StandAimingAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Stand Reload", StandReloadAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Rifle Idle", StandIdleAnimationClip);

                SetAnimationClipByKeyword(animatorController, "UpperBody Idle", UpperBodyIdleAnimationClip);

                SetAnimationClipByKeyword(animatorController, "BaseCrouchAimingAnimationClip", BaseCrouchAimingAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Crouch Weapon Recoil", CrouchFireAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Crouch Reload", CrouchReloadAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Crouch Aiming", CrouchAimingAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Rifle Run", SprintingAnimationClip);
                SetAnimationClipByKeyword(animatorController, "StandCoverLeft", StandCoverLeftAnimationClip);
                SetAnimationClipByKeyword(animatorController, "StandCoverRight", StandCoverRightAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Stand Cover Neutral", StandCoverNeutralAnimationClip);

                SetAnimationClipByKeyword(animatorController, "Run Forward", RunForwardAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Run Backward", RunBackwardAnimationClip);
                SetAnimationClipByKeyword(animatorController, "RunRight", RunRightAnimationClip);
                SetAnimationClipByKeyword(animatorController, "RunLeft", RunLeftAnimationClip);
                SetAnimationClipByKeyword(animatorController, "RunForwardRight", RunForwardRightAnimationClip);
                SetAnimationClipByKeyword(animatorController, "RunForwardLeft", RunForwardLeftAnimationClip);
                SetAnimationClipByKeyword(animatorController, "RunBackwardRight", RunBackwardRightAnimationClip);
                SetAnimationClipByKeyword(animatorController, "RunBackwardLeft", RunBackwardLeftAnimationClip);


                SetAnimationClipByKeyword(animatorController, "Walk Right", WalkRightAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Walk Left", WalkLeftAnimationClip);
                SetAnimationClipByKeyword(animatorController, "WalkingForwardAiming", WalkForwardAimingAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Walking Backwards", WalkBackwardAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Walk Forward Right", WalkForwardRightAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Walk Backward Right", WalkBackwardRightAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Walk Backward Left", WalkBackwardLeftAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Walk Forward Left", WalkForwardLeftAnimationClip);

                SetAnimationClipByKeyword(animatorController, "Grenade Throw", GrenadeThrowAnimationClip);

                SetAnimationClipByKeyword(animatorController, "MeleeAttack1", MeleeAttack1AnimationClip);
                SetAnimationClipByKeyword(animatorController, "MeleeAttack2", MeleeAttack2AnimationClip);
                SetAnimationClipByKeyword(animatorController, "MeleeAttack3", MeleeAttack3AnimationClip);

                SetAnimationClipByKeyword(animatorController, "Rifle Walk", WalkIdleAnimationClip);
                SetAnimationClipByKeyword(animatorController, "UB Scan Animation", UpperBodyScanAnimationClip);
                SetAnimationClipByKeyword(animatorController, "TurnBackward", TurnBackwardAnimationClip);
                SetAnimationClipByKeyword(animatorController, "TurnForward", TurnForwardAnimationClip);
                SetAnimationClipByKeyword(animatorController, "TurnLeft", TurnLeftAnimationClip);
                SetAnimationClipByKeyword(animatorController, "TurnRight", TurnRightAnimationClip);

                SetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation1", UpperBodyHitAnimationClip1);
                SetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation2", UpperBodyHitAnimationClip2);
                SetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation3", UpperBodyHitAnimationClip3);
                SetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation1", LowerBodyHitAnimationClip1);
                SetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation2", LowerBodyHitAnimationClip2);
                SetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation3", LowerBodyHitAnimationClip3);

                SetAnimationClipByKeyword(animatorController, "LowerBodyHaltLeftAnimationClip", LowerBodyHaltLeftAnimationClip);
                SetAnimationClipByKeyword(animatorController, "LowerBodyHaltRightAnimationClip", LowerBodyHaltRightAnimationClip);
                SetAnimationClipByKeyword(animatorController, "UpperBodyHaltLeftAnimationClip", UpperBodyHaltLeftAnimationClip);
                SetAnimationClipByKeyword(animatorController, "UpperBodyHaltRightAnimationClip", UpperBodyHaltRightAnimationClip);
            }
            else
            {
                SetAnimationClipByKeyword(animatorController, "Stand Idle", StandIdleAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Run Forward", RunForwardAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Sprinting", SprintingAnimationClip);
                SetAnimationClipByKeyword(animatorController, "Walk Forward", WalkForwardAimingAnimationClip);

                SetAnimationClipByKeyword(animatorController, "MeleeAttack1", MeleeAttack1AnimationClip);
                SetAnimationClipByKeyword(animatorController, "MeleeAttack2", MeleeAttack2AnimationClip);
                SetAnimationClipByKeyword(animatorController, "MeleeAttack3", MeleeAttack3AnimationClip);

                SetAnimationClipByKeyword(animatorController, "TurnBackward", TurnBackwardAnimationClip);
                SetAnimationClipByKeyword(animatorController, "TurnForward", TurnForwardAnimationClip);
                SetAnimationClipByKeyword(animatorController, "TurnLeft", TurnLeftAnimationClip);
                SetAnimationClipByKeyword(animatorController, "TurnRight", TurnRightAnimationClip);

                SetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation1", UpperBodyHitAnimationClip1);
                SetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation2", UpperBodyHitAnimationClip2);
                SetAnimationClipByKeyword(animatorController, "UpperBodyHitAnimation3", UpperBodyHitAnimationClip3);
                SetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation1", LowerBodyHitAnimationClip1);
                SetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation2", LowerBodyHitAnimationClip2);
                SetAnimationClipByKeyword(animatorController, "LowerBodyHitAnimation3", LowerBodyHitAnimationClip3);
            }
        }

        private void ResetAnimations()
        {
            BaseStandAimingAnimationClip = null;
            StandFireAnimationClip = null;
            StandAimingAnimationClip = null;
            StandReloadAnimationClip = null;
            StandIdleAnimationClip = null;

            BaseCrouchAimingAnimationClip = null;
            CrouchFireAnimationClip = null;
            CrouchReloadAnimationClip = null;
            CrouchAimingAnimationClip = null;
            SprintingAnimationClip = null;
            StandCoverLeftAnimationClip = null;
            StandCoverRightAnimationClip = null;
            StandCoverNeutralAnimationClip = null;

            RunForwardAnimationClip = null;
            RunBackwardAnimationClip = null;
            RunLeftAnimationClip = null;
            RunRightAnimationClip = null;
            RunForwardRightAnimationClip = null;
            RunForwardLeftAnimationClip = null;
            RunBackwardRightAnimationClip = null;
            RunBackwardLeftAnimationClip = null;

            WalkForwardAimingAnimationClip = null;
            WalkRightAnimationClip = null;
            WalkLeftAnimationClip = null;
            WalkBackwardAnimationClip = null;
            WalkForwardRightAnimationClip = null;
            WalkBackwardRightAnimationClip = null;
            WalkBackwardLeftAnimationClip = null;
            WalkForwardLeftAnimationClip = null;
            GrenadeThrowAnimationClip = null;

            MeleeAttack1AnimationClip = null;
            MeleeAttack2AnimationClip = null;
            MeleeAttack3AnimationClip = null;

            WalkIdleAnimationClip = null;
            UpperBodyIdleAnimationClip = null;

            UpperBodyScanAnimationClip = null;
            TurnBackwardAnimationClip = null;
            TurnForwardAnimationClip = null;
            TurnLeftAnimationClip = null;
            TurnRightAnimationClip = null;

            UpperBodyHitAnimationClip1 = null;
            UpperBodyHitAnimationClip2 = null;
            UpperBodyHitAnimationClip3 = null;
            LowerBodyHitAnimationClip1 = null;
            LowerBodyHitAnimationClip2 = null;
            LowerBodyHitAnimationClip3 = null;

            LowerBodyHaltLeftAnimationClip = null;
            LowerBodyHaltRightAnimationClip = null;
            UpperBodyHaltLeftAnimationClip = null;
            UpperBodyHaltRightAnimationClip = null;
        }

        private AnimationClip GetAnimationClipByKeyword(AnimatorController animatorController, string keyword)
        {
            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {
                    if (state.state.name.Contains(keyword))
                    {
                        return state.state.motion as AnimationClip;
                    }
                }
            }

            return null;
        }
        private void SetAnimationClipByKeyword(AnimatorController animatorController, string keyword, AnimationClip animationClip)
        {
            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {
                    if (state.state.name.Contains(keyword))
                    {
                        state.state.motion = animationClip;
                    }
                }
            }
        }
    }
}



// Previous code 
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEditor.Animations;
//using UnityEngine;

//namespace MobileActionKit
//{
//    public enum ChooseBetweenZombieAndSoliderOption
//    {
//        Soldier,
//        Zombie
//    }
//    public class HumanoidAiAnimationEditor : EditorWindow
//    {
//       // private string AnimatorControllerName;
//        private GameObject targetObject;
//        [Header("Combat Animations")]
//        private AnimationClip StandPoseAnimationClip;
//        private AnimationClip StandFireAnimationClip;
//        private AnimationClip StandAimingAnimationClip;
//        private AnimationClip StandReloadAnimationClip;
//        private AnimationClip StandIdleAnimationClip;

//        private AnimationClip CrouchPoseAnimationClip;
//        private AnimationClip CrouchFireAnimationClip;
//        private AnimationClip CrouchReloadAnimationClip;
//        private AnimationClip CrouchAimingAnimationClip;
//        private AnimationClip SprintingAnimationClip;
//        private AnimationClip StandCoverLeftAnimationClip;
//        private AnimationClip StandCoverRightAnimationClip;
//        private AnimationClip StandCoverNeutralAnimationClip;

//        private AnimationClip RunForwardAnimationClip;
//        private AnimationClip RunRightAnimationClip;
//        private AnimationClip RunLeftAnimationClip;
//        private AnimationClip RunBackwardAnimationClip;
//        private AnimationClip RunForwardRightAnimationClip;
//        private AnimationClip RunBackwardRightAnimationClip;
//        private AnimationClip RunBackwardLeftAnimationClip;
//        private AnimationClip RunForwardLeftAnimationClip;

//        private AnimationClip WalkForwardAnimationClip;
//        private AnimationClip WalkRightAnimationClip;
//        private AnimationClip WalkLeftAnimationClip;
//        private AnimationClip WalkBackwardAnimationClip;
//        private AnimationClip WalkForwardRightAnimationClip;
//        private AnimationClip WalkBackwardRightAnimationClip;
//        private AnimationClip WalkBackwardLeftAnimationClip;
//        private AnimationClip WalkForwardLeftAnimationClip;
//        private AnimationClip GrenadeThrowAnimationClip;

//        private AnimationClip MeleeAttack1AnimationClip;
//        private AnimationClip MeleeAttack2AnimationClip;
//        private AnimationClip MeleeAttack3AnimationClip;

//        [Header("NonCombat Animations")]
//        private AnimationClip WalkIdleAnimationClip;
//        private AnimationClip UpperBodyIdleAnimationClip;
//        private AnimationClip UpperBodyScanAnimationClip;
//        private AnimationClip TurnLeftAnimationClip;
//        private AnimationClip TurnRightAnimationClip;
//        private AnimationClip TurnForwardAnimationClip;
//        private AnimationClip TurnBackwardAnimationClip;
//        private AnimationClip UpperBodyHitAnimationClip1;
//        private AnimationClip UpperBodyHitAnimationClip2;
//        private AnimationClip UpperBodyHitAnimationClip3;
//        private AnimationClip LowerBodyHitAnimationClip1;
//        private AnimationClip LowerBodyHitAnimationClip2;
//        private AnimationClip LowerBodyHitAnimationClip3;

//        private AnimationClip LowerBodyLeftCoverIdleAnimationClip;
//        private AnimationClip LowerBodyRightCoverIdleAnimationClip;
//        private AnimationClip UpperBodyLeftCoverIdleAnimationClip;
//        private AnimationClip UpperBodyRightCoverIdleAnimationClip;

//        private List<AnimationClip> DeathAnimationClips = new List<AnimationClip>();

//        private Vector2 scrollPosition;

//        private Texture2D customButtonImage;

//        private ChooseBetweenZombieAndSoliderOption selectedCharacter = ChooseBetweenZombieAndSoliderOption.Soldier; // Default to Soldier
//        Animator anim;

//        [MenuItem("Tools/MobileActionKit/Humanoid AI/Edit Humanoid AI Animations", priority = 1)]
//        private static void Init()
//        {
//            HumanoidAiAnimationEditor window = (HumanoidAiAnimationEditor)EditorWindow.GetWindow(typeof(HumanoidAiAnimationEditor));
//            window.titleContent = new GUIContent("Edit Humanoid AI Animations");
//            window.Show();
//        }
//        private void OnGUI()
//        {
//            customButtonImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Mobile Action Kit/Art/EditorWindows_Art/btn.png");

//            float windowWidth = position.width;
//            float windowHeight = position.height;

//            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
//            float labelWidth = windowWidth * 0.4f;
//            float fieldWidth = windowWidth * 0.5f;
//            float spaceHeight = windowHeight * 0.02f;
//            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
//            labelStyle.wordWrap = true;

//            GUILayout.Label("Target Object", EditorStyles.boldLabel);
//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Humanoid Ai Animator Controller", labelStyle, GUILayout.Width(labelWidth));
//            targetObject = (GameObject)EditorGUILayout.ObjectField(targetObject, typeof(GameObject), true, GUILayout.Width(fieldWidth));
//            GUILayout.EndHorizontal();


//            //GUILayout.Label("Custom Animator", EditorStyles.boldLabel);
//            //GUILayout.BeginHorizontal();
//            //GUILayout.Label("Animator Controller Name", labelStyle, GUILayout.Width(labelWidth));
//            //AnimatorControllerName = EditorGUILayout.TextField(AnimatorControllerName, GUILayout.Width(fieldWidth));
//            //GUILayout.EndHorizontal();

//            //if (GUILayout.Button(new GUIContent("Create New Animator")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
//            //{
//            //    DuplicateAnimator();
//            //}

//            GUILayout.Label("Character Type", EditorStyles.boldLabel);
//            selectedCharacter = (ChooseBetweenZombieAndSoliderOption)EditorGUILayout.EnumPopup(selectedCharacter);

//            GUILayout.Space(spaceHeight);

//            if(selectedCharacter == ChooseBetweenZombieAndSoliderOption.Soldier)
//            {
//                GUILayout.Label("COMBAT ANIMATION CLIPS", EditorStyles.boldLabel);

//                GUILayout.Space(spaceHeight);

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("StandPoseAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                StandPoseAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandPoseAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Rifle+Aiming+Idle");
//                GUILayout.EndHorizontal();


//                GUILayout.BeginHorizontal();
//                GUILayout.Label("StandFireAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                StandFireAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandFireAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Firing+Rifle");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("StandReloadAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                StandReloadAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandReloadAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Reloading");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("StandIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                StandIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=+Rifle+Idle");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("StandAimingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                StandAimingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandAimingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Rifle+Aiming+Idle");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("CrouchPoseAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                CrouchPoseAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(CrouchPoseAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Crouch+Idle");
//                GUILayout.EndHorizontal();


//                GUILayout.BeginHorizontal();
//                GUILayout.Label("CrouchFireAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                CrouchFireAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(CrouchFireAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Crouch+Rapid+Fire");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("CrouchReloadAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                CrouchReloadAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(CrouchReloadAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Reload+Crouch");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("CrouchAimingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                CrouchAimingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(CrouchAimingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Crouch+Idle");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("SprintingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                SprintingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(SprintingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Rifle+Run");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("StandCoverLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                StandCoverLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandCoverLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Taking+Cover+Idle");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("StandCoverRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                StandCoverRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandCoverRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Taking+Cover+Idle");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("StandCoverNeutralAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                StandCoverNeutralAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandCoverNeutralAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=+Rifle+Idle");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("RunForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                RunForwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunForwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+Forward");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("RunBackwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                RunBackwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunBackwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+backward");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("RunLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                RunLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+Strafe+left");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("RunRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                RunRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+Strafe+Right+");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("RunForwardRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                RunForwardRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunForwardRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=+jog+forward++right");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("RunForwardLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                RunForwardLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunForwardLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=+jog+forward+left");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("RunBackwardRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                RunBackwardRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunBackwardRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+backward+left");
//                GUILayout.EndHorizontal();


//                GUILayout.BeginHorizontal();
//                GUILayout.Label("RunBackwardLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                RunBackwardLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunBackwardLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+backward+left");
//                GUILayout.EndHorizontal();


//                GUILayout.BeginHorizontal();
//                GUILayout.Label("WalkForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                WalkForwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkForwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero%2CSkinning+Test&page=1&query=Walking+");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("WalkRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                WalkRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Strafe+Right");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("WalkLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                WalkLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Strafe+Left");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("WalkBackwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                WalkBackwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkBackwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Walking+Backwards");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("WalkForwardRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                WalkForwardRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkForwardRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Walk+Forward+Right");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("WalkBackwardRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                WalkBackwardRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkBackwardRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Walk+Backward+Right");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("WalkBackwardLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                WalkBackwardLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkBackwardLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Walk+Backward+Left");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("WalkForwardLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                WalkForwardLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkForwardLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Walk+Forward+Left");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("GrenadeThrowAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                GrenadeThrowAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(GrenadeThrowAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Throw");
//                GUILayout.EndHorizontal();


//                GUILayout.BeginHorizontal();
//                GUILayout.Label("MeleeAttack1AnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                MeleeAttack1AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack1AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Bayonet+Stab");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("MeleeAttack2AnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                MeleeAttack2AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack2AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Punch");
//                GUILayout.EndHorizontal();


//                GUILayout.BeginHorizontal();
//                GUILayout.Label("MeleeAttack3AnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                MeleeAttack3AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack3AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn+And+Kick");
//                GUILayout.EndHorizontal();

//                GUILayout.Space(spaceHeight);

//                GUILayout.Label("NONCOMBAT ANIMATION CLIPS", EditorStyles.boldLabel);

//                GUILayout.Space(spaceHeight);

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("WalkIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                WalkIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Rifle+Walk");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("UpperBodyIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                UpperBodyIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.google.com");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("UpperBodyScanAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                UpperBodyScanAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyScanAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.google.com");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("TurnLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                TurnLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("TurnRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                TurnRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("TurnForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                TurnForwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnForwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("TurnBackwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                TurnBackwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnBackwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("UpperBodyHitAnimationClip1", labelStyle, GUILayout.Width(labelWidth));
//                UpperBodyHitAnimationClip1 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip1, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("UpperBodyHitAnimationClip2", labelStyle, GUILayout.Width(labelWidth));
//                UpperBodyHitAnimationClip2 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip2, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("UpperBodyHitAnimationClip3", labelStyle, GUILayout.Width(labelWidth));
//                UpperBodyHitAnimationClip3 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip3, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("LowerBodyHitAnimationClip1", labelStyle, GUILayout.Width(labelWidth));
//                LowerBodyHitAnimationClip1 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip1, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("LowerBodyHitAnimationClip2", labelStyle, GUILayout.Width(labelWidth));
//                LowerBodyHitAnimationClip2 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip2, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("LowerBodyHitAnimationClip3", labelStyle, GUILayout.Width(labelWidth));
//                LowerBodyHitAnimationClip3 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip3, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("LowerBodyLeftCoverIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                LowerBodyLeftCoverIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyLeftCoverIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=cover&type=Motion%2CMotionPack");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("LowerBodyRightCoverIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                LowerBodyRightCoverIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyRightCoverIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=cover&type=Motion%2CMotionPack");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("UpperBodyLeftCoverIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                UpperBodyLeftCoverIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyLeftCoverIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=cover&type=Motion%2CMotionPack");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("UpperBodyRightCoverIdleAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                UpperBodyRightCoverIdleAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyRightCoverIdleAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=cover&type=Motion%2CMotionPack");
//                GUILayout.EndHorizontal();

//                GUILayout.Space(spaceHeight);
//                GUILayout.FlexibleSpace();
//            }
//            else
//            {
//                GUILayout.BeginHorizontal();
//                GUILayout.Label("TurnLeftAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                TurnLeftAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnLeftAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=rifle+turn+left");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("TurnRightAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                TurnRightAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnRightAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=zombie+turn+right");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("TurnForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                TurnForwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnForwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.google.com");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("TurnBackwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                TurnBackwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(TurnBackwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.google.com");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("WalkForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                WalkForwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(WalkForwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero%2CSkinning+Test&page=1&query=Walking+");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("RunForwardAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                RunForwardAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(RunForwardAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Jog+Forward");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("SprintingAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                SprintingAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(SprintingAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?genres=Combat%2CAdventure%2CSport%2CDance%2CFantasy%2CSuperhero&page=1&query=Rifle+Run");
//                GUILayout.EndHorizontal();


//                GUILayout.BeginHorizontal();
//                GUILayout.Label("MeleeAttack1AnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                MeleeAttack1AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack1AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Bayonet+Stab");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("MeleeAttack2AnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                MeleeAttack2AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack2AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Punch");
//                GUILayout.EndHorizontal();


//                GUILayout.BeginHorizontal();
//                GUILayout.Label("MeleeAttack3AnimationClip", labelStyle, GUILayout.Width(labelWidth));
//                MeleeAttack3AnimationClip = (AnimationClip)EditorGUILayout.ObjectField(MeleeAttack3AnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=Rifle+Turn+And+Kick");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("UpperBodyHitAnimationClip1", labelStyle, GUILayout.Width(labelWidth));
//                UpperBodyHitAnimationClip1 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip1, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("UpperBodyHitAnimationClip2", labelStyle, GUILayout.Width(labelWidth));
//                UpperBodyHitAnimationClip2 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip2, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("UpperBodyHitAnimationClip3", labelStyle, GUILayout.Width(labelWidth));
//                UpperBodyHitAnimationClip3 = (AnimationClip)EditorGUILayout.ObjectField(UpperBodyHitAnimationClip3, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("LowerBodyHitAnimationClip1", labelStyle, GUILayout.Width(labelWidth));
//                LowerBodyHitAnimationClip1 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip1, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("LowerBodyHitAnimationClip2", labelStyle, GUILayout.Width(labelWidth));
//                LowerBodyHitAnimationClip2 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip2, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//                GUILayout.BeginHorizontal();
//                GUILayout.Label("LowerBodyHitAnimationClip3", labelStyle, GUILayout.Width(labelWidth));
//                LowerBodyHitAnimationClip3 = (AnimationClip)EditorGUILayout.ObjectField(LowerBodyHitAnimationClip3, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//                OpenLinkButton("https://www.mixamo.com/#/?page=1&query=hit");
//                GUILayout.EndHorizontal();

//            }



//            GUILayout.Label("Drag and Drop Death Animation Clips Here:", EditorStyles.boldLabel);

//            Event evt = Event.current;
//            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50f);
//            GUI.Box(dropArea, "Drop Animation Clips Here");

//            switch (evt.type)
//            {
//                case EventType.DragUpdated:
//                case EventType.DragPerform:
//                    if (!dropArea.Contains(evt.mousePosition))
//                        break;

//                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

//                    if (evt.type == EventType.DragPerform)
//                    {
//                        DragAndDrop.AcceptDrag();
//                        DeathAnimationClips.AddRange(GetAnimationClipsFromDraggedObjects());
//                        Repaint();
//                    }

//                    Event.current.Use();
//                    break;
//            }

//            if (DeathAnimationClips.Count > 0)
//            {
//                GUILayout.Label("Dragged Animation Clips:");
//                foreach (AnimationClip clip in DeathAnimationClips)
//                {
//                    EditorGUILayout.LabelField(clip.name);
//                }

//                GUILayout.Space(10);
//            }

//            if (GUILayout.Button(new GUIContent("Add Death Animation clips To Animator")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
//            {
//                if (targetObject != null)
//                {
//                    anim = targetObject.GetComponent<Animator>();
//                }

//                if (anim != null && DeathAnimationClips.Count > 0)
//                {
//                    if(selectedCharacter == ChooseBetweenZombieAndSoliderOption.Soldier)
//                    {
//                        AssignAnimationClipsToLayer(anim, 5, DeathAnimationClips.ToArray());
//                    }
//                    else
//                    {
//                        AssignAnimationClipsToLayer(anim, 2, DeathAnimationClips.ToArray());
//                    }

//                    DeathAnimationClips.Clear();
//                    Repaint();
//                }
//                else
//                {
//                    Debug.LogWarning("Please select an Animator and drag Animation Clips before assigning.");
//                }
//            }

//            if (GUILayout.Button("Remove Last Dragged Death Animation Clip"))//, buttonStyle))
//            {
//                RemoveLastUnassignedAnimationClip();
//            }

//            if (GUILayout.Button(new GUIContent("Auto Modify Animation Import Settings")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
//            {
//                ModifyAnimationSettingsWithFullChanges(StandPoseAnimationClip, "StandPoseAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(StandFireAnimationClip, "StandFireAnimationClip");            
//                ModifyAnimationSettingsWithFullChanges(StandIdleAnimationClip, "StandIdleAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(StandAimingAnimationClip, "StandAimingAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(CrouchPoseAnimationClip, "CrouchPoseAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(CrouchFireAnimationClip, "CrouchFireAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(CrouchAimingAnimationClip, "CrouchAimingAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(SprintingAnimationClip, "SprintingAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(StandCoverLeftAnimationClip, "StandCoverLeftAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(StandCoverRightAnimationClip, "StandCoverRightAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(StandCoverNeutralAnimationClip, "StandCoverNeutralAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(RunForwardAnimationClip, "RunForwardAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(RunBackwardAnimationClip, "RunBackwardAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(RunLeftAnimationClip, "RunLeftAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(RunRightAnimationClip, "RunRightAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(RunForwardRightAnimationClip, "RunForwardRightAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(RunForwardLeftAnimationClip, "RunForwardLeftAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(RunBackwardRightAnimationClip, "RunBackwardRightAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(RunBackwardLeftAnimationClip, "RunBackwardLeftAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(WalkForwardAnimationClip, "WalkForwardAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(WalkRightAnimationClip, "WalkRightAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(WalkLeftAnimationClip, "WalkLeftAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(WalkBackwardAnimationClip, "WalkBackwardAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(WalkBackwardLeftAnimationClip, "WalkBackwardLeftAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(WalkForwardLeftAnimationClip, "WalkForwardLeftAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(WalkIdleAnimationClip, "WalkIdleAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(UpperBodyIdleAnimationClip, "UpperBodyIdleAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(UpperBodyScanAnimationClip, "UpperBodyScanAnimationClip");

//                ModifyAnimationSettingsWithFullChanges(LowerBodyLeftCoverIdleAnimationClip, "LowerBodyLeftCoverIdleAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(LowerBodyRightCoverIdleAnimationClip, "LowerBodyRightCoverIdleAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(UpperBodyLeftCoverIdleAnimationClip, "UpperBodyLeftCoverIdleAnimationClip");
//                ModifyAnimationSettingsWithFullChanges(UpperBodyRightCoverIdleAnimationClip, "UpperBodyRightCoverIdleAnimationClip");


//                ModifyAnimationSettingsWithNoLoop(MeleeAttack1AnimationClip, "MeleeAttack1AnimationClip");
//                ModifyAnimationSettingsWithNoLoop(MeleeAttack2AnimationClip, "MeleeAttack2AnimationClip");
//                ModifyAnimationSettingsWithNoLoop(MeleeAttack3AnimationClip, "MeleeAttack3AnimationClip");

//                ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnLeftAnimationClip, "TurnLeftAnimationClip");
//                ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnRightAnimationClip, "TurnRightAnimationClip");
//                ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnForwardAnimationClip, "TurnForwardAnimationClip");
//                ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(TurnBackwardAnimationClip, "TurnBackwardAnimationClip");

//                ModifyAnimationSettingsWithNoLoop(UpperBodyHitAnimationClip1, "UpperBodyHitAnimationClip1");
//                ModifyAnimationSettingsWithNoLoop(UpperBodyHitAnimationClip2, "UpperBodyHitAnimationClip2");
//                ModifyAnimationSettingsWithNoLoop(UpperBodyHitAnimationClip3, "UpperBodyHitAnimationClip3");
//                ModifyAnimationSettingsWithNoLoop(LowerBodyHitAnimationClip1, "LowerBodyHitAnimationClip1");
//                ModifyAnimationSettingsWithNoLoop(LowerBodyHitAnimationClip2, "LowerBodyHitAnimationClip2");
//                ModifyAnimationSettingsWithNoLoop(LowerBodyHitAnimationClip3, "LowerBodyHitAnimationClip3");


//                ModifyAnimationSettingsWithNoLoop(StandReloadAnimationClip, "StandReloadAnimationClip");
//                ModifyAnimationSettingsWithNoLoop(GrenadeThrowAnimationClip, "GrenadeThrowAnimationClip");
//                ModifyAnimationSettingsWithNoLoop(CrouchReloadAnimationClip, "CrouchReloadAnimationClip");

//                if (anim != null && DeathAnimationClips.Count > 0)
//                {
//                    for(int x = 0;x < DeathAnimationClips.Count; x++)
//                    {
//                        ModifyAnimationSettingsWithNoLoop(DeathAnimationClips[x], "DeathAnimationClips" + x.ToString());
//                    }
//                }

//            }

//            if (GUILayout.Button(new GUIContent("Import Existing Animations From Animator")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
//            {
//                ExportAnimations();
//            }
//            if (GUILayout.Button(new GUIContent("Save New Animations To Animator")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
//            {
//                SaveAnimations();
//                EditorWindow.GetWindow<HumanoidAiAnimationEditor>().Close();
//            }
//            if (GUILayout.Button(new GUIContent("Reset Animations Clips In this Window")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
//            {
//                ResetAnimations();
//            }
//            GUILayout.FlexibleSpace();

//            GUILayout.EndScrollView();

//        }
//        private AnimationClip[] GetAnimationClipsFromDraggedObjects()
//        {
//            Object[] draggedObjects = DragAndDrop.objectReferences;
//            List<AnimationClip> clips = new List<AnimationClip>();

//            foreach (Object obj in draggedObjects)
//            {
//                AnimationClip clip = obj as AnimationClip;
//                if (clip != null)
//                {
//                    clips.Add(clip);
//                }
//            }

//            return clips.ToArray();
//        }
//        private void OpenLinkButton(string link)
//        {
//            // Use custom button image for the button, without any text
//            if (GUILayout.Button(customButtonImage, GUILayout.Width(21), GUILayout.Height(21))) // Adjust width and height as needed
//            {
//                Application.OpenURL(link);
//            }
//        }
//        private void ModifyAnimationSettingsWithFullChanges(AnimationClip clip, string FieldName)
//        {
//            if (clip != null)
//            {
//                ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;

//                if (modelImporter != null)
//                {
//                    // Set Humanoid settings
//                    modelImporter.animationType = ModelImporterAnimationType.Human;
//                    //modelImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
//                    ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

//                    foreach (var clipAnimation in clipAnimations)
//                    {
//                        clipAnimation.name = FieldName;
//                        // Activate "Bake Into Pose"
//                        clipAnimation.lockRootRotation = true;
//                        clipAnimation.lockRootHeightY = true;

//                        // Choose wrap mode based on original animation
//                        clipAnimation.loop = clip.isLooping;

//                        // Set Root Transform Rotation, Loop Pose, and Loop Time to Original
//                        clipAnimation.loopTime = true;
//                        clipAnimation.loopPose = true;

//                        clipAnimation.keepOriginalOrientation = true;

//                        // Set Based Upon to Original for Root Transform Rotation
//                        clipAnimation.cycleOffset = 0f;
//                    }

//                    modelImporter.clipAnimations = clipAnimations;

//                    modelImporter.SaveAndReimport();
//                }
//            }
//        }
//        private void ModifyAnimationSettingsWithOnlyYPositionBasedUponAndNoLoop(AnimationClip clip, string FieldName)
//        {
//            if (clip != null)
//            {
//                ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;

//                if (modelImporter != null)
//                {
//                    // Set Humanoid settings
//                    modelImporter.animationType = ModelImporterAnimationType.Human;
//                    //modelImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
//                    ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

//                    foreach (var clipAnimation in clipAnimations)
//                    {
//                        clipAnimation.name = FieldName;
//                        // Activate "Bake Into Pose"
//                        //clipAnimation.lockRootRotation = true;
//                        //clipAnimation.lockRootHeightY = true;

//                        // Choose wrap mode based on original animation
//                        clipAnimation.loop = clip.isLooping;

//                        // Set Root Transform Rotation, Loop Pose, and Loop Time to Original
//                        //clipAnimation.loopTime = true;
//                        //clipAnimation.loopPose = true;

//                        //clipAnimation.keepOriginalOrientation = true;

//                        // Set Based Upon to Original for Root Transform Rotation
//                        clipAnimation.cycleOffset = 0f;
//                    }

//                    modelImporter.clipAnimations = clipAnimations;

//                    modelImporter.SaveAndReimport();
//                }
//            }
//        }
//        private void ModifyAnimationSettingsWithNoLoop(AnimationClip clip,string FieldName)
//        {
//            if (clip != null)
//            {
//                ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;

//                if (modelImporter != null)
//                {
//                    // Set Humanoid settings 
//                    modelImporter.animationType = ModelImporterAnimationType.Human;
//                    //modelImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

//                    ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

//                    foreach (var clipAnimation in clipAnimations)
//                    {
//                        clipAnimation.name = FieldName;
//                        // Activate "Bake Into Pose"
//                        clipAnimation.lockRootRotation = true;
//                        clipAnimation.lockRootHeightY = true;

//                        // Choose wrap mode based on original animation
//                        clipAnimation.loop = clip.isLooping;

//                        //clipAnimation.keepOriginalOrientation = true;

//                        // Set Based Upon to Original for Root Transform Rotation
//                        clipAnimation.cycleOffset = 0f;
//                    }

//                    modelImporter.clipAnimations = clipAnimations;

//                    modelImporter.SaveAndReimport();

//                }
//            }
//        }
//        private void AssignAnimationClipsToLayer(Animator animator, int layerIndex, AnimationClip[] clips)
//        {
//            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

//            if (controller != null)
//            {
//                AnimatorControllerLayer[] layers = controller.layers;

//                if (layerIndex >= 0 && layerIndex < layers.Length)
//                {
//                    AnimatorStateMachine stateMachine = layers[layerIndex].stateMachine;

//                    foreach (AnimationClip clip in clips)
//                    {
//                        AnimatorState state = stateMachine.AddState(clip.name);
//                        state.motion = clip;
//                    }

//                    Debug.Log("Animation clips assigned successfully.");
//                }
//                else
//                {
//                    Debug.LogWarning("Invalid layer index. Please enter a valid layer index.");
//                }
//            }
//            else
//            {
//                Debug.LogWarning("Selected Animator does not have a valid Animator Controller.");
//            }
//        }
//        private void RemoveLastUnassignedAnimationClip()
//        {
//            if (DeathAnimationClips.Count > 0)
//            {
//                AnimationClip lastUnassignedClip = DeathAnimationClips[DeathAnimationClips.Count - 1];
//                DeathAnimationClips.RemoveAt(DeathAnimationClips.Count - 1);
//                Debug.Log("Last unassigned animation clip removed successfully.");
//            }
//            else
//            {
//                Debug.LogWarning("No unassigned animation clips to remove.");
//            }
//        }
//        //private void ModifyAnimationSettings(AnimationClip clip)
//        //{
//        //    if (clip != null)
//        //    {
//        //        ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;

//        //        if (modelImporter != null)
//        //        {
//        //            ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

//        //            foreach (var clipAnimation in clipAnimations)
//        //            {
//        //                // Activate "Bake Into Pose"
//        //                clipAnimation.lockRootRotation = true;
//        //                clipAnimation.lockRootHeightY = true;

//        //                // Choose wrap mode based on original animation
//        //                clipAnimation.loop = clip.isLooping;

//        //                // Set Root Transform Rotation, Loop Pose, and Loop Time to Original
//        //                clipAnimation.loopTime = true;
//        //                clipAnimation.loopPose = true;

//        //                clipAnimation.keepOriginalOrientation = true;

//        //                // Set Based Upon to Original for Root Transform Rotation
//        //                clipAnimation.cycleOffset = 0f;

//        //            }

//        //            modelImporter.clipAnimations = clipAnimations;
//        //            modelImporter.SaveAndReimport();
//        //        }
//        //    }
//        //}
//        private void ExportAnimations()
//        {
//            if (targetObject == null)
//            {
//                Debug.LogError("Target Object is not assigned.");
//                return;
//            }

//            Animator animator = targetObject.GetComponent<Animator>();
//            if (animator == null)
//            {
//                Debug.LogError("Target Object does not have an Animator component.");
//                return;
//            }

//            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
//            if (animatorController == null)
//            {
//                Debug.LogError("Target Object's Animator does not have an AnimatorController assigned.");
//                return;
//            }

//            StandPoseAnimationClip = GetAnimationClipByKeyword(animatorController, "Stand Pose");

//            StandFireAnimationClip = GetAnimationClipByKeyword(animatorController, "Stand Weapon Recoil");
//            StandAimingAnimationClip = GetAnimationClipByKeyword(animatorController, "Aiming Stand Posture");
//            StandReloadAnimationClip = GetAnimationClipByKeyword(animatorController, "Stand Reload");
//            StandIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "Rifle Idle");

//            UpperBodyIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "UpperBody Idle");


//            CrouchPoseAnimationClip = GetAnimationClipByKeyword(animatorController, "Crouch Pose");
//            CrouchFireAnimationClip = GetAnimationClipByKeyword(animatorController, "Crouch Weapon Recoil");
//            CrouchReloadAnimationClip = GetAnimationClipByKeyword(animatorController, "Crouch Reload");
//            CrouchAimingAnimationClip = GetAnimationClipByKeyword(animatorController, "Crouch Aiming");
//            SprintingAnimationClip = GetAnimationClipByKeyword(animatorController, "Rifle Run");
//            StandCoverLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "StandCoverLeft");
//            StandCoverRightAnimationClip = GetAnimationClipByKeyword(animatorController, "StandCoverRight");
//            StandCoverNeutralAnimationClip = GetAnimationClipByKeyword(animatorController, "Stand Cover Neutral");


//            RunForwardAnimationClip = GetAnimationClipByKeyword(animatorController, "Run Forward");
//            RunBackwardAnimationClip = GetAnimationClipByKeyword(animatorController, "Run Backward");
//            RunRightAnimationClip = GetAnimationClipByKeyword(animatorController, "RunRight");
//            RunLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "RunLeft");
//            RunForwardRightAnimationClip = GetAnimationClipByKeyword(animatorController, "RunForwardRight");
//            RunForwardLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "RunForwardLeft");
//            RunBackwardRightAnimationClip = GetAnimationClipByKeyword(animatorController, "RunBackwardRight");
//            RunBackwardLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "RunBackwardLeft");


//            WalkForwardAnimationClip = GetAnimationClipByKeyword(animatorController, "WalkingForwardAiming");
//            WalkRightAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Right");
//            WalkLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Left");
//            WalkBackwardAnimationClip = GetAnimationClipByKeyword(animatorController, "Walking Backwards");
//            WalkForwardRightAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Forward Right");
//            WalkBackwardRightAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Backward Right");
//            WalkBackwardLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Backward Left");
//            WalkForwardLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "Walk Forward Left");
//            GrenadeThrowAnimationClip = GetAnimationClipByKeyword(animatorController, "Grenade Throw");

//            MeleeAttack1AnimationClip = GetAnimationClipByKeyword(animatorController, "MeleeAttack1");
//            MeleeAttack2AnimationClip = GetAnimationClipByKeyword(animatorController, "MeleeAttack2");
//            MeleeAttack3AnimationClip = GetAnimationClipByKeyword(animatorController, "MeleeAttack3");

//            WalkIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "Rifle Walk");

//            UpperBodyScanAnimationClip = GetAnimationClipByKeyword(animatorController, "UB Scan Animation");
//            TurnBackwardAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnBackward");
//            TurnForwardAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnForward");
//            TurnLeftAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnLeft");
//            TurnRightAnimationClip = GetAnimationClipByKeyword(animatorController, "TurnRight");

//            UpperBodyHitAnimationClip1 = GetAnimationClipByKeyword(animatorController, "Upper Body Hit Animation 1");
//            UpperBodyHitAnimationClip2 = GetAnimationClipByKeyword(animatorController, "Upper Body Hit Animation 2");
//            UpperBodyHitAnimationClip3 = GetAnimationClipByKeyword(animatorController, "Upper Body Hit Animation 3");
//            LowerBodyHitAnimationClip1 = GetAnimationClipByKeyword(animatorController, "Lower Body Hit Animation 1");
//            LowerBodyHitAnimationClip2 = GetAnimationClipByKeyword(animatorController, "Lower Body Hit Animation 2");
//            LowerBodyHitAnimationClip3 = GetAnimationClipByKeyword(animatorController, "Lower Body Hit Animation 3");

//            LowerBodyLeftCoverIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "LowerBody Left Cover Idle");
//            LowerBodyRightCoverIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "LowerBody Right Cover Idle");
//            UpperBodyLeftCoverIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "UpperBody Left Cover Idle");
//            UpperBodyRightCoverIdleAnimationClip = GetAnimationClipByKeyword(animatorController, "UpperBody Right Cover Idle");
//        }

//        private void SaveAnimations()
//        {
//            if (targetObject == null)
//            {
//                Debug.LogError("Target Object is not assigned.");
//                return;
//            }

//            Animator animator = targetObject.GetComponent<Animator>();
//            if (animator == null)
//            {
//                Debug.LogError("Target Object does not have an Animator component.");
//                return;
//            }

//            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
//            if (animatorController == null)
//            {
//                Debug.LogError("Target Object's Animator does not have an AnimatorController assigned.");
//                return;
//            }

//            SetAnimationClipByKeyword(animatorController, "Stand Pose", StandPoseAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Stand Weapon Recoil", StandFireAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Aiming Stand Posture", StandAimingAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Stand Reload", StandReloadAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Rifle Idle", StandIdleAnimationClip);

//            SetAnimationClipByKeyword(animatorController, "UpperBody Idle", UpperBodyIdleAnimationClip);

//            SetAnimationClipByKeyword(animatorController, "Crouch Pose", CrouchPoseAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Crouch Weapon Recoil", CrouchFireAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Crouch Reload", CrouchReloadAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Crouch Aiming", CrouchAimingAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Rifle Run", SprintingAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "StandCoverLeft", StandCoverLeftAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "StandCoverRight", StandCoverRightAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Stand Cover Neutral", StandCoverNeutralAnimationClip);

//            SetAnimationClipByKeyword(animatorController, "Run Forward", RunForwardAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Run Backward", RunBackwardAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "RunRight", RunRightAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "RunLeft", RunLeftAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "RunForwardRight", RunForwardRightAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "RunForwardLeft", RunForwardLeftAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "RunBackwardRight", RunBackwardRightAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "RunBackwardLeft", RunBackwardLeftAnimationClip);


//            SetAnimationClipByKeyword(animatorController, "Walk Right", WalkRightAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Walk Left", WalkLeftAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "WalkingForwardAiming", WalkForwardAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Walking Backwards", WalkBackwardAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Walk Forward Right", WalkForwardRightAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Walk Backward Right", WalkBackwardRightAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Walk Backward Left", WalkBackwardLeftAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "Walk Forward Left", WalkForwardLeftAnimationClip);

//            SetAnimationClipByKeyword(animatorController, "Grenade Throw", GrenadeThrowAnimationClip);

//            SetAnimationClipByKeyword(animatorController, "MeleeAttack1", MeleeAttack1AnimationClip);
//            SetAnimationClipByKeyword(animatorController, "MeleeAttack2", MeleeAttack2AnimationClip);
//            SetAnimationClipByKeyword(animatorController, "MeleeAttack3", MeleeAttack3AnimationClip);

//            SetAnimationClipByKeyword(animatorController, "Rifle Walk", WalkIdleAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "UB Scan Animation", UpperBodyScanAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "TurnBackward", TurnBackwardAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "TurnForward", TurnForwardAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "TurnLeft", TurnLeftAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "TurnRight", TurnRightAnimationClip);

//            SetAnimationClipByKeyword(animatorController, "Upper Body Hit Animation 1", UpperBodyHitAnimationClip1);
//            SetAnimationClipByKeyword(animatorController, "Upper Body Hit Animation 2", UpperBodyHitAnimationClip2);
//            SetAnimationClipByKeyword(animatorController, "Upper Body Hit Animation 3", UpperBodyHitAnimationClip3);
//            SetAnimationClipByKeyword(animatorController, "Lower Body Hit Animation 1", LowerBodyHitAnimationClip1);
//            SetAnimationClipByKeyword(animatorController, "Lower Body Hit Animation 2", LowerBodyHitAnimationClip2);
//            SetAnimationClipByKeyword(animatorController, "Lower Body Hit Animation 3", LowerBodyHitAnimationClip3);

//            SetAnimationClipByKeyword(animatorController, "LowerBody Left Cover Idle", LowerBodyLeftCoverIdleAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "LowerBody Right Cover Idle", LowerBodyRightCoverIdleAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "UpperBody Left Cover Idle", UpperBodyLeftCoverIdleAnimationClip);
//            SetAnimationClipByKeyword(animatorController, "UpperBody Right Cover Idle", UpperBodyRightCoverIdleAnimationClip);
//        }

//        private void ResetAnimations()
//        {
//            StandPoseAnimationClip = null;
//            StandFireAnimationClip = null;
//            StandAimingAnimationClip = null;
//            StandReloadAnimationClip = null;
//            StandIdleAnimationClip = null;

//            CrouchPoseAnimationClip = null;
//            CrouchFireAnimationClip = null;
//            CrouchReloadAnimationClip = null;
//            CrouchAimingAnimationClip = null;
//            SprintingAnimationClip = null;
//            StandCoverLeftAnimationClip = null;
//            StandCoverRightAnimationClip = null;
//            StandCoverNeutralAnimationClip = null;

//            RunForwardAnimationClip = null;
//            RunBackwardAnimationClip = null;
//            RunLeftAnimationClip = null;
//            RunRightAnimationClip = null;
//            RunForwardRightAnimationClip = null;
//            RunForwardLeftAnimationClip = null;
//            RunBackwardRightAnimationClip = null;
//            RunBackwardLeftAnimationClip = null;

//            WalkForwardAnimationClip = null;
//            WalkRightAnimationClip = null;
//            WalkLeftAnimationClip = null;
//            WalkBackwardAnimationClip = null;
//            WalkForwardRightAnimationClip = null;
//            WalkBackwardRightAnimationClip = null;
//            WalkBackwardLeftAnimationClip = null;
//            WalkForwardLeftAnimationClip = null;
//            GrenadeThrowAnimationClip = null;

//            MeleeAttack1AnimationClip = null;
//            MeleeAttack2AnimationClip = null;
//            MeleeAttack3AnimationClip = null;

//            WalkIdleAnimationClip = null;
//            UpperBodyIdleAnimationClip = null;

//            UpperBodyScanAnimationClip = null;
//            TurnBackwardAnimationClip = null;
//            TurnForwardAnimationClip = null;
//            TurnLeftAnimationClip = null;
//            TurnRightAnimationClip = null;

//            UpperBodyHitAnimationClip1 = null;
//            UpperBodyHitAnimationClip2 = null;
//            UpperBodyHitAnimationClip3 = null;
//            LowerBodyHitAnimationClip1 = null;
//            LowerBodyHitAnimationClip2 = null;
//            LowerBodyHitAnimationClip3 = null;

//            LowerBodyLeftCoverIdleAnimationClip = null;
//            LowerBodyRightCoverIdleAnimationClip = null;
//            UpperBodyLeftCoverIdleAnimationClip = null;
//            UpperBodyRightCoverIdleAnimationClip = null;
//        }

//        private AnimationClip GetAnimationClipByKeyword(AnimatorController animatorController, string keyword)
//        {
//            foreach (AnimatorControllerLayer layer in animatorController.layers)
//            {
//                foreach (ChildAnimatorState state in layer.stateMachine.states)
//                {
//                    if (state.state.name.Contains(keyword))
//                    {
//                        return state.state.motion as AnimationClip;
//                    }
//                }
//            }

//            return null;
//        }
//        private void SetAnimationClipByKeyword(AnimatorController animatorController, string keyword, AnimationClip animationClip)
//        {
//            foreach (AnimatorControllerLayer layer in animatorController.layers)
//            {
//                foreach (ChildAnimatorState state in layer.stateMachine.states)
//                {
//                    if (state.state.name.Contains(keyword))
//                    {
//                        state.state.motion = animationClip;
//                    }
//                }
//            }
//        }
//        //private void DuplicateAnimator()
//        //{
//        //    if (string.IsNullOrEmpty(AnimatorControllerName))
//        //    {
//        //        Debug.LogError("Animator Name is not provided.");
//        //        return;
//        //    }

//        //    AnimatorControllerName = AnimatorControllerName.Trim(); // Trim leading and trailing spaces

//        //    if (targetObject == null)
//        //    {
//        //        Debug.LogError("Target Object is not assigned.");
//        //        return;
//        //    }

//        //    Animator sourceAnimator = targetObject.GetComponent<Animator>();
//        //    if (sourceAnimator == null)
//        //    {
//        //        Debug.LogError("Target Object does not have an Animator component.");
//        //        return;
//        //    }

//        //    AnimatorController sourceController = sourceAnimator.runtimeAnimatorController as AnimatorController;
//        //    if (sourceController == null)
//        //    {
//        //        Debug.LogError("Target Object's Animator does not have an AnimatorController assigned.");
//        //        return;
//        //    }
//        //    string controllerPath = "Assets/" + AnimatorControllerName + ".controller";
//        //    AnimatorController duplicatedController = new AnimatorController();
//        //    duplicatedController.name = AnimatorControllerName;
//        //    EditorUtility.CopySerialized(sourceController, duplicatedController);
//        //    AssetDatabase.CreateAsset(duplicatedController, controllerPath);
//        //    AssetDatabase.SaveAssets();
//        //    AssetDatabase.Refresh();
//        //    Debug.Log("Animator duplicated and saved as: " + controllerPath);
//        //    sourceAnimator.runtimeAnimatorController = duplicatedController;
//        //    Debug.Log("Animator duplicated and assigned to target object: " + controllerPath);
//        //}
//    }
//}