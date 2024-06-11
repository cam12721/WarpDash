using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> Names;
    [SerializeField]
    private List<TextMeshProUGUI> Scores;

    private string publicLeaderboardKey =
        "6e4cf289c63f284b6cd6e9ec2992d8c80cfec7926f2a248462924bb6c9a33fe5";

    private void Start()
    {
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) =>
        {
            int loopLength = (msg.Length < Names.Count) ? msg.Length : Names.Count;
            for (int i = 0; i < loopLength; i++)
            {
                Names[i].text = msg[i].Username;
                Scores[i].text = msg[i].Score.ToString();
            }
        }));
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, ((msg) =>
        {
            GetLeaderboard();
        }));
    }
}
