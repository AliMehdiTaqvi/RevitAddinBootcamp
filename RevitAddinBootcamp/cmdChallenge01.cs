using Autodesk.Revit.DB;

namespace RevitAddinBootcamp
{
    [Transaction(TransactionMode.Manual)]
    public class cmdChallenge01 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // 1. SET VARIABLES

            int numFloors = 250;
            double currentElev = 0;
            int floorHeight = 15;
            int fizzbuzzCount = 0;
            int buzzCount = 0;
            int fizzCount = 0;
         



            // GET STATIC INFO
            //2.GET TITLEBLOCK

            FilteredElementCollector tbCollector = new FilteredElementCollector(doc);
            tbCollector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            tbCollector.WhereElementIsElementType();
            ElementId tbBlockId = tbCollector.FirstElementId();

            // 3. GET ALL VIEW FAMILY TYPE

            FilteredElementCollector vfCollector = new FilteredElementCollector(doc);
            vfCollector.OfClass(typeof(ViewFamilyType));

            // 4. GET FLOORPALNS AND CEILING PLANS

            ViewFamilyType vftFloor = null;
            ViewFamilyType vftCeiling = null;

            foreach (ViewFamilyType curVFT in vfCollector)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    vftFloor = curVFT;
                }


                else if (curVFT.ViewFamily == ViewFamily.CeilingPlan)

                {
                    vftCeiling = curVFT;
                }

            }

            // 5.IF VIEW FAMILY DOES NOT EXIST IN THE MODEL

            if (vftFloor == null) 
            {
                TaskDialog.Show("Error", "View Family Does Not exist");
                    return Result.Failed;
            }
             
            else if (vftCeiling == null) 
            {
                TaskDialog.Show("Error", "View Family Does Not exist");
                    return Result.Failed;
            }

            Transaction t=new Transaction(doc);
            t.Start("FIZZBUZZ CHALLENGE");

            // 6.CREATE LEVELS 

                for (int i = 0;i<=numFloors;i++)
            {
                Level newlevel = Level.Create(doc, currentElev);
                newlevel.Name = $"LEVEL {i}";


                // 7.CHECK FIZZBUZZ_

                if (i % 3 == 0 && i % 5 == 0)
                {
                    ViewSheet newsheet = ViewSheet.Create(doc, tbBlockId);

                    newsheet.SheetNumber = i.ToString();
                    newsheet.Name = $"FIZZBUZZ {i}";

                    // 8.BONUS

                    ViewPlan bonusplan = ViewPlan.Create(doc, vftFloor.Id, newlevel.Id);
                    bonusplan.Name = $"FIZZBUSS_{i}";
                    Viewport bonusViewport = Viewport.Create(doc,newsheet.Id, bonusplan.Id, new XYZ (1,05,0));

                    fizzbuzzCount++;


                    // 9.CHECK FIZZ
                }

                else if (i % 3 == 0)
                {
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, vftFloor.Id, newlevel.Id);
                    newFloorPlan.Name = $"FIZZ_{i}";

                    fizzCount++;
                }

                // 9.CHECK BUZZ

                else if (i % 5 == 0)
                {
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, vftCeiling.Id, newlevel.Id);
                    newFloorPlan.Name = $"BUZZ_{i}";

                    buzzCount++;
                }

                // 10.INCREMENT ELEVATION
                currentElev += floorHeight;

            }

                    t.Commit();
                    t.Dispose();



            TaskDialog.Show("Challenge Complete", $"Cteated {numFloors} Levels"+
                $"Created {fizzbuzzCount} FizzBuzz Sheets"+
                $"Created {fizzCount} Fizz Views"+
                $"Created {buzzCount} Buzz Views");


            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnChallenge01";
            string buttonTitle = "Module\r01";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Module01,
                Properties.Resources.Module01,
                "Module 01 Challenge");

            return myButtonData.Data;
        }
    }

}
