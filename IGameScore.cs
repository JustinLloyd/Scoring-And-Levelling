using System;

public interface IGameScore
{
    int Score { get; }
    event ScoreAdjustedHandler ScoreAdjusted;

}
