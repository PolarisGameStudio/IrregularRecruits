
namespace GameLogic
{
    //TODO: move amount to noun as well
    [System.Serializable]
    public struct Noun
    {
        public CharacterTyp Character;
        public Deck.Zone Location;
        public Allegiance Relationship;
        public DamageType DamageState;
        public RaceType Race;
        public enum CharacterTyp { Any, This, Other, It }
        public enum Allegiance { Any, Friend, Enemy };
        public enum DamageType { Any, Damaged, Undamaged };
        public enum RaceType { Any, Same, Different };


        public Noun(CharacterTyp character = CharacterTyp.Any, Allegiance relationship = Allegiance.Any, DamageType damageState = DamageType.Any, RaceType race = RaceType.Any, Deck.Zone location = Deck.Zone.Battlefield)
        {
            Character = character;
            Location = location;
            Relationship = relationship;
            DamageState = damageState;
            Race = race;
        }
        public bool CorrectNoun(Card instigator, Card abilityOwner)
        {
            //TODO: THIs should be a more complex check depending on the ability. Now this + etb won't trigger for instance
            // maybe all abilities should have a locations allowed for onwer
            if (abilityOwner.Location != Deck.Zone.Battlefield) return false;

            //Trigger Actions without instigators always has correct noun
            if (instigator == null) return true;

            return
                CorrectCharacter(instigator, abilityOwner) &&
                CorrectRace(instigator, abilityOwner) &&
                CorrectAllegiance(instigator, abilityOwner) &&
                CorrectDamageState(instigator) &&
                instigator.Location == this.Location;
        }

        public bool CorrectCharacter(Card instigator, Card abilityOwner, Card triggerExecuter = null)
        {
            switch (Character)
            {
                case CharacterTyp.Any:
                    return true;
                case CharacterTyp.This:
                    return instigator == abilityOwner;
                case CharacterTyp.Other:
                    return instigator != abilityOwner;
                case CharacterTyp.It:
                    return triggerExecuter != null && instigator == triggerExecuter;
                default:
                    return true;
            }
        }

        public bool CorrectDamageState(Card instigator)
        {
            switch (DamageState)
            {
                case DamageType.Any:
                    return true;
                case DamageType.Damaged:
                    return instigator.CurrentHealth < instigator.MaxHealth;
                case DamageType.Undamaged:
                    return instigator.CurrentHealth < instigator.MaxHealth;
                default:
                    return true;
            }
        }
        public bool CorrectRace(Card instigator, Card abilityOwner)
        {
            switch (Race)
            {
                case RaceType.Any:
                    return true;
                case RaceType.Same:
                    return instigator.Creature.Race == abilityOwner.Creature.Race;
                case RaceType.Different:
                    return instigator.Creature.Race == abilityOwner.Creature.Race;
                default:
                    return true;
            }
        }
        public bool CorrectAllegiance(Card instigator, Card abilityOwner)
        {
            if (instigator.InDeck == null || abilityOwner.InDeck == null)
                return false;

            switch (Relationship)
            {
                case Allegiance.Any:
                    return true;
                case Allegiance.Friend:
                    return instigator.InDeck == abilityOwner.InDeck;
                case Allegiance.Enemy:
                    return instigator.InDeck != abilityOwner.InDeck;
                default:
                    return true;
            }
        }

        public string NounAsString(Creature _owner, Ability.Count count = Ability.Count.One)
        {
            var str = "";

            switch (Character)
            {
                case CharacterTyp.This:
                    return _owner.name;
                case CharacterTyp.It:
                    return "it";
                case CharacterTyp.Any:
                    if (count == Ability.Count.One)
                        str += "a ";
                    else
                        str += count.ToString() + " ";
                    break;
                case CharacterTyp.Other:
                    if (count == Ability.Count.One)
                        str += "another ";
                    else
                        str += count.ToString() + " other ";
                    break;
            }

            switch (DamageState)
            {
                case DamageType.Any:
                    break;
                case DamageType.Damaged:
                    str += "damaged ";
                    break;
                case DamageType.Undamaged:
                    str += "damaged ";
                    break;
            }
            switch (Relationship)
            {
                case Allegiance.Any:
                    break;
                case Allegiance.Friend:
                    str += "friendly ";
                    break;
                case Allegiance.Enemy:
                    str += "enemy ";
                    break;
            }

            switch (Race)
            {
                case RaceType.Any:
                    str += "minion" + (count == Ability.Count.One ? "" : "s");
                    break;
                case RaceType.Same:
                    str += _owner.Race?.name + (count == Ability.Count.One ? "" : "s");
                    break;
                case RaceType.Different:
                    str += "non-" + _owner.Race?.name + (count == Ability.Count.One ? "" : "s");
                    break;
            }

            switch (Location)
            {
                case Deck.Zone.Library:
                    switch (Relationship)
                    {
                        case Allegiance.Friend:
                            return $"{str.Replace("friendly ", "")} in {_owner.name}'s deck";
                        case Allegiance.Enemy:
                            return $"{str.Replace("enemy ", "")} in the enemy deck";
                        case Allegiance.Any:
                        default:
                            return $"{str} in a deck";
                    }
                case Deck.Zone.Battlefield:
                    return str;
                case Deck.Zone.Graveyard:
                    return str + ", that are dead"; //TODO: remove. just for debugging now
                case Deck.Zone.Hand:
                    switch (Relationship)
                    {
                        case Allegiance.Friend:
                            return $"{str.Replace("friendly ", "")} in {_owner.name}'s hand";
                        case Allegiance.Enemy:
                            return $"{str.Replace("enemy ", "")} in the enemy hand";
                        case Allegiance.Any:
                        default:
                            return $"{str} in a hand";
                    }

                default:
                    return str;
            }


        }

    }

}