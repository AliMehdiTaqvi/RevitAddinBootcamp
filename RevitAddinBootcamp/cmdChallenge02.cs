using System.ComponentModel.DataAnnotations;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;



namespace RevitAddinBootcamp
{
    [Transaction(TransactionMode.Manual)]
    public class cmdChallenge02 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            //1. ASK USER TO SELECT THE ELEMENTS ON SCREEN

            TaskDialog.Show("Select Lines", "Select some Lines to convert to Revit Elements");

            List<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select Some Elements").ToList();

            // 2. FILTER CURVE ELEMENTS FROM THE SELECTED ITEMS

            List<CurveElement> filteredList = new List<CurveElement>();

            foreach (Element element in pickList)

            {
                if (element is CurveElement)
                {

                    filteredList.Add(element as CurveElement);
                }

            }

            // 3. GET LEVEL

            View curview = doc.ActiveView;

            Parameter curLevel = curview.LookupParameter("Associated Level");

            String levelName = curLevel.AsString();
            ElementId levelId = curLevel.AsElementId();

            Level doclevel =  GetLevelByName(doc, levelName);


            // 4. GET TYPES

            WallType wallType = GetWallTypeByName(doc, "Storefront");
            WallType wallType2 = GetWallTypeByName(doc, "Generic - 8\"");
            MEPSystemType ductSystem = GetSystemTypeByName(doc, "Supply Air");
            MEPSystemType PipeSystem = GetSystemTypeByName(doc, "Domestic Hot Water");
            DuctType ductType = GetDuctTypeByName(doc, "Default");
            PipeType pipeType = GetPipeTypeByName(doc, "Default");


            // 5. LOOP THROUGH CURVE

            using (Transaction T = new Transaction(doc)) 


            {
            T.Start("Create Elements");

                foreach (CurveElement currentCurve in filteredList)
                
                {
                
                 // 6. GET GRAPHICS STYLE
                 GraphicsStyle currentStyle = currentCurve.LineStyle as GraphicsStyle;
                 String lineStylename = currentCurve.Name;

                 // 6b. GET CURVE GEOMETRY
                 Curve curveGeo= currentCurve.GeometryCurve ;

                 // 7. USE SWITH STATEMENT TO CREATE ELEMENTS

                    switch(lineStylename)

                    {
                        case "A-GLAZ":
                            Wall wall1 = Wall.Create(doc, curveGeo, wallType.Id, doclevel.Id, 20, 0, false, false);
                            break;

                        case "A-WALL":
                            Wall wall2 = Wall.Create(doc, curveGeo, wallType2.Id, doclevel.Id, 20, 0, false, false);
                            break;

                        case "M-DUCT":
                            Duct duct = Duct.Create(doc, ductSystem.Id, ductType.Id,doclevel.Id, curveGeo.GetEndPoint(0), curveGeo.GetEndPoint(1));
                            break;

                        case "P-PIPE":
                            Pipe pipe = Pipe.Create(doc, PipeSystem.Id, pipeType.Id, doclevel.Id, curveGeo.GetEndPoint(0), curveGeo.GetEndPoint(1));
                            break;

                            default:
                            break;
                    }

                   

                }

                T.Commit();
            }


            return Result.Succeeded;

        }

        internal Level GetLevelByName(Document doc, string levelName)
        {
            FilteredElementCollector levelCollector = new FilteredElementCollector(doc);
            levelCollector.OfCategory(BuiltInCategory.OST_Levels);
            levelCollector.WhereElementIsNotElementType();

          

            foreach (Level cLevel in levelCollector)

            {

                if (cLevel.Name == levelName)

                {
                    return cLevel;

                }
               
            }
            return null;
        }


        internal WallType GetWallTypeByName(Document doc, string wallName)
        {
            FilteredElementCollector WallCollector = new FilteredElementCollector(doc);
            WallCollector.OfCategory(BuiltInCategory.OST_Walls);
            WallCollector.WhereElementIsElementType();



            foreach (WallType cWall in WallCollector)

            {

                if (cWall.Name == wallName)

                {
                    return cWall;

                }

            }
            return null;
        }

        internal MEPSystemType GetSystemTypeByName(Document doc, string systemName)
        {
            FilteredElementCollector systemCollector = new FilteredElementCollector(doc);
            systemCollector.OfClass(typeof(MEPSystemType));



            foreach (MEPSystemType cSystem in systemCollector)

            {

                if (cSystem.Name == systemName)

                {
                    return cSystem;

                }

            }
            return null;
        }

        internal DuctType GetDuctTypeByName(Document doc, string ductName)
        {
            FilteredElementCollector DuctCollector = new FilteredElementCollector(doc);
            DuctCollector.OfClass(typeof(DuctType));



            foreach (DuctType cDuct in DuctCollector)

            {

                if (cDuct.Name == ductName)

                {
                    return cDuct;

                }

            }
            return null;
        }

        internal PipeType GetPipeTypeByName(Document doc, string PipeName)
        {
            FilteredElementCollector PipeCollector = new FilteredElementCollector(doc);
            PipeCollector.OfClass(typeof(PipeType));



            foreach (PipeType cPipe in PipeCollector)

            {

                if (cPipe.Name == PipeName)

                {
                    return cPipe;

                }

            }
            return null;
        }


        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnChallenge02";
            string buttonTitle = "Module\r02";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Module02,
                Properties.Resources.Module02,
                "Module 02 Challenge");

            return myButtonData.Data;
        }
    }

}
