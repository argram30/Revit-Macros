/*
 * Created by SharpDevelop.
 * User: Raja
 * Date: 19.03.2019
 * Time: 14:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Visual;
using System.Text;

namespace Test
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	[Autodesk.Revit.DB.Macros.AddInId("A6217D81-81F8-4AB9-8368-483B46F57B9B")]
	public partial class ThisApplication
	{
		private void Module_Startup(object sender, EventArgs e)
		{

		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}
		
		
		#region Revit Macros generated code
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		#endregion
		
		#region Get the ceiling of the room
		public void RoomCeilingHeight()
		{
			Document doc = this.ActiveUIDocument.Document;
			UIDocument uidoc = new UIDocument(doc);
//			RevitCommandId commandId = RevitCommandId.LookupPostableCommandId(PostableCommand.Default3DView);
			//            if (this.CanPostCommand(commandId))
			//            	this.PostCommand(commandId);

			// Prompt user to select a room
			Room room = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)) as Room;
			LocationPoint roomPoint = room.Location as LocationPoint;
			
			//GeometryElement roomGeoElem = room.get_Geometry(new Options());
			//XYZ roomCent = roomGeoElem.Select(geo => geo as Solid).FirstOrDefault(sld => null != sld).ComputeCentroid();
			var view = new FilteredElementCollector(doc)
				.OfClass(typeof(View3D)).OfType<View3D>()
				.FirstOrDefault((View3D v) => v.Name.Contains("{3d"));
			var bic = BuiltInCategory.OST_Floors;
			

			ReferenceIntersector intersector = new ReferenceIntersector(
				new ElementCategoryFilter(bic),
				FindReferenceTarget.Element, view);
			
			// XYZ.BasisZ shoots the ray "up"
			ReferenceWithContext rwC = intersector.FindNearest(roomPoint.Point, XYZ.BasisZ * -1);

			// Report the data to the user. This information could also be used to set a "Ceiling Height" instance parameter
			if (rwC == null)
				TaskDialog.Show(string.Format("{0}", view.Name), "no Floors found");
			else
			{
				var rwD = "View used to Project the Ray " + doc.GetElement(intersector.ViewId).Name;
				var rwPro = "Height is " + UnitUtils.ConvertFromInternalUnits(rwC.Proximity, DisplayUnitType.DUT_MILLIMETERS).ToString() + " mm";
				var rwEle = bic.ToString() + " id: " + rwC.GetReference().ElementId.ToString();
				TaskDialog.Show("Element ID", rwEle + "\n" + rwPro + "\n" + rwD);
			}
		}
		#endregion
		
		#region Geometry of the wall
		public void GetFacesAndEdges()
		{
			String faceInfo = "";
			
			Document doc = this.ActiveUIDocument.Document;
			UIDocument uidoc = new UIDocument(doc);

			// Prompt user to select a room
			Wall elem = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)) as Wall;

			Options opt = new Options();
			GeometryElement geomElem = elem.get_Geometry(opt);
			foreach (GeometryObject geomObj in geomElem)
			{
				Solid geomSolid = geomObj as Solid;
				if (null != geomSolid)
				{
					int faces = 0;
					double totalArea = 0;
					foreach (Face geomFace in geomSolid.Faces)
					{
						faces++;
						faceInfo += "Face " + faces + " area: " + geomFace.Area.ToString() + "\n";
						totalArea += geomFace.Area;
					}
					faceInfo += "Number of faces: " + faces + "\n";
					faceInfo += "Total area: " + totalArea.ToString() + "\n";
					foreach (Edge geomEdge in geomSolid.Edges)
					{
						// get wall's geometry edges
					}
				}
			}
			TaskDialog.Show("Revit", faceInfo);
		}
		
		#endregion
		
		#region Centroid of Solid
		public void GetCentroid()
		{
			String faceInfo = "";
			
			Document doc = this.ActiveUIDocument.Document;
			UIDocument uidoc = new UIDocument(doc);

			// Prompt user to select a room
			Room elem = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)) as Room;

			Options opt = new Options();
			GeometryElement geomElem = elem.get_Geometry(opt);
			foreach (GeometryObject geomObj in geomElem)
			{
				Solid geomSolid = geomObj as Solid;
				if (null != geomSolid)
				{
					XYZ cent = geomSolid.ComputeCentroid();
					
					faceInfo += cent.ToString();
				}
			}
			TaskDialog.Show("Revit", faceInfo);
		}
		
		#endregion
		
		#region Geometry of the room
		public void RoomFaces()
		{
			String faceInfo = "";
			
			Document doc = this.ActiveUIDocument.Document;
			UIDocument uidoc = new UIDocument(doc);

			// Prompt user to select a room
			Room elem = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)) as Room;

			Options opt = new Options();
			GeometryElement geomElem = elem.get_Geometry(opt);
			foreach (GeometryObject geomObj in geomElem)
			{
				Solid geomSolid = geomObj as Solid;
				if (null != geomSolid)
				{
					int faces = 0;
					double totalArea = 0;
					foreach (Face geomFace in geomSolid.Faces)
					{
						faces++;
						faceInfo += "Face " + faces + " area: " + geomFace.Area.ToString() + "\n";
						totalArea += geomFace.Area;
					}
					faceInfo += "Number of faces: " + faces + "\n";
					faceInfo += "Total area: " + totalArea.ToString() + "\n";
					
				}
			}
			TaskDialog.Show("Revit", faceInfo);
		}
		
		#endregion
		
		#region Shrink single Grid in the Active view
		public void ShrinkGrid()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			Selection sel = uidoc.Selection;
			View view = doc.ActiveView;
			var num = 2;
			
			ISelectionFilter f = new JtElementsOfClassSelectionFilter<Grid>();
			Reference elemRef = uidoc.Selection.PickObject(ObjectType.Element, f, "Pick a Grid");
			
			Grid grid = doc.GetElement(elemRef) as Grid;
			IList<Curve> gridCurves = grid.GetCurvesInView(DatumExtentType.ViewSpecific, view);
			
			using (Transaction tx = new Transaction(doc))
			{
				tx.Start("Modify the grid start and end point");
				foreach (var c in gridCurves)
				{
					XYZ start = c.GetEndPoint( 0 );
					XYZ end = c.GetEndPoint( 1 );

					XYZ newStart = start + num * XYZ.BasisX;
					XYZ newEnd = end - num * XYZ.BasisX;
					Line newLine = Line.CreateBound( newStart, newEnd );
					
//					XYZ crStart = cr.GetEndPoint(0);
//					XYZ crEnd = cr.GetEndPoint(1);
//					Line crLine = Line.CreateBound(crStart, crEnd);

					grid.SetCurveInView(DatumExtentType.ViewSpecific, view, newLine);
				}
				tx.Commit();
			}
			TaskDialog.Show("Result", "The Grid is extended");
		}
		#endregion
		
		#region Shrink Multiple Grids in the Active view
		public void ShrinkMultipleGrids()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			Selection sel = uidoc.Selection;
			View view = doc.ActiveView;
			var num = 3;
			
			#region Shrinking Grids using the Line - WIP
//			ISelectionFilter l = new JtElementsOfClassSelectionFilter<DetailCurve>();
//			Reference lineRef = uidoc.Selection.PickObject(ObjectType.Element, l, "Pick the grid extent");
//			LocationCurve crv = doc.GetElement(lineRef).Location as LocationCurve;
//			var cr = crv.Curve;
			#endregion
			
			// Get the Grids to extend from the project
			ISelectionFilter f = new JtElementsOfClassSelectionFilter<Grid>();
			IList<Reference> picked = sel.PickObjects(ObjectType.Element, f, "Pick Grids");
			
			foreach (var elemRef in picked)
			{
				Grid grid = doc.GetElement(elemRef) as Grid;
				IList<Curve> gridCurves = grid.GetCurvesInView(DatumExtentType.ViewSpecific, view);
				
				using (Transaction tx = new Transaction(doc))
				{
					tx.Start("Modify the grids endpoints");
					foreach (var c in gridCurves)
					{
						XYZ start = c.GetEndPoint( 0 );
						XYZ end = c.GetEndPoint( 1 );

						XYZ newStart = start.Add(num * XYZ.BasisX);
						XYZ newEnd = end.Subtract(num * XYZ.BasisX);
						Line newLine = Line.CreateBound( newStart, newEnd );
						
						grid.SetCurveInView(DatumExtentType.ViewSpecific, view, newLine);
					}
					tx.Commit();
				}
			}
			TaskDialog.Show("Result", "The Grids are extended");
		}
		#endregion
		
		#region View CropShape
		public void GetViewBoundaryCurves()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			View view = uidoc.Document.ActiveView;
			
			string curveName = string.Empty;
			
			var cropshape = view.GetCropRegionShapeManager();
			var crvs = cropshape.GetCropShape();
			
			foreach (var i in crvs)
			{
				var loop = i.GetCurveLoopIterator();
				loop.Reset();
				while (loop.MoveNext())
				{
					var name = loop.Current.ApproximateLength.ToString();
					curveName += name + "\n";
				}
			}
			TaskDialog.Show("Check", curveName);
		}
		#endregion
		
		#region Check if the family is an In Place family or not
		public void CheckFamilyInstance()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			Selection sel = uidoc.Selection;
			ISelectionFilter f = new JtElementsOfClassSelectionFilter<FamilyInstance>();
			Reference refe = sel.PickObject(ObjectType.Element, f, "Select an Family Instance");
			FamilyInstance elem = doc.GetElement(refe) as FamilyInstance;
			Family fam = elem.Symbol.Family;
			
			if (fam.IsInPlace)
			{
				TaskDialog.Show("In Place Family or Not", "Its In place family");
			}
			else
			{
				TaskDialog.Show("In Place Family or Not", "Its not In place family");
			}
			
		}
		#endregion
		
		#region Get the schedule cell Text
		public void Schedule2014GetCellText()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			ViewSchedule viewSchedule = doc.ActiveView as ViewSchedule;
			SectionType sectionType = SectionType.Body;

			string data = "";
			for (int i = 0; i < getRowColumnCount(viewSchedule, sectionType, true); i++)
			{
				for (int j = 0; j <  getRowColumnCount(viewSchedule, sectionType, false); j++)
				{
					data += viewSchedule.GetCellText(sectionType, i, j) + ",";
				}
				// remove the trailing "," after the last cell and add a newline for the end of this row
				data = data.TrimEnd(',') + "\n";
			}
			TaskDialog.Show(viewSchedule.Name, data);
		}
		#endregion

		#region Get the Row and Column Count
		private int getRowColumnCount(ViewSchedule view, SectionType sectionType, bool countRows)
		{
			int ctr = 1;
			// loop over the columns of the schedule
			while (true)
			{
				try // GetCellText will throw an ArgumentException is the cell does not exist
				{
					if (countRows)
						view.GetCellText(sectionType, ctr, 1);
					else
						view.GetCellText(sectionType, 1, ctr);
					ctr++;
				}
				catch (Autodesk.Revit.Exceptions.ArgumentException)
				{
					return ctr;
				}
			}
		}
		#endregion
		
		#region Get the Name of Active selection from the document
		public void GetActiveUISelection()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			
			var elemIds = uidoc.Selection.GetElementIds();
			var name = string.Empty;
			
			foreach (var id in elemIds)
			{
				var elem = doc.GetElement(id);
				var catId = elem.Category.Id.IntegerValue;
				var catName = (BuiltInCategory)Enum.ToObject(typeof(BuiltInCategory),catId);
				var elemCat = elem.Category.Name;
				name += elemCat.ToString() + "\n";
			}
			
			TaskDialog.Show("Name and Id", name);
		}
		#endregion
		
		#region Create a duplicate view of the Active with only Two clicks
		public void DuplicateViewWithTwoClicks()
		{
			Document doc = this.ActiveUIDocument.Document;
			UIDocument uidoc = new UIDocument(doc);

			// prompt user to pick two points
			XYZ lowLeftPickPoint = uidoc.Selection.PickPoint("Pick lower left corner");
			XYZ upRightPickPoint = uidoc.Selection.PickPoint("Pick upper right corner");

			// create a new BoundingBoxXYZ & set its Min and Max to the XYZ points selected by the user
			BoundingBoxXYZ bboxFromPicks = new BoundingBoxXYZ();
			bboxFromPicks.Min = lowLeftPickPoint;
			bboxFromPicks.Max = upRightPickPoint;

			// Find a titleblock in the project, or use InvalidElementId to create a sheet with no titleblock
//			ElementId titleblockId = ElementId.InvalidElementId;
//			FamilySymbol titleBlockSymbol = doc.TitleBlocks.Cast<FamilySymbol>().FirstOrDefault();
//
//			if (titleBlockSymbol != null)
//				titleblockId = titleBlockSymbol.Id;

//			ViewSheet sheet = null;
			View newView = null;
			using (Transaction t = new Transaction(doc,"crop"))
			{
				t.Start();

				// duplicate the active view
				ElementId newViewId = doc.ActiveView.Duplicate(ViewDuplicateOption.WithDetailing);
				newView = doc.GetElement(newViewId) as View;

				// set the crop box of the new view to the bounding box created from the two picked points
				newView.CropBox = bboxFromPicks;

				// Create the new sheet
//				sheet = ViewSheet.Create(doc, titleblockId);

//				newView.Name = sheet.Name + "-" + sheet.SheetNumber;
//				newView.Scale = 10;
//
//				// Create the viewport to put the new view on the new sheet at (0,0,0)
//				Viewport.Create(doc, sheet.Id, newViewId, XYZ.Zero);
				t.Commit();
			}
			// make the new sheet the active view
			uidoc.ActiveView = newView;

		}
		#endregion
		
		#region Create a sectional view of the family instance
		public void CreateSectionOfFamily()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			Selection sel = uidoc.Selection;
			ISelectionFilter f = new JtElementsOfClassSelectionFilter<FamilyInstance>();
			Reference refer = sel.PickObject(ObjectType.Element, f , "Select the family instance");
			FamilyInstance fi = doc.GetElement(refer) as FamilyInstance;
			
			ViewFamilyType vft = new FilteredElementCollector(doc)
				.OfClass(typeof(ViewFamilyType))
				.Cast<ViewFamilyType>()
				.FirstOrDefault<ViewFamilyType>(x =>ViewFamily.Section == x.ViewFamily);
			
			var loc = fi.Location as LocationPoint;
			var locPt = loc.Point;
			
			var bb = fi.get_BoundingBox(null);
			var height = bb.Max.Z;
			var width = bb.Max - bb.Min;
			var minPt = new XYZ(locPt.X, locPt.Y + width.Y, height);
			
			var message = locPt.ToString() + "\n" + bb.Max.ToString() + "\n" + bb.Min.ToString() + "\n" + width.ToString() + "\n" + minPt.ToString();
			TaskDialog.Show("Title", message);
			
		}
		#endregion
		
		#region Duplicate sheet with view
		public void DuplicateSheet()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document revDoc = uidoc.Document;
			ViewSheet revSheet = revDoc.ActiveView as ViewSheet;
			
			#region do stuff
			if (null != revSheet)
			{
				TaskDialog.Show("Title", revSheet.Name + "  " + revSheet.SheetNumber);
				using (Transaction t = new Transaction(revDoc, "Duplicating sheet with parameters"))
				{
					var title = new FilteredElementCollector(revDoc).OfClass(typeof(FamilyInstance))
						.OfCategory(BuiltInCategory.OST_TitleBlocks).Cast<FamilyInstance>()
						.First(block => block.OwnerViewId == revSheet.Id);


					var newSheet = ViewSheet.Create(revDoc, title.GetTypeId());
					newSheet.SheetNumber = revSheet.SheetNumber + "1";
					newSheet.Name = revSheet.Name;

					foreach (var item in revSheet.GetAllViewports())
					{
						View newView = null;
						var actualViewport = revDoc.GetElement(item) as Viewport;
						var actualView = revDoc.GetElement(actualViewport.ViewId) as View;
						var actualViewTemp = actualView.ViewTemplateId;
						

						if (actualView.ViewType != ViewType.Legend)
						{
							var newViewId = actualView.Duplicate(ViewDuplicateOption.WithDetailing);
							newView = revDoc.GetElement(newViewId) as View;
							newView.Name = actualView.Name + "Suffix";
							if(null != actualViewTemp)
								newView.ViewTemplateId = actualViewTemp;
						}

						else
							newView = actualView;

						var newViewport = Viewport
							.Create(revDoc, newSheet.Id, newView.Id, actualViewport.GetBoxCenter());
					}
					
					#region Getting User Created Parameters from the Project
					var defList = new List<Definition>();
					var it = revSheet.ParametersMap.ForwardIterator();
					it.Reset();

					while (it.MoveNext())
					{
						var param = it.Current as Parameter;
						var definition = param.Definition as InternalDefinition;

						if (definition.BuiltInParameter == BuiltInParameter.INVALID)
							defList.Add(definition);
					}
					#endregion //Getting User Created Parameters from the Project
					
					string parameters = null;
					
					foreach (var definition in defList)
					{
						var actualParam = revSheet.get_Parameter(definition);
						var newParam = newSheet.get_Parameter(definition);
						parameters += actualParam;

						if (actualParam.HasValue)
							newParam.SetValueString(actualParam.AsString());
					}
					
					TaskDialog.Show("Parameters", parameters);
				}
			}
			
			else
				TaskDialog.Show("Title", "This view is not a sheet");
			#endregion
		}
		#endregion
		
		#region Get the parameters of the sheet
		public void GetParametersOfSheet()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document revDoc = uidoc.Document;
			ViewSheet revSheet = revDoc.ActiveView as ViewSheet;
			
			#region Getting User Created Parameters from the Project
			var defList = new List<Definition>();
			var it = revSheet.ParametersMap.ForwardIterator();
			it.Reset();

			while (it.MoveNext())
			{
				var param = it.Current as Parameter;
				var definition = param.Definition as InternalDefinition;

				if (definition.BuiltInParameter == BuiltInParameter.INVALID)
					defList.Add(definition);
			}
			#endregion //Getting User Created Parameters from the Project
			
			string parameters = string.Empty;
			
			using (Transaction t = new Transaction(revDoc, "Set Value"))
			{
				int num = 0;
				foreach (var definition in defList)
				{
					Parameter actualParam = revSheet.get_Parameter(definition);
					t.Start();
					actualParam.Set("Changed " + num.ToString());
					t.Commit();
					parameters += actualParam.AsString();
					num += 1;
				}
			}
			TaskDialog.Show("Parameters", parameters);
		}
		#endregion
		
		#region Get the Boundingbox of the viewport
		public void GetBBoxVP()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			
			var elem = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element)) as Viewport;
			
			var bbox = elem.get_BoundingBox(doc.ActiveView);
			
			TaskDialog.Show("BBox", bbox.Min.ToString());
		}
		#endregion
		
		#region Get the Material RGB from Project
		public void GetMaterialColor ()
		{
			Document doc = this.ActiveUIDocument.Document;
			List<Element> materials = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
			var message = new StringBuilder ();

			foreach (Material material in materials)
			{
				string name = material.Name;
				try
				{
					string red = material.Color.Red.ToString();
					string green = material.Color.Green.ToString();
					string blue = material.Color.Blue.ToString();
					message.AppendLine (name + "-" + red + "_" + green + "_" + blue + ".");
				}
				catch (Exception e)
				{
					message.AppendLine (name + " - " + e.Message);
				}

			}
			TaskDialog.Show ("Materials in the project", message.ToString ());
		}
		#endregion
		
		#region Get the diffuse color
		public void GetDiffuseColor()
		{
			var message = new StringBuilder();
			Document doc = this.ActiveUIDocument.Document;
			var materials = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
			
			foreach (Material mat in materials)
			{
				try
				{
					ElementId appAssetId = mat.AppearanceAssetId;
					if(appAssetId != ElementId.InvalidElementId)
					{
						AppearanceAssetElement assetElem = mat.Document.GetElement(appAssetId) as AppearanceAssetElement;
						if(assetElem != null)
						{
							using (Transaction t = new Transaction(assetElem.Document, "Get Material Color"))
							{
								t.Start();
								using (AppearanceAssetEditScope editScope = new AppearanceAssetEditScope(assetElem.Document))
								{
									string name = mat.Name;
									
									Asset editableAsset = editScope.Start(assetElem.Id);
									
									AssetPropertyDoubleArray4d genericDiffuseProperty = editableAsset.FindByName("generic_diffuse") as AssetPropertyDoubleArray4d;
									var color = genericDiffuseProperty.GetValueAsColor();
									string red = color.Red.ToString();
									string green = color.Green.ToString();
									string blue = color.Blue.ToString();
									message.AppendLine(name + "-" + red + "_" + green + "_" + blue + ".");
									
									editScope.Commit(true);
								}
								t.Commit();
							}
						}
						else
						{
							message.AppendLine(mat.Name.ToString() + " - " + appAssetId.ToString());
						}
					}
					else
					{
						message.AppendLine(mat.Name.ToString() + " - " + appAssetId.ToString());
					}
				}
				catch(Exception ex)
				{
					message.AppendLine(mat.Name.ToString() + " - " + ex.Message);
				}
			}
			TaskDialog.Show ("Materials in the project", message.ToString());
		}
		#endregion //Get the diffuse color
		
		#region Read Material Properties
		private void ReadMaterialAppearanceProp()
		{
			var message = new StringBuilder();
			Document doc = this.ActiveUIDocument.Document;
			var app = this.ActiveUIDocument.Document.Application;
			var materials = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
			
			foreach (Material material in materials)
			{
				try
				{
					var appearanceId = material.AppearanceAssetId;
					var appearanceElem = doc.GetElement(appearanceId) as AppearanceAssetElement;
					Asset theAsset = appearanceElem.GetRenderingAsset();
					string libraryName = theAsset.LibraryName;
					string title = theAsset.Title;

					// The predefined asset can be empty. at this time, get the system appearance asset instead.
					if (theAsset.Size == 0)
					{
						AssetSet systemAppearanceAssets = doc.Application.get_Assets(AssetType.Appearance);
						foreach (Asset systemAsset in systemAppearanceAssets)
						{
							if(theAsset.LibraryName == systemAsset.LibraryName
							   && theAsset.Name == systemAsset.Name)
							{
								message.AppendLine(systemAsset.FindByName("generic_diffusion").ToString());
							}
						}
					}
				}
				catch(Exception ex)
				{
					message.AppendLine(material.Name.ToString() + " - " + ex.Message);
				}
			}
			
			TaskDialog.Show ("Read material Properties", message.ToString());
		}
		#endregion \\Read Material Properties
		
		#region Create a direct Shape of Sphere geometry
		public void CreateSphereDirectShape()
		{
			Document doc = this.ActiveUIDocument.Document;
			List<Curve> profile = new List<Curve>();

			// first create sphere with 2' radius
			XYZ center = XYZ.Zero;
			double radius = 10.0;
			XYZ profile00 = center;
			XYZ profilePlus = center + new XYZ(0, radius, 0);
			XYZ profileMinus = center - new XYZ(0, radius, 0);

			profile.Add(Line.CreateBound(profilePlus, profileMinus));
			profile.Add(Arc.Create(profileMinus, profilePlus, center + new XYZ(radius, 0, 0)));

			CurveLoop curveLoop = CurveLoop.Create(profile);
			SolidOptions options = new SolidOptions(new ElementId(20489), ElementId.InvalidElementId);

			Frame frame = new Frame(center, XYZ.BasisX, -XYZ.BasisZ, XYZ.BasisY);
			if (Frame.CanDefineRevitGeometry(frame) == true)
			{
				Solid sphere = GeometryCreationUtilities.CreateRevolvedGeometry(frame, new CurveLoop[] { curveLoop }, 0, 2 * Math.PI, options);
				using (Transaction t = new Transaction(doc, "Create sphere direct shape"))
				{
					t.Start();
					// create direct shape and assign the sphere shape
					DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));

					ds.ApplicationId = "Application id";
					ds.ApplicationDataId = "Geometry object id";
					ds.SetShape(new GeometryObject[] { sphere });
					t.Commit();
				}
			}
		}
		#endregion
		
		#region Place the family instance on the face
		public void CreateInstancesOnHostFamilyFaces()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			
			Selection sel = uidoc.Selection;
			ISelectionFilter famFilter = new JtElementsOfClassSelectionFilter<FamilyInstance>();
			ISelectionFilter elemFilter = new JtElementsOfClassSelectionFilter<Element>();
			Reference famRefer = sel.PickObject(ObjectType.Element, famFilter , "Select the family instance");
			Reference elemRefer = sel.PickObject(ObjectType.Element, elemFilter , "Select the Host element");
			Element elem = doc.GetElement(elemRefer) as Element;
			FamilyInstance famInst = doc.GetElement(famRefer) as FamilyInstance;
			FamilySymbol famSym = famInst.Symbol;
			
			
			var faces = GetSolids(elem)
				.SelectMany(x => x.Faces.OfType<PlanarFace>());

			using (var transaction = new Transaction(doc, "create families"))
			{
				transaction.Start();

				if (!famSym.IsActive)
					famSym.Activate();

				foreach (var planarFace in faces)
				{
					var faceBox = planarFace.GetBoundingBox();

					var center = planarFace.Evaluate(0.5*(faceBox.Max + faceBox.Min));

					doc.Create.NewFamilyInstance(planarFace, center, XYZ.Zero, famSym);
				}

				transaction.Commit();
			}
		}

		private static IEnumerable<Solid> GetSolids(Element element)
		{
			var geometry = element
				.get_Geometry(new Options {ComputeReferences = true});

			if (geometry == null)
				return Enumerable.Empty<Solid>();

			return GetSolids(geometry)
				.Where(x => x.Volume > 0);
		}

		private static IEnumerable<Solid> GetSolids(IEnumerable<GeometryObject> geometryElement)
		{
			foreach (var geometry in geometryElement)
			{
				var solid = geometry as Solid;
				if (solid != null)
					yield return solid;

				var instance = geometry as GeometryInstance;
				if (instance != null)
					foreach (var instanceSolid in GetSolids(instance.GetInstanceGeometry()))
						yield return instanceSolid;

				var element = geometry as GeometryElement;
				if (element != null)
					foreach (var elementSolid in GetSolids(element))
						yield return elementSolid;
			}
		}
		#endregion
			
		#region Purge Materials
		public void purgeMaterials()
		{
			
			Document doc = this.ActiveUIDocument.Document;
			var app = doc.Application;
			string matNames = String.Empty;
			
			var materials = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
			
			foreach (var material in materials) 
			{
				var materialName = material.Name;
				if (materialName.Contains("Default")) {
					matNames += materialName + Environment.NewLine;
				}
				
				
			}
			
			TaskDialog td = new TaskDialog("Info");
			td.MainInstruction = "Materials in the project:";
			td.MainContent = matNames;
			td.Show();
			
		}
		#endregion
		
		#region SOL1 Export schedule
		public void SOL1ScheduleExport()
		{
			var doc = this.ActiveUIDocument.Document;
			var app = doc.Application;
			string scheduleNames = String.Empty;
			
			var folder_env = @"%USERPROFILE%\OneDrive - TTP AG\08_Testing\RTV-BAY_SOL1_Schedule_Export\Schedule Exports";
			var folder_path = Environment.ExpandEnvironmentVariables(folder_env);
			var _ext = ".txt";
			var opt = new ViewScheduleExportOptions();
			opt.HeadersFootersBlanks = false;
			opt.Title = true;
			
			var toExport = new List<string>()
			{
				"BAY_AKZ-Check_List_Doors",
				"BAY_AKZ-Check_List_General",
				"BAY_AKZ-Check_List_Windows",
				"BAY_Overview-Clean_Room_Classification",
				"BAY_Room Schedule Detailed",
				"SOL1_Raumbuch_DatenMaster",
				"SOL1_Tuerliste_DatenMaster"
			};
			
			var vsCol = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule));
			
			foreach (ViewSchedule vs in vsCol) {
				if (toExport.Contains(vs.Name)) {
					vs.Export(folder_path, vs.Name + _ext, opt);
				}
			}
			
//			var td = new TaskDialog("Info");
//			td.MainInstruction = "View Schedule in project:";
//			td.MainContent = scheduleNames;
//			td.Show();
		}
		#endregion
		
		#region Reset Param values
		public void resetParamValues()
		{
			UIDocument uidoc = this.ActiveUIDocument;
			Document doc = uidoc.Document;
			
			
			Selection sel = uidoc.Selection;
			var spacFilter = new JtElementsOfClassSelectionFilter<Space>();
			Reference spaRefer = sel.PickObject(ObjectType.Element,spacFilter, "Select the space");
			
			var elem = doc.GetElement(spaRefer) as Element;
			
			var parameter = elem.LookupParameter("SP_Space_Temperature");
			var check = false;
			
			using(Transaction t = new Transaction(doc, "Clear the parameter Values"))
			{
				t.Start();
				check = parameter.ClearValue();
				t.Commit();
			}
			
			TaskDialog.Show("The result", check.ToString() + " " + parameter.AsString());
		}
		#endregion
		
		#region Export Parameters to csv
		public void SOL1ParameterCheck()
		{
			Document doc = this.ActiveUIDocument.Document;
			var sb = new StringBuilder();
			var userName = doc.Application.Username.ToString();
			var docName = doc.Title.ToString().Replace(userName, "");
			
			var catList = new List<BuiltInCategory>
			{
				BuiltInCategory.OST_DuctTerminal,// DuctTerminal is Air Terminal in Revit
				BuiltInCategory.OST_CableTray,
				BuiltInCategory.OST_DataDevices,
				BuiltInCategory.OST_Doors,
				BuiltInCategory.OST_DuctAccessory,
				BuiltInCategory.OST_DuctFitting,
				BuiltInCategory.OST_DuctSystem,
				BuiltInCategory.OST_ElectricalEquipment,
				BuiltInCategory.OST_ElectricalFixtures,
				BuiltInCategory.OST_FireAlarmDevices,
				BuiltInCategory.OST_Furniture,
				BuiltInCategory.OST_LightingDevices,
				BuiltInCategory.OST_LightingFixtures,
				BuiltInCategory.OST_MechanicalEquipment,
				BuiltInCategory.OST_NurseCallDevices,
				BuiltInCategory.OST_PipeAccessory,
				BuiltInCategory.OST_PlumbingFixtures,
				BuiltInCategory.OST_Rooms,
				BuiltInCategory.OST_SecurityDevices,
				BuiltInCategory.OST_Sheets,
				BuiltInCategory.OST_SpecialityEquipment,
				BuiltInCategory.OST_Sprinklers,
				BuiltInCategory.OST_TelephoneDevices,
				BuiltInCategory.OST_Walls,
				BuiltInCategory.OST_Windows
			};
			
			var headers = new List<string>{"Document Name", "Element ID", "Element Name", "Element Category"};
			var BayParamList = new List<string>{
				"BAY_industrial_complex",
				"BAY_technical_equipment",
				"BAY_process_plant",
				"BAY_plant_section",
				"BAY_industrial_unit"
			};
			headers.AddRange(BayParamList);
			sb.AppendLine(string.Join(",", headers));
			
			var multiFilter = new ElementMulticategoryFilter(catList);			
			var fec = new FilteredElementCollector(doc).WherePasses(multiFilter).WhereElementIsNotElementType();
			
			foreach (var element in fec) {
				var elemInfo = new List<string>
				{
					docName,
					element.Id.ToString(), 
					element.Name.ToString(), 
					element.Category.Name.ToString()
				};
				
				var paramsIncluded = BayParamList.Select(x => element.LookupParameter(x) == null ? "Not present" : "Present").ToList();
				
				elemInfo.AddRange(paramsIncluded);
				sb.AppendLine(string.Join(",", elemInfo));
			}
			
			//var exportCsv = File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export.csv");			
			var filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Test\" + string.Format("{0}.csv", docName);
			File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding());
			
			TaskDialog.Show("Test",sb.ToString());
		}
		#endregion
		
	}

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
}
