public void lengthLines()
		{
			// The first two commands are the regular routines
			// Creating the instance of the User Interface Document
			UIDocument uidoc = this.ActiveUIDocument;
			// Creating the instance of the Database Document
			Document doc = uidoc.Document;

			// Assigning initial value of the Line to be 0
			double lenLines = 0;
			// Prompting the user to select the multiple lines to store it in list
			ICollection<ElementId> ids = uidoc.Selection.GetElementIds();

			// Looping each ids to get the length of the line
			foreach (ElementId id in ids)
			{
				// Using the id from the list to get the element we need
				Element e = doc.GetElement(id);
				// Using the parameter class to get the parameter value
				Parameter lenParam = e.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
				if (lenParam == null)
					continue;
				// Adding the current value of length as double
				lenLines += lenParam.AsDouble();

			}

			// Formating the lenLines value according to the units of the project
			string lenLineUnits = UnitFormatUtils.Format(doc.GetUnits(),UnitType.UT_Length, lenLines, false, false);
			// Show the value generated in the Dialog box
			TaskDialog.Show("Length ", ids.Count + " elements selected and the total length is: " + lenLineUnits + "m");

		}
