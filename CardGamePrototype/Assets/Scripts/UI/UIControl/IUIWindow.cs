using System.Collections.Generic;
using UnityEngine;


namespace UI
{
    public interface IUIWindow 
    {
        //List<IUIWindow> Children { get; }
        //IUIWindow Parent { get;  }

        CanvasGroup GetCanvasGroup();
        GameObject GetHolder();
        //higher means in front of other
        int GetPriority();

        //These should only be called by UIController
        //void Open();
        //void Close();


        //void SetParent(IUIWindow parent);

    }

}