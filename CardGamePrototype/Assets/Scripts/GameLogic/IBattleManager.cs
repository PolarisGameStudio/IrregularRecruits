namespace GameLogic
{
    //responsible for keeping track of decks currently in combat and setting up actions 
    public interface IBattleManager
    {
        //TODO: should it have a combat finished action? which parses the result?
        //Should the battle manager or the caller generate the deck from CR + Races
        void ResolveCombat(Deck playerDeck, Deck enemyDeck);
    }
}