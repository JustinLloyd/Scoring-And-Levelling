/*
 * Levelling.cs - Tracks which level the game is on
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



[RequireComponent(typeof(AudioSource))]
public class Levelling : MonoBehaviour
{
    private const string MsgLevelledUp = "LevelledUp";
    private const string MsgLevelAdjusted = "LevelAdjusted";
    public delegate void LevelAdjustedHandler(int level);
    public delegate void LevelledUpHandler(int level);

    public event LevelAdjustedHandler LevelAdjusted;
    public event LevelledUpHandler LevelledUp;

    /// <summary>
    /// Audio clips to play for each level up sound.
    /// </summary>
    public AudioClip[] m_nextLevelSound;

    /// <summary>
    /// Maximum permitted level.
    /// </summary>
    public int m_maximumLevel;

    /// <summary>
    /// The list of scores required to advance to the next level.
    /// </summary>
    public int[] m_nextLevelScore;

    /// <summary>
    /// The number of required points to score to advance to the next level once the score has gone beyond the provided list of points.
    /// </summary>
    public int m_nextLevelScoreProgression;

    /// <summary>
    /// The player's current level.
    /// </summary>
    private int m_level;

    public MonoBehaviour m_scoring;
    /// <summary>
    /// The minimum level permitted.
    /// </summary>
    private const int MinimumLevel = 1;

    public int m_startingLevel;

    private void OnLevelledUp(int level)
    {
        SendMessage(MsgLevelledUp, level, SendMessageOptions.DontRequireReceiver);
        if (LevelledUp != null)
        {
            LevelledUp(level);
        }

    }
    private void OnLevelAdjusted(int level)
    {
        SendMessage(MsgLevelAdjusted, level, SendMessageOptions.DontRequireReceiver);
        if (LevelAdjusted != null)
        {
            LevelAdjusted(level);
        }

    }

    protected void Reset()
    {
        m_maximumLevel = 100;
        m_nextLevelScoreProgression = 100000;
        m_startingLevel = 1;
        m_nextLevelScore = new int[] { 0, 3000, 7000, 12000, 18000, 25000, 34000, 44000, 56000, 69000, 80000 };
    }

    void Awake()
    {
        m_level = m_startingLevel;
    }

    void Start()
    {
        (m_scoring as IGameScore).ScoreAdjusted += ScoreAdjusted;
    }

    void ScoreAdjusted(int score, int points)
    {
        CheckForLevelUp();
    }

    /// <summary>
    /// Adjust the current level by the specified number of levels. Negative
    /// values will subtract levels. Does not adjust the score to match. The
    /// new level will be clamped to within the maximum permitted level.
    /// </summary>
    /// <param name="levels">Number of levels to adjust the current level by.</param>
    public void AdjustLevel(int levels)
    {
        m_level = Mathf.Clamp(m_level + levels, MinimumLevel, m_maximumLevel);

        OnLevelAdjusted(m_level);
    }

    /// <summary>
    /// The player's current level. Specifying a new level will ensure that the
    /// new level is clamped to the maximum permitted level.
    /// </summary>
    public int Level
    {
        get
        {
            return (m_level);
        }

        set
        {
            m_level = Mathf.Clamp(value, MinimumLevel, m_maximumLevel);

            OnLevelAdjusted(m_level);
        }

    }

    /// <summary>
    /// Play the audio for level up sound.
    /// </summary>
    private void PlayNextLevelSound()
    {
        if ((m_nextLevelSound == null) || (m_nextLevelSound.Length == 0))
        {
            return;
        }

        int levelUpIndex = Mathf.Clamp(m_level, MinimumLevel, m_nextLevelSound.Length - 1) - 1;
        if (m_nextLevelSound[levelUpIndex] == null)
        {
            return;
        }

        this.audio.PlayOneShot(m_nextLevelSound[levelUpIndex]);
    }

    void ScoreAdjusted()
    {
        CheckForLevelUp();
    }

    /// <summary>
    /// Checks for completion of the current level and advances to the next
    /// level if the score is high enough.
    /// </summary>
    public virtual void CheckForLevelUp()
    {
        // if we have reached the maximum level, do nothing
        if (m_level >= m_maximumLevel)
        {
            return;
        }

        // check for the next required score
        int nextLevelScore = 0;
        // if there are no more scores in the level score progression array
        //      switch over to linear progression
        //      otherwise, use the non-linear progression
        if (m_level >= m_nextLevelScore.Length)
        {
            nextLevelScore = (m_level - m_nextLevelScore.Length + 1) * m_nextLevelScoreProgression;
        }
        else
        {
            nextLevelScore = m_nextLevelScore[m_level];
        }

        // if we have the required score to level up, advance to the next level
        if ((m_scoring as IGameScore).Score >= nextLevelScore)
        {
            m_level = Math.Min(m_level + 1, m_maximumLevel);
            PlayNextLevelSound();

            OnLevelledUp(m_level);
        }

    }

}
