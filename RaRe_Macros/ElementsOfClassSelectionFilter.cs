#region Extra Helper Class
public class JtElementsOfClassSelectionFilter<T> : ISelectionFilter where T : Element
{
    public bool AllowElement(Element e)
    {
        return e is T;
    }
    
    public bool AllowReference(Reference r,  XYZ p)
    {
        return true;
    }
    
    bool CompareCategoryToTargetList(Element e)
    {
        bool rc = null != e.Category;
        if(rc)
        {
            int[] targets = new int[]
            {
                (int) BuiltInCategory.OST_StructuralColumns,
                (int) BuiltInCategory.OST_StructuralFraming,
                (int) BuiltInCategory.OST_Walls
            };
            int icat = e.Category.Id.IntegerValue;
            rc = targets.Any<int>(i => i.Equals(icat));
        }
        return rc;
    }
}
#endregion