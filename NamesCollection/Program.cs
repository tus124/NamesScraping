using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NamesCollection
{

    public static class DatabaseExtensions
    {
        public static void WriteToDb(this List<Data> list)
        {
            DatabaseConnection dbconn = new DatabaseConnection();
            foreach (var item in list)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                item.meaning = rgx.Replace(item.meaning, "");

                dbconn.InsertRecord(item);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {

            for (int i = 1; i <= 14; i++)
            {
                string filename = "output" + i.ToString() + ".json";
                BuildNamesCollection.LoadJson(filename).WriteToDb();
            }
        }
       
    }

    public static class BuildNamesCollection
    {
        public static List<Data> LoadJson(string filename)
        {
            using (StreamReader r = new StreamReader(filename))
            {
                string json = r.ReadToEnd();
                dynamic array = JObject.Parse(json);

                var names = (JArray)array.names;
                var meanings = (JArray)array.meanings;
                var genders = (JArray)array.genders;
                var origins = (JArray)array.origins;
                var religions = (JArray)array.religions;

                var namesCollection = new List<Data>();

                for(int i=0; i<names.Count(); i++)
                {
                    var d = new Data();
                    d.name = names[i].ToString();
                    d.meaning = meanings[i].ToString();
                    d.gender = genders[i].ToString();
                    d.origin = origins[i].ToString();
                    d.religion = religions[i].ToString();

                    namesCollection.Add(d);
                }

                Console.WriteLine(namesCollection.Count());
                return namesCollection;

            }
        }
    }

    public class Data
    {
        public string name;
        public string meaning;
        public string gender;
        public string origin;
        public string religion;
        public int NameCount;
        public bool IsNamePrime;
    }


    public class DatabaseConnection
    {
        private SqlConnection conn;

        public DatabaseConnection()
        {
            //Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NamesDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False
            conn = new SqlConnection();
            conn.ConnectionString =
                "Data Source=(localdb)\\MSSQLLocalDB;" +
                "Initial Catalog = NamesDb;" +
                "Integrated Security = True;" +
                "Connect Timeout = 30;" +
                "Encrypt = False;" +
                "TrustServerCertificate = False;" +
                "ApplicationIntent = ReadWrite;" +
                "MultiSubnetFailover = False";

           
        }
        public void InsertRecord(Data d)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;


            cmd.CommandText = string.Format("INSERT Names (Name, Meaning, Origin, Religion) VALUES ('{0}','{1}','{2}','{3}')", d.name, d.meaning, d.origin, d.religion);
            cmd.Connection = conn;

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
