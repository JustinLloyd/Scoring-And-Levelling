/*
 * TrickleScoring.cs - Trickles the score up to the destination value
 * Copyright (C) 2010 Justin Lloyd
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU lesser General Public License
 * along with this library.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using UnityEngine;
using System;

/// <summary>
/// Tracks the player's score, increments it when necessary.
/// </summary>
[RequireComponent(typeof(Scoring))]
public class TrickleScoring : MonoBehaviour, IGameScore
{
    private const string MsgScoreAdjustedBy = "ScoreAdjustedBy";
    private const string MsgScoreAdjustedTo = "ScoreAdjustedTo";
    private const string InvokedScoreAdjustment = "ScoreAdjustment";

    public event ScoreAdjustedHandler ScoreAdjusted;
    public int[] m_unitDivisors;

    public float m_trickleStepDelay;

    public AudioClip m_trickling;
    public bool m_shouldTrickle;

    protected MonoBehaviour m_scoringScript;
    protected int m_score;
    protected int m_destinationScore;

    void Reset()
    {
        m_trickling = null;
        m_shouldTrickle = true;
        m_trickleStepDelay = 0.05f;
        m_unitDivisors = new int[] { 1000, 100, 10 };

    }

    private void OnScoreAdjusted(int score, int points)
    {
        SendMessage(MsgScoreAdjustedTo, score, SendMessageOptions.DontRequireReceiver);
        SendMessage(MsgScoreAdjustedBy, points, SendMessageOptions.DontRequireReceiver);
        if (ScoreAdjusted != null)
        {
            ScoreAdjusted(score, points);
        }

    }

    void Start()
    {
        m_scoringScript = GetComponent<Scoring>();
        if (m_scoringScript == null)
        {
            Debug.LogWarning("Scoring script not detected on game object, doing nothing");
            return;
        }

        (m_scoringScript as IGameScore).ScoreAdjusted += TargetScoreAdjusted;
        m_score = (m_scoringScript as IGameScore).Score;
    }

    public int Score
    {
        get
        {
            return m_score;
        }

    }

    private void TargetScoreAdjusted(int score, int points)
    {
        m_destinationScore = score;
        if (m_shouldTrickle == false)
        {
            m_score = score;
            OnScoreAdjusted(score, points);
        }
        else if (m_score != score)
        {
            m_destinationScore = score;
            Invoke(InvokedScoreAdjustment, 0.0f);
        }

    }

    private void AdjustScoreBy(int points)
    {
        m_score += points;
        audio.PlayOneShot(m_trickling);
        Invoke(InvokedScoreAdjustment, m_trickleStepDelay);
        OnScoreAdjusted(m_score, points);
    }

    void ScoreAdjustment()
    {
        int pointDifference = m_destinationScore - m_score;
        int absolutePointDifference = Math.Abs(pointDifference);
        int sign = Math.Sign(pointDifference);
        if (absolutePointDifference == 0)
        {
            return;
        }

        foreach (int divisor in m_unitDivisors)
        {
            if (absolutePointDifference > divisor)
            {
                AdjustScoreBy(divisor * sign);
                return;
            }

        }

        AdjustScoreBy(pointDifference);
    }


}
