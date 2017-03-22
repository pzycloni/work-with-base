using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work_with_DataBase {

	class Program {
		static void Main(string[] args) {

			var dict = new Dictionary<string, string>();

			dict.Add("ID", "INT IDENTITY(1,1) NOT NULL");
			dict.Add("Name", "NVARCHAR(100) NULL");
			dict.Add("Age", "NVARCHAR(10) NULL");
			dict.Add("Sex", "NVARCHAR(10) NULL");
			dict.Add("Email", "NVARCHAR(100) NOT NULL");
			dict.Add("Password", "NVARCHAR(100) NOT NULL");
			dict.Add("CONSTRAINT", "PK_Client PRIMARY KEY (ID ASC)");

			if (!DB.CreateTable("Clients", dict)) {
				foreach (var error in DB.errors) {
					Console.WriteLine(error);
				}
			}

		}
	}

}
