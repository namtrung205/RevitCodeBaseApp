using System.Collections.Generic ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;

namespace HelperRevit ;

public static class UtSelection
{
  public static IList<Element> SelectObjectByTypes(UIDocument uiDoc)
  {
    try
    {
      // Allow the user to pick multiple objects
      IList<Reference> pickedRefs = uiDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, "Please select multiple objects");

      if (pickedRefs != null)
      {
        string elementNames = "";
        foreach (Reference pickedRef in pickedRefs)
        {
          Element element = uiDoc.Document.GetElement(pickedRef);
          elementNames += $"{element.Name}\n";
        }
        TaskDialog.Show("Selected Objects", $"You selected:\n{elementNames}");
      }
    }
    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
    {
      // Handle if user cancels the selection
      TaskDialog.Show("Cancelled", "Selection cancelled.");
    }

    return new List<Element>() ;
  }
}