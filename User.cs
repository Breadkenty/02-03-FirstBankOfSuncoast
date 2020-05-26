using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace _02_03_FirstBankOfSuncoast
{
    partial class Program
    {
        class User
        {
            public string UserID { get; set; }
            public string UserPassword { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public string FullName()
            {
                return FirstName + " " + LastName;
            }

            public List<Account> Accounts = new List<Account>();

            public void SaveAccount()
            {
                var writer = new StreamWriter("Accounts.csv");
                var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csvWriter.WriteRecords(Accounts);

                writer.Close();
            }

            public void LoadAccount()
            {
                TextReader reader;

                if (File.Exists("Accounts.csv"))
                {
                    reader = new StreamReader("Accounts.csv");
                }
                else
                {
                    reader = new StringReader("");
                }

                var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                var accounts = csvReader.GetRecords<Account>().ToList();

                reader.Close();
            }

        }
    }
}
