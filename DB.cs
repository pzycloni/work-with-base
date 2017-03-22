using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Work_with_DataBase {

	static class DB {

		static private SqlConnection connection = new SqlConnection(Work_with_DataBase.Properties.Settings.Default.MyTestConnection);
		static public bool success = true;
		static public List<string> errors = new List<string>();

		static public bool ExecuteQueryWithoutData(string query) {
			query = "USE mytest; " + query;
			// добавляем запрос
			SqlCommand command = new SqlCommand(query, connection);
			// выполняем запрос
			try {
				connection.Open();
				command.ExecuteNonQuery();
			} catch (Exception e) {
				success = false;
				errors.Add(e.Message);
			} finally {
				connection.Close();
			}
			// если все ок, то возвращаем true
			return success;
		}

		static public bool CreateTable(string table, Dictionary<string, string> columns) {
			// основной запрос к бд
			string options = null;

			var keys = GetKeys(columns);
			var values = GetValues(columns);

			for (var col = 0; col < columns.Count; col++) {
				options += String.Format("{0} {1}", keys[col], values[col]);

				if (col < columns.Count - 1) {
					options += ", ";
				}

			} 

			string query = String.Format("CREATE TABLE {0}({1});", table, options);
			// возвращаем результат
			return ExecuteQueryWithoutData(query);
		}

		static private string BuildString(List<string> columns, string wrapper = "") {
			string line = null;

			foreach (var column in columns) {
				line += wrapper + column + wrapper;
			}

			return line;
		}

		static private List<string> GetKeys(Dictionary<string, string> columns) {

			var keys = new List<string>();
			// преобразуем в список
			foreach (var key in columns.Keys) {
				keys.Add(key);
			}

			return keys;
		}

		static private List<string> GetValues(Dictionary<string, string> columns) {

			var values = new List<string>();
			// преобразуем в список
			foreach (var key in columns.Values) {
				values.Add(key);
			}

			return values;
		}

		static public List<Dictionary<string, string>> Select(string table, Dictionary<string, string> columns, string where) {

			var keys = GetKeys(columns);

			// создаем запрос
			string query = "USE mytest; " + String.Format("SELECT {0} FROM {1} WHERE {2};", BuildString(keys), table, where);

			SqlCommand command = new SqlCommand(query, connection);

			try {
				connection.Open();
				command.ExecuteNonQuery();
			} catch (Exception e) {
				success = false;
			} finally {
				connection.Close();
			}

			var result = new List<Dictionary<string, string>>();

			if (success) {
				int counter = 0;

				SqlDataReader reader = command.ExecuteReader();

				while (reader.Read()) {
					var row = new Dictionary<string, string>();

					for (var column = 0; column < reader.FieldCount; column++) {
						// создаем строку key, value
						row.Add(reader.GetName(column).ToString(), reader.GetValue(column).ToString());
					}
					// добавляем строку в список
					result.Insert(counter, row);

					counter++;
				}
			}

			return result;
		}

		static public bool Update(string table, Dictionary<string, string> columns, string where) {
			// получаем все ключи
			var keys = GetKeys(columns);
			// получаем все значения
			var values = GetValues(columns);

			string set = null;

			for (var col = 0; col < columns.Count; col++) {
				set += String.Format("{0}='{1}'", keys[col], values[col]);

				if (col < columns.Count - 1) {
					set += ", ";
				}
			}
			// формируем строку запроса
			string query = String.Format("UPDATE {0} SET {1} WHERE {2};", table, set, where);

			return ExecuteQueryWithoutData(query);

		}

		static public bool Insert(string table, Dictionary<string, string> columns) {
			// получаем названия колонок и преобразуем в строку
			var keys = BuildString(GetKeys(columns));
			// получаем значения и преобразуем в строку
			var values = BuildString(GetValues(columns));
			// формируем строку запроса
			string query = String.Format("INSERT INTO {0}({1}) VALUES({2});", table, keys, values);

			return ExecuteQueryWithoutData(query);

		}
	}
}
