using System;
using System.Collections.Generic;
using System.Linq;

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
    
        public NounType Subjekt;
        public Verb TriggerAction;

        public Trigger(NounType subj, Verb triggerAction)
        {
            Subjekt = subj;
            TriggerAction = triggerAction;
        }

        public string Description(Card _owner) {
            switch (TriggerAction)
            {
                case Verb.ATTACKS:
                    return $"When {NounAsString(Subjekt, _owner) } attacks";
                case Verb.IsATTACKED:
                    return $"When {NounAsString(Subjekt, _owner) } is attacked";
                case Verb.KILLS:
                    return $"When {NounAsString(Subjekt, _owner) } kills a minion";
                case Verb.DIES:
                    return $"When {NounAsString(Subjekt, _owner) } dies";
                case Verb.ETB:
                    return $"When {NounAsString(Subjekt, _owner) } enters the battlefield";
                case Verb.IsDAMAGED:
                    return $"When {NounAsString(Subjekt, _owner) } is damaged";
                case Verb.IsHealed:
                    return $"When {NounAsString(Subjekt, _owner) } is healed";
                case Verb.Draw:
                    return $"When {_owner.Name}'s controller draws a card";
                case Verb.Withdraw:
                    return $"When {NounAsString(Subjekt, _owner) } is withdrawn";
                case Verb.RoundEnd:
                    return $"At the end of a combat round";
                case Verb.COUNT:
                default:
                    return $"When {NounAsString(Subjekt, _owner) } {TriggerAction}";
            }
        }

        //Higher the more often it happens
        internal float GetValue()
        {
            switch (TriggerAction)
            {
                //HAPPENS MORE THAN ONCE EACH TURN:
                case Verb.ATTACKS:
                case Verb.IsATTACKED:
                case Verb.IsDAMAGED:
                    return 3f * GetSubjectValue();
                    //Happens about once a turn, depending on subject
                case Verb.KILLS:
                case Verb.DIES:
                case Verb.ETB:
                case Verb.Draw:
                case Verb.Withdraw:
                case Verb.IsHealed:
                    return 1f * GetSubjectValue();
                case Verb.RoundEnd:
                case Verb.COUNT:
                default:
                    return 1f ;
            }
        }

        private float GetSubjectValue()
        {

            //The more common the higher the value. So each constraint subtracts the value
            var value = 1.4f;
                                   
            if (Subjekt.Relationship != Allegiance.Any)
                value -= 0.1f;
            if (Subjekt.Race != RaceType.Any)
                value -= 0.1f;
            if (Subjekt.DamageState != DamageState.Any)
                value -= 0.1f;
            if (Subjekt.Character != Character.Any)
                value -= 0.1f;

            return value;
        }
    }
    //internal bool AnyTriggerInconsistencies()
    //{
    //    return InconsistentNounVerbs.Any(a => a.Key == TriggerCondition.TriggerAction && a.Value.Contains(TriggerCondition.Subject));
    //}

}
