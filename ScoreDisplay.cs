/*
 * ScoreDisplay.cs - Simple score display
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


public class ScoreDisplay : MonoBehaviour
{
    public MonoBehaviour m_scoringScript;
    public Levelling m_levellingScript;
    public GUISkin m_skin;

    void Reset()
    {
        m_skin = null;
        m_scoringScript = null;
        m_levellingScript = null;
    }

    /// <summary>
    /// Displays the player's score in a simple user interface
    /// </summary>
    void OnGUI()
    {
        if (m_skin != null)
        {
            GUI.skin = m_skin;
        }

        if (m_scoringScript != null)
        {
            GUI.Label(new Rect(25, 25, 200, 25), String.Format("Score: {0:N0}", (m_scoringScript as IGameScore).Score));
        }

        if (m_levellingScript != null)
        {
            GUI.Label(new Rect(Screen.width - 200, 25, 200, 25), String.Format("Level: {0:N0}", m_levellingScript.Level));
        }

    }

}