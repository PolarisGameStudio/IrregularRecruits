using System;
namespace GameLogic
{
    public partial class Ability
    {

        //Also includes redundant statements like heal damaged
        //TODO: should be moved to a value in Verb Classes
        //public static Dictionary<Verb,List<Noun>> InconsistentNounVerbs = new Dictionary<Verb, List<Noun>>
        //{
        //    { Verb.KILLS,new List<Noun>() { Noun.IT, Noun.CardInOwnersDeck,Noun.CardInOwnersHand,Noun.CardInOpponentsDeck,Noun.CardInOpponentsHand,Noun.COUNT } },
        //    { Verb.IsHealed,new List<Noun>() {Noun.UNDAMAGED, Noun.DAMAGED,Noun.IT,Noun.CardInOwnersDeck, Noun.CardInOwnersHand, Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand,Noun.COUNT } },
        //    { Verb.ATTACKS,new List<Noun>() {Noun.IT,Noun.CardInOwnersDeck, Noun.CardInOwnersHand, Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand,Noun.COUNT } },
        //    { Verb.IsDAMAGED,new List<Noun>() {Noun.DAMAGED,Noun.UNDAMAGED,Noun.IT,Noun.CardInOwnersDeck, Noun.CardInOwnersHand, Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand,Noun.COUNT } },
        //    { Verb.DIES,new List<Noun>() {Noun.UNDAMAGED, Noun.DAMAGED,Noun.IT,Noun.CardInOwnersDeck, Noun.CardInOwnersHand, Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand,Noun.COUNT } },
        //    { Verb.ETB,new List<Noun>() {Noun.IT,Noun.CardInOwnersDeck, Noun.CardInOwnersHand, Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand,Noun.COUNT } },
        //    { Verb.IsATTACKED,new List<Noun>() {Noun.IT,Noun.CardInOwnersDeck, Noun.CardInOwnersHand, Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand,Noun.COUNT } },
        //    { Verb.Withdraw,new List<Noun>() {Noun.IT,Noun.CardInOwnersDeck, Noun.CardInOwnersHand, Noun.CardInOpponentsDeck, Noun.CardInOpponentsHand,Noun.COUNT } },
        //};

        [Serializable]
        public struct Trigger
        {

            public Noun Subjekt;
            public Verb TriggerAction;

            public Trigger(Noun subj, Verb triggerAction)
            {
                Subjekt = subj;
                TriggerAction = triggerAction;
            }

            public string Description(Creature _owner)
            {
                return AbilityProcessor.GetTrigger(TriggerAction).Description(Subjekt.NounAsString(_owner));

            }

            //Higher the more often it happens
            internal float GetValue()
            {
                return AbilityProcessor.GetTrigger(TriggerAction).GetValue() * GetSubjectValue();

            }

            private float GetSubjectValue()
            {

                //The more common the higher the value. So each constraint subtracts the value
                var value = 1.4f;

                if (Subjekt.Relationship != Noun.Allegiance.Any)
                    value -= 0.1f;
                if (Subjekt.Race != Noun.RaceType.Any)
                    value -= 0.1f;
                if (Subjekt.DamageState != Noun.DamageType.Any)
                    value -= 0.1f;
                if (Subjekt.Character != Noun.CharacterTyp.Any)
                    value -= 0.1f;

                return value;
            }
        }

        public void FixTriggerInconsistencies()
        {
            //TODO: fix inconsistencies

        }

        //internal bool AnyTriggerInconsistencies()
        //{
        //    return InconsistentNounVerbs.Any(a => a.Key == TriggerCondition.TriggerAction && a.Value.Contains(TriggerCondition.Subject));
        //}

    }
}