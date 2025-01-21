namespace RevitAddinBootcamp
{
    [Transaction(TransactionMode.Manual)]
    public class cmdSkills01 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction t = new Transaction(doc);
            t.Start("Lets go");

            //Create a level
            Level Lvl = Level.Create(doc,10);
            //Rename the level
            Lvl.Name = "My New Level";

            //Collect all elements of view family type

        FilteredElementCollector collector1 = new FilteredElementCollector(doc);
            collector1.OfClass(typeof(ViewFamilyType));

            ViewFamilyType FloorPlanView1 = GetFamilyType(doc, "Floor Plan");
            


            ViewFamilyType CeilingPlanView1 = GetFamilyType(doc, "Ceiling Plan");
            

            ViewPlan NewFloorPlan = ViewPlan.Create(doc, FloorPlanView1.Id,Lvl.Id);
           NewFloorPlan.Name = $"418_AR_FloorPlan" + " " + Lvl.Name;

            ViewPlan NewCeilingPlan = ViewPlan.Create(doc, CeilingPlanView1.Id, Lvl.Id);
            NewCeilingPlan.Name = $"418_AR_Ceiling Plan" + " " + Lvl.Name;

            //get titleblock

            FilteredElementCollector collector2 = new FilteredElementCollector(doc);
            collector2.OfCategory(BuiltInCategory.OST_TitleBlocks);
            collector2.WhereElementIsElementType();

            //CREATE SHEET

            ViewSheet newSheet = ViewSheet.Create(doc, collector2.FirstElementId());
            newSheet.Name = $"418_AR_Sheet" + " " + Lvl.Name;
            newSheet.SheetNumber = "A101";

            XYZ InsPoint1 = new XYZ();
            XYZ InsPoint2 = new XYZ(1,1,1);
            Viewport newViewPort = Viewport.Create(doc, newSheet.Id, NewFloorPlan.Id, InsPoint2);

            t.Commit();
            t.Dispose();

            return Result.Succeeded;
        }

        internal ViewFamilyType GetFamilyType(Document doc, string Name) 
        {

            FilteredElementCollector collector1 = new FilteredElementCollector(doc);
            collector1.OfClass(typeof(ViewFamilyType));

           
            foreach (ViewFamilyType view in collector1)
            {
                if (view.Name == Name)
                {
                    return view;
                }
                

            }
            return null;
        }
    }



}



