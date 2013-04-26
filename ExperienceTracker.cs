using UnityEngine;
using System;

public class ExperienceTracker : MonoBehaviour
{
    private const string MsgExperienceAdjustedBy = "ExperienceAdjustedBy";
    private const string MsgExperienceAdjustedTo = "ExperienceAdjustedTo";
    private const string MsgExperienceReachedMaximum = "ExperienceReachedMaximum";
    private const string MsgExperienceReachedMinimum = "ExperienceReachedMinimum";
    private const string MsgExperienceWasReset = "ExperienceWasReset";

    public delegate void ExperienceAdjustedHandler(int experience, int points);
    public delegate void ExperienceReachedMaximumHandler(int previousExperience, int currentExperience, int overage);
    public delegate void ExperienceReachedMinimumHandler(int previousExperience, int currentExperience, int underage);

    public event ExperienceAdjustedHandler ExperienceAdjusted;
    public event Action ExperienceWasReset;
    public event ExperienceReachedMaximumHandler ExperienceReachedMaximum;
    public event ExperienceReachedMinimumHandler ExperienceReachedMinimum;

    private int m_experience;

    public bool m_experienceCanDecrease;
    public int m_startingExperience;
    public bool m_clampToMinimumExperience;
    public int m_minimumPermittedExperience;
    public bool m_clampToMaximumExperience;
    public int m_maximumPermittedExperience;
    public GameObject m_sendMessageTo;

    private void OnExperienceReset()
    {
        m_sendMessageTo.SendMessage(MsgExperienceWasReset, SendMessageOptions.DontRequireReceiver);
        if (ExperienceWasReset != null)
        {
            ExperienceWasReset();
        }

    }

    private void OnExperienceReachedMaximum(int previousExperience, int currentExperience, int overage)
    {
        m_sendMessageTo.SendMessage(MsgExperienceReachedMaximum, SendMessageOptions.DontRequireReceiver);
        if (ExperienceReachedMaximum != null)
        {
            ExperienceReachedMaximum(previousExperience, currentExperience, overage);
        }

    }

    private void OnExperienceReachedMinimum(int previousExperience, int currentExperience, int underage)
    {
        m_sendMessageTo.SendMessage(MsgExperienceReachedMinimum, SendMessageOptions.DontRequireReceiver);
        if (ExperienceReachedMinimum != null)
        {
            ExperienceReachedMinimum(previousExperience, currentExperience, underage);
        }

    }

    private void OnExperienceAdjusted(int score, int points)
    {
        SendMessage(MsgExperienceAdjustedTo, score, SendMessageOptions.DontRequireReceiver);
        SendMessage(MsgExperienceAdjustedBy, points, SendMessageOptions.DontRequireReceiver);
        if (ExperienceAdjusted != null)
        {
            ExperienceAdjusted(score, points);
        }

    }

    void Reset()
    {
        m_experienceCanDecrease = true;
        m_startingExperience = 0;
        m_clampToMaximumExperience = true;
        m_clampToMinimumExperience = true;
        m_minimumPermittedExperience = 0;
        m_maximumPermittedExperience = int.MaxValue;
    }

    public void ResetExperience()
    {
        m_experience = m_startingExperience;
        OnExperienceReset();
    }

    void Awake()
    {
        ResetExperience();
    }

    public int Experience
    {
        get
        {
            return m_experience;
        }

        set
        {
            if (m_experience == value)
            {
                return;
            }

            if (value < 0 && m_experienceCanDecrease)
            {
                throw new ArgumentOutOfRangeException("Experience cannot be taken away when the Experience Can Decrease flag is not set.");
            }

            int previousExperience = m_experience;
            m_experience = value;

            OnExperienceAdjusted(m_experience, m_experience - previousExperience);
        }

    }

    public void AdjustExperience(int points)
    {
        if (points < 0 && m_experienceCanDecrease)
        {
            throw new ArgumentOutOfRangeException("Experience cannot be taken away when the Experience Can Decrease flag is not set.");
        }

        int previousExperience = m_experience;
        m_experience += points;
        if (m_clampToMinimumExperience == true)
        {
            m_experience = Math.Max(m_experience, m_minimumPermittedExperience);
        }

        if ((previousExperience > m_minimumPermittedExperience) && (m_experience <= m_minimumPermittedExperience))
        {
            OnExperienceReachedMinimum(previousExperience, m_experience, Mathf.Abs(m_experience - m_minimumPermittedExperience));
        }

        if (m_clampToMaximumExperience == true)
        {
            m_experience = Math.Min(m_experience, m_maximumPermittedExperience);
        }

        if ((previousExperience < m_maximumPermittedExperience) && (m_experience >= m_maximumPermittedExperience))
        {
            OnExperienceReachedMaximum(previousExperience, m_experience, Mathf.Abs(m_experience-m_maximumPermittedExperience));
        }

        OnExperienceAdjusted(m_experience, points);
    }

}
