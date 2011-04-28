using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shared.Database;

[DataTable(DatabaseName = "Characters", TableName = "Map_Info", PreCache = true)]
[Serializable]
public class Map_Info : DataObject
{

}
