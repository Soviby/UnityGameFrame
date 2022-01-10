
using System.Collections.Generic;
public class MyUIItemData
{
    private List<MyUIItem> _uiItemList = new List<MyUIItem>();

    public void AddUIItem(MyUIItem item)
    {
        _uiItemList.Add(item);
    }

    public void ClearUIItem()
    {

        foreach (var item in _uiItemList)
        {
            item?.Close();
        }
        _uiItemList.Clear();
    }



}
