using System;
using System.Data; 
using System.Data.OleDb; 

namespace Endogine.Tools
{
	/// <summary>
	/// Summary description for DbAccess.
	/// </summary>
	public class DbAccess
	{
		public DbAccess()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static DataTable ExcelToDataTable(string filename)
		{
			if (!System.IO.File.Exists(filename))
				throw new Exception("File not found: "+filename);

			string sConn = "Provider=Microsoft.Jet.OLEDB.4.0;" +
				"Data Source=" + filename + "; " +
				"Extended Properties=Excel 8.0;"; //MaxBufferSize=1024; IMEX=1  Jet OLEDB:Max Buffer Size=256;
			
			OleDbConnection conn = new OleDbConnection(sConn);
			conn.Open();

			OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", conn);
			DataTable dt = new DataTable();
			adp.Fill(dt);
			conn.Close();

			return dt;
		}

		public static DataTable CSVToDataTable(string filename)
		{
			string sFile = Endogine.Files.FileReadWrite.Read(filename);
			string[] lines = sFile.Split("\r\n".ToCharArray());
			DataTable dt = new DataTable();
			foreach (string line in lines)
			{
				string[] columns = line.Split("\t".ToCharArray());
				if (dt.Columns.Count==0)
				{
					foreach (string column in columns)
						dt.Columns.Add(column);
				}
				else
				{
					bool bEmpty = false;
					for (int i=0;i<columns.Length;i++)
						if (columns[i].Length == 0)
							bEmpty = true;
					if (bEmpty)
						continue;
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					for (int i=0;i<columns.Length;i++)
						row[i] = columns[i];
				}
			}
//			string sConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+filename+";Extended Properties='text;FMT=Delimited'";
//			OleDbConnection conn = new OleDbConnection(sConn);
//			conn.Open();
//			OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", conn);
//			DataTable dt = new DataTable();
//			adp.Fill(dt);
//			conn.Close();
			return dt;
		}
	}
}
