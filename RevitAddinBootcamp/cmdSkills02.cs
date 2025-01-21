namespace RevitAddinBootcamp
{
    [Transaction(TransactionMode.Manual)]
    public class cmdSkills02 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // 1a. PICK SINGLE ELEMENT
            // REFERENCE IS DATATYPE USED TO ASSIGN REFERENCE TO A VARIABLE SO IT CAN BE USED BY PICK ELEMENT
            Reference pickRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select Element");

            // ELEMENT IS GENEREAL DATATYPE DOC.GETELEMT METHOD WILL USED PICKREFERENCE VARIABLE FROM LINE ABOVE
            Element pickElement = doc.GetElement(pickRef);


            // 1B. PICK MULTIPLE ELEMENTS

            
            List<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select Elements").ToList();

            // 2.FILTER SELECTED ELEMENTS

            //List<CurveElement> allCurves = new List<CurveElement>();
            //foreach (Element curve in pickList)
            //{
            //    if(curve is CurveElement) 
            //    { 
            //        allCurves.Add(curve as CurveElement);
            //    }
            //}

            // 2b . SELECT ONLY MODEL CURVES

            List<CurveElement> modelCurve = new List<CurveElement>();
            foreach (Element curve in pickList)
            {
                if (curve is CurveElement)
                {
                    CurveElement curveElement = (CurveElement)curve;

                    if (curveElement.CurveElementType == CurveElementType.ModelCurve)
                    {
                        modelCurve.Add(curveElement as CurveElement);
                    }
                }

            }

            
              
            // EXTRACT CURVE DATA

            foreach (CurveElement currentCurve in modelCurve)
            {
                Curve curve = currentCurve.GeometryCurve;
                XYZ endpoint = curve.GetEndPoint(1);
                XYZ Startpoint = curve.GetEndPoint(0);


            }

            using (Transaction t = new Transaction(doc)) 

            {
                t.Start("Create wall");

                // 4. CREATE WALL

                Level reflevel = Level.Create(doc, 0);

                Curve refcurve = modelCurve[0].GeometryCurve;

                Wall newWall = Wall.Create(doc, refcurve, reflevel.Id, false);

                t.Commit();

            }


            TaskDialog.Show("Test", $"I Selected {modelCurve.Count} elements");
            return Result.Succeeded;
        }
    }

}
