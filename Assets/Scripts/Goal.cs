using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    // the player who gets a point for this goal, 1 or 2
    public int Player = 1;

    // the Scorekeeper
    public ScoreKeeper scorekeeper;

    public void GetPoint()
    {
        // when the ball collides with this goal, give the player a point
        scorekeeper.AddScore(Player);
    }
}
