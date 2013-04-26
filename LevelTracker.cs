using UnityEngine;
using System;

[Serializable]
public class NextLevel
{
    public int ExperienceRequired;
    public AudioClip LevelUp;
}

public class LevelTracker : MonoBehaviour
{
    protected const string MsgLevelledUp = "LevelledUp";
    protected const string MsgLevelChanged = "LevelChanged";
    protected const string MsgLevelWasReset = "LevelWasReset";
    protected const string MsgLevelCapReached = "LevelCapReached";
    protected const string MsgMaximumLevelChanged = "MaximumLevelChanged";
    protected const string MsgMinimumLevelChanged = "MinimumLevelChanged";

    public ExperienceTracker m_experienceTracker;
    public delegate void LevelAdjustedHandler(int level);
    public delegate void LevelledUpHandler(int level);
    public delegate void MaximumLevelChangedHandler(int previousMaximumLevel, int maximumLevel);
    public delegate void MinimumLevelChangedHandler(int previousMinimumLevel, int minimumLevel);

    public event LevelAdjustedHandler LevelAdjusted;
    public event LevelledUpHandler LevelledUp;
    public event MaximumLevelChangedHandler MaximumLevelChanged;
    public event MinimumLevelChangedHandler MinimumLevelChanged;

    //public AudioClip[] LevelUpSounds;

    [SerializeField]
    protected int m_minimumLevel;

    [SerializeField]
    protected int m_maximumLevel;

    [SerializeField]
    protected GameObject m_targetGameObject;

    public NextLevel[] LevelProgression;
    //public int[] NextLevelExperience;

    [SerializeField]
    protected int m_linearExperienceProgression;

    [SerializeField]
    public bool PlayLeveUpSound;

    [SerializeField]
    private int m_startingLevel;

    protected int m_level;

    void Start()
    {
        if (m_experienceTracker == null)
        {
            Debug.Log("Experience Tracker was not found, trying to automatically discover it on the game object");
            m_experienceTracker = GetComponent<ExperienceTracker>();
        }

        if (m_experienceTracker == null)
        {
            Debug.LogWarning("Experience Tracker was not configured, I cannot do anything.");
            return;
        }

        m_experienceTracker.ExperienceAdjusted += ExperienceAdjusted;
    }

    protected void ExperienceAdjusted(int experience, int points)
    {
        CheckForLevelUp();
    }

    private void OnMaximumLevelChanged(int previousMaximumLevel, int maximumLevel)
    {
        if (m_targetGameObject != null)
        {
            m_targetGameObject.SendMessage(MsgMaximumLevelChanged, maximumLevel);
        }

        if (MaximumLevelChanged != null)
        {
            MaximumLevelChanged(previousMaximumLevel, maximumLevel);
        }

    }

    private void OnMinimumLevelChanged(int previousMinimumLevel, int minimumLevel)
    {
        if (m_targetGameObject != null)
        {
            m_targetGameObject.SendMessage(MsgMinimumLevelChanged, minimumLevel);
        }

        if (MinimumLevelChanged != null)
        {
            MinimumLevelChanged(previousMinimumLevel, minimumLevel);
        }

    }

    private void OnLevelledUp(int level)
    {
        if (m_targetGameObject != null)
        {
            m_targetGameObject.SendMessage(MsgLevelledUp, level);
        }

        if (LevelledUp != null)
        {
            LevelledUp(level);
        }

    }

    protected void OnLevelAdjusted(int level)
    {
        if (m_targetGameObject != null)
        {
            m_targetGameObject.SendMessage(MsgLevelChanged, level);
        }

        if (LevelAdjusted != null)
        {
            LevelAdjusted(level);
        }

    }

    void Reset()
    {
        m_minimumLevel = 1;
        m_maximumLevel = 100;
        m_targetGameObject = null;
        m_linearExperienceProgression = 2500;
        PlayLeveUpSound = true;
        m_startingLevel = 1;
        LevelProgression = new NextLevel[]
        {
            new NextLevel() { ExperienceRequired = 100, LevelUp = null },
            new NextLevel() { ExperienceRequired = 200, LevelUp = null },
            new NextLevel() { ExperienceRequired = 400, LevelUp = null },
            new NextLevel() { ExperienceRequired = 800, LevelUp = null },
            new NextLevel() { ExperienceRequired = 1600, LevelUp = null },
            new NextLevel() { ExperienceRequired = 2000, LevelUp = null },
            new NextLevel() { ExperienceRequired = 2500, LevelUp = null },
            new NextLevel() { ExperienceRequired = 3000, LevelUp = null },
            new NextLevel() { ExperienceRequired = 4000, LevelUp = null },
            new NextLevel() { ExperienceRequired = 5000, LevelUp = null },
        };

    }

