﻿namespace GameLogic
{
    public enum EffectType
    {
        Kill,
        DealDamage,
        StatPlus,
        StatMinus,
        Withdraw,
        //Discard, use kill in hand instead
        Heal,
        Resurrect,
        Draw,
        Charm,
        Summon,
        Clone,
        Copy,
        GainEnergy,
        Rally,
        GainGold,

        //only for the UI and Sound FX
        Doubler,
        
        COUNT,
    }


}