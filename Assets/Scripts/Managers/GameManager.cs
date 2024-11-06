using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    private List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
    }

    public void AddEndGameObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveEndGameObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyEndGameObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
}
