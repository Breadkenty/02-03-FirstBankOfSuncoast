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
        }
    }
}
