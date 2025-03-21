using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class TeamMatch : MonoBehaviour
    {
        public static TeamMatch instance;

        [Tooltip("Enabling This will Make the Teams To Score Max To win The Game")]
        public bool EnableScoreSystemBetweenTeamsAsWinCondition = false;

        [Tooltip("For How Long The Match Will Run")]
        public bool EnableMatchTime;
        public float MatchTime;
        public Text MatchTimeText;
        public Text WinText;

        [Tooltip("First Team To Reach The Max Score Will Regarded as a winner")]
        public bool UsingMaxScoreReachedAsWinningCondition;
        public int MaxScoreToReach = 500;

        public int HeadShotPoints;
        public int SingleShotPoints;

        [System.Serializable]
        public class Team
        {
            public string TeamName;
            [Tooltip("Debug Team Score")]
            public int TeamScore;
            [Tooltip("Show Headshots Taken By Team")]
            public int HeadshotsTaken;
            [Tooltip("Show How much Kills Done By Team")]
            public int Kills;
            [Tooltip("Text To Show The Score")]
            public Text ScoreText;
        }
        public List<Team> Teams = new List<Team>();
        bool CheckOnce = false;

        [HideInInspector]
        public List<int> Score = new List<int>();
        int HighestScorer;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        private void Update()
        {
            if (EnableMatchTime == true)
            {
                MatchTime -= Time.deltaTime;
                string minutes = Mathf.Floor(MatchTime / 60).ToString("00:");
                string seconds = (MatchTime % 60).ToString("00");
                MatchTimeText.text = minutes + " " + seconds.ToString();

                if (MatchTime <= 0f)
                {
                    if (CheckOnce == false)
                    {
                        CheckScore();
                        Score.Sort();
                        HighestScorer = Score[Score.Count - 1];
                        UpdateName();
                        CheckOnce = true;
                    }
                }
            }

            if (UsingMaxScoreReachedAsWinningCondition == true)
            {
                if (CheckOnce == false)
                {
                    MaxReachScores();
                }
            }
        }
        void CheckScore()// Check For Scores
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                Score.Add(Teams[i].TeamScore);
            }
        }
        void UpdateName() // Update Winning Team Name
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                if (Teams[i].TeamScore == HighestScorer)
                {
                    WinText.text = Teams[i].TeamName + " " + Teams[i].TeamScore.ToString() + " " + "WINS";
                }
            }
        }
        void MaxReachScores() // Check Which Team wins
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                if (Teams[i].TeamScore >= MaxScoreToReach)
                {
                    WinText.text = Teams[i].TeamName + " " + "WINS";
                    CheckOnce = true;
                }
            }
        }
    }
}