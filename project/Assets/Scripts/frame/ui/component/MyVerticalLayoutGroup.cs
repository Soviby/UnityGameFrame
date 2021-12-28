using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class MyVerticalLayoutGroup : MyHorizontalOrVerticalLayoutGroup
{
    public override void CalculateLayoutInputVertical()
    {
        CalcAlongAxis(1,true);
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        CalcAlongAxis(0, true);
    }

    public override void SetLayoutHorizontal()
    {
        SetChildrenAlongAxis(0, true);
    }

    public override void SetLayoutVertical()
    {
        SetChildrenAlongAxis(1, true);
    }
}

