using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.BattleDeck
{
    [System.Serializable]
    public class Action
    {

        public enum Type
        {
            Attack,
            Defense,
            MoveUp,
            MoveBack,
            Wait
        }

        public readonly Type type;
        Battalion battalion;
        __Engine.CardSymbol symbol = __Engine.CardSymbol.sword;

        public Action(Type type, Battalion battalion, __Engine.CardSymbol symbol)
        {
            this.type = type;
            this.battalion = battalion;
            this.symbol = symbol;
        }

        public Battalion GetBattalion()
        {
            return battalion;
        }

        public int GetValue()
        {
            if(type == Type.Attack || type == Type.Defense)
            {
                return battalion.GetStatByCardSymbol(symbol);
            } else
            {
                return 0;
            }
            
        }

        public __Engine.CardSymbol GetSymbol()
        {
            return symbol;
        }

        public override string ToString()
        {
            return battalion.name + " " + type.ToString() + " action (" + symbol.ToString() + ")";
        }



    }

    
}