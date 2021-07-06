using System;

namespace GameLogic
{
    [Serializable]
    public class UnlockCondition
    {
        public HeroObject UnlocksHero;
        public UnlockConditionType Condition;
        public Race Against;
        public int Count;
        public int UnlocksAt;
        public int StartedAt;
        public bool UnlockedAtStart;

        public Event.IntEvent OnCountUp = new Event.IntEvent();

        public bool Unlocked() => Count >= UnlocksAt;

        //sets the startedAt value to the current count. To track how much have improved during a run.
        public void StartRun()
        {
            UnlockedAtStart = Unlocked();
            StartedAt = Count;
        }

        public string Description()
        {
            return $"{Condition} (against {Against}) to unlock {UnlocksHero.name} {Count}/{UnlocksAt} ";        
        }


        public void CountUp()
        {
            Count++;

            OnCountUp.Invoke(Count);


        }
    }
}