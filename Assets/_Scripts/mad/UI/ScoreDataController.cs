using TMPro;
using UnityEngine;

namespace WGRF.UI
{
    ///<summary>A simple data container for the UI score elements</summary>
    public class ScoreDataController : MonoBehaviour
    {
        ///<summary>*Future feature*: Display rank</summary>
        //[SerializeField] TextMeshProUGUI rankTxt;
        ///<summary>The name text</summary>
        [SerializeField, Tooltip("The name text")] TextMeshProUGUI nameTxt;
        ///<summary>The score text</summary>
        [SerializeField, Tooltip("The score text")] TextMeshProUGUI scoreTxt;

        /* public void SetRank(int rank)
        { rankTxt.SetText(rank.ToString()); } */

        /// <summary>
        /// Sets the player name of the data container
        /// </summary>
        public void SetName(string name)
        { nameTxt.SetText(name); }

        /// <summary>
        /// Sets the player score of the data container
        /// </summary>
        public void SetScore(int score)
        { scoreTxt.SetText(score.ToString()); }
    }
}