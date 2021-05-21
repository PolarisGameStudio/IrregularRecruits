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


        public Event.IntEvent OnCountUp = new Event.IntEvent();

        public bool Unlocked() => Count >= UnlocksAt;

        public string Description()
        {
            return $"{Condition} (against {Against}) to unlock {UnlocksHero.name} {Count}/{UnlocksAt} ";        
        }


        public void CountUp()
        {
            Count++;

            OnCountUp.Invoke(Count);

            if (Count == UnlocksAt)
                Event.OnAchievement.Invoke(this);

        }
    }
}