    void Awake()
    {
        Level = StartingLevel;
    }

    public void ResetLevel()
    {
        m_level = StartingLevel;
        //OnLevelReset();
    }

    /// <summary>
    /// Adjust the current level by the specified number of levels. Negative
    /// values will subtract levels. Does not adjust the experience to match.
    /// The new level will be clamped to within the minimum and maximum
    /// permitted level range.
    /// </summary>
    /// <param name="levels">Number of levels to adjust the current level by.
    /// </param>
    public void AdjustLevel(int levels)
    {
        Level += levels;
    }

    public int LinearExperienceProgression
    {
        get
        {
            return m_linearExperienceProgression;
        }

        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException("Linear experience progression cannot be zero or lower.");
            }

            m_linearExperienceProgression = value;
        }

    }

    public int StartingLevel
    {
        get
        {
            return m_startingLevel;
        }
        set
        {
            if (value < MinimumLevel)
            {
                throw new ArgumentOutOfRangeException("Starting level cannot be less than minimum level.");
            }

            if (value > MaximumLevel)
            {
                throw new ArgumentOutOfRangeException("Starting level cannot be greater than maximum level.");
            }

            m_startingLevel = value;
        }

    }

    public GameObject TargetGameObject
    {
        get
        {
            return m_targetGameObject;
        }
        set
        {
            if (m_targetGameObject == value)
            {
                return;
            }

            m_targetGameObject = value;
        }

    }

    public int MinimumLevel
    {
        get
        {
            return m_minimumLevel;
        }

        protected set
        {
            if (m_minimumLevel == value)
            {
                return;
            }

            int previousMinimumLevel = m_minimumLevel;
            m_minimumLevel = value;
            OnMinimumLevelChanged(previousMinimumLevel, m_minimumLevel);
        }

    }

    public int MaximumLevel
    {
        get
        {
            return m_maximumLevel;
        }

        protected set
        {
            if (m_maximumLevel == value)
            {
                return;
            }

            int previousMaximumLevel = m_maximumLevel;
            m_maximumLevel = value;
            OnMaximumLevelChanged(previousMaximumLevel, m_maximumLevel);
        }

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

        protected set
        {
            m_level = Mathf.Clamp(value, MinimumLevel, MaximumLevel);

            OnLevelAdjusted(m_level);
        }

    }

    /// <summary>
    /// Play the audio for level up sound.
    /// </summary>
    private void PlayNextLevelSound()
    {
        //if ((!PlayLeveUpSound) || (LevelProgression == null) || (LevelProgression.Length == 0))
        //{
        //    return;
        //}

        //int levelUpIndex = Mathf.Clamp(m_level, MinimumLevel, LevelProgression.Length - 1) - 1;
        //if (LevelProgression[levelUpIndex] == null || LevelProgression[levelUpIndex] || (LevelProgression[Level]==null) || (LevelProgression[Level].LevelUp == null))
        //{
        //    return;
        //}

        //this.audio.PlayOneShot(LevelProgression[levelUpIndex]);
    }

    /// <summary>
    /// Checks for completion of the current level and advances to the next
    /// level if the experience is high enough.
    /// </summary>
    public virtual void CheckForLevelUp()
    {
        //// if we have reached the maximum level, do nothing
        //if (Level >= MaximumLevel)
        //{
        //    return;
        //}

        //// check for the next required experience
        //int nextLevelExperience = 0;
        //// if there are no more experiences in the level experience progression array
        ////      switch over to linear progression
        ////      otherwise, use the non-linear progression
        //if (Level >= NextLevelExperience.Length)
        //{
        //    nextLevelExperience = NextLevelExperience[NextLevelExperience.Length - 1] + (Level - NextLevelExperience.Length + 1) * LinearExperienceProgression;
        //}
        //else
        //{
        //    nextLevelExperience = NextLevelExperience[Level];
        //}

        //// if we have the required experience to level up, advance to the next level
        //if (m_experienceTracker.Experience >= nextLevelExperience)
        //{
        //    Level++;
        //    PlayNextLevelSound();

        //    OnLevelledUp(Level);
        //    if (Level >= MaximumLevel)
        //    {

        //    }

        //}

    }

}
