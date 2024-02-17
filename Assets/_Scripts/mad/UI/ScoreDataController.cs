using TMPro;
using UnityEngine;

namespace WGRF.UI
{
    public class ScoreDataController : MonoBehaviour
    {
        //[SerializeField] TextMeshProUGUI rankTxt;
        [SerializeField] TextMeshProUGUI nameTxt;
        [SerializeField] TextMeshProUGUI scoreTxt;

        /* public void SetRank(int rank)
        { rankTxt.SetText(rank.ToString()); } */

        public void SetName(string name)
        { nameTxt.SetText(name); }

        public void SetScore(int score)
        { scoreTxt.SetText(score.ToString()); }
    }
}