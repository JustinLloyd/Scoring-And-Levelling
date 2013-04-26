/*
 * Scoring.cs - Tracks the player's score
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

public class Scoring : MonoBehaviour, IGameScore
{
    private const string MsgScoreAdjustedBy = "ScoreAdjustedBy";
    private const string MsgScoreAdjustedTo = "ScoreAdjustedTo";

    public event ScoreAdjustedHandler ScoreAdjusted;

    /// <summary>
    /// The player's current score.
    /// </summary>
    private int m_score;

    public int m_startingScore;

    public bool m_clampToMinimumScore;
    public int m_minimumPermittedScore;
    public bool m_clampToMaximumScore;
    public int m_maximumPermittedScore;

    private void OnScoreAdjusted(int score, int points)
    {
        SendMessage(MsgScoreAdjustedTo, score, SendMessageOptions.DontRequireReceiver);
        SendMessage(MsgScoreAdjustedBy, points, SendMessageOptions.DontRequireReceiver);
        if (ScoreAdjusted != null)
        {
            ScoreAdjusted(score, points);
        }

    }

    protected void Reset()
    {
        m_startingScore = 0;
        m_clampToMaximumScore = true;
        m_clampToMinimumScore = true;
        m_minimumPermittedScore = 0;
        m_maximumPermittedScore = int.MaxValue;
    }

    protected void Awake()
    {
        m_score = m_startingScore;
    }

    public int Score
    {
        get
        {
            return m_score;
        }

        set
        {
            int previousScore = m_score;
            m_score = value;

            OnScoreAdjusted(m_score, m_score - previousScore);
        }

    }

    /// <summary>
    /// Adjust the score by the specified number of points. Negative values
    /// will subtract points.
    /// </summary>
    /// <param name="points">Number of points to adjust the current score by.</param>
    public void AdjustScore(int points)
    {
        m_score += points;
        if (m_clampToMinimumScore == true)
        {
            m_score = Math.Max(m_score, m_minimumPermittedScore);
        }

        if (m_clampToMaximumScore == true)
        {
            m_score = Math.Min(m_score, m_maximumPermittedScore);
        }


        OnScoreAdjusted(m_score, points);
    }

}
