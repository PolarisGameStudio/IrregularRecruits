using System;

public interface IDeckController
{
     void SetupDeckActions(Deck deck, Action onfinish);
     void YourTurn();

}
