using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class MyHorizontalLayoutGroup : MyHorizontalOrVerticalLayoutGroup
{
    public override void CalculateLayoutInputVertical()
    {
        CalcAlongAxis(1,false);
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        CalcAlongAxis(0, false);
    }

    public override void SetLayoutHorizontal()
    {
        SetChildrenAlongAxis(0,false);
    }

    public override void SetLayoutVertical()
    {
        SetChildrenAlongAxis(1,false);
    }
}

