using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SmartShoes.Common.Forms
{
	public class Sqlite3Helper
	{
		// Method to open a connection
		private SQLiteConnection OpenConnection()
		{
			//string basePath = AppDomain.CurrentDomain.BaseDirectory;
			string basePath = Application.StartupPath;
			//string basePath = "C:\\Program Files (x86)\\Default Company Name\\TotalTestSetup";
			string fullPath = Path.Combine(basePath, @"GHWLH23.db3");
			string sqlString = $"Data Source={fullPath};Version=3;";

			var connection = new SQLiteConnection(sqlString);
			connection.Open();
			return connection;
		}

		// Method to execute a non-query (INSERT, UPDATE, DELETE)
		public void ExecuteNonQuery(string query, List<SQLiteParameter> parameters = null)
		{
			using (var connection = OpenConnection())
			using (var command = new SQLiteCommand(query, connection))
			{
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters.ToArray());
				}
				command.ExecuteNonQuery();
			}
		}

		// Method to execute a query and return a list of results
		public List<Dictionary<string, object>> ExecuteQuery(string query, List<SQLiteParameter> parameters = null)
		{
			var result = new List<Dictionary<string, object>>();

			using (var connection = OpenConnection())
			using (var command = new SQLiteCommand(query, connection))
			{
				if (parameters != null)
				{
					command.Parameters.AddRange(parameters.ToArray());
				}

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var row = new Dictionary<string, object>();
						for (int i = 0; i < reader.FieldCount; i++)
						{
							row[reader.GetName(i)] = reader.GetValue(i);
						}
						result.Add(row);
					}
				}
			}
			return result;
		}

		// Example method to get all records from a table
		public List<Dictionary<string, object>> GetAllRecords(string tableName)
		{
			string query = $"SELECT * FROM {tableName} ORDER BY ID DESC LIMIT 1;";
			return ExecuteQuery(query);
		}

		public List<Dictionary<string, object>> GetQueryRecords(string queryString)
		{
			string query = queryString;
			return ExecuteQuery(query);
		}

		// Example method to insert a record
		public void InsertRecord(string tableName, Dictionary<string, object> data)
		{
			var columns = string.Join(", ", data.Keys);
			var values = string.Join(", ", data.Keys.Select(k => "@" + k));
			string query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

			var parameters = data.Select(kvp => new SQLiteParameter("@" + kvp.Key, kvp.Value)).ToList();
			ExecuteNonQuery(query, parameters);
		}

		// Example method to update a record
		public void UpdateRecord(string tableName, Dictionary<string, object> data, string whereClause)
		{
			var setClause = string.Join(", ", data.Keys.Select(k => $"{k} = @{k}"));
			string query = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";

			var parameters = data.Select(kvp => new SQLiteParameter("@" + kvp.Key, kvp.Value)).ToList();
			ExecuteNonQuery(query, parameters);
		}

		// Example method to delete a record
		public void DeleteRecord(string tableName, string whereClause)
		{
			string query = $"DELETE FROM {tableName} WHERE {whereClause}";
			ExecuteNonQuery(query);
		}



	}
}