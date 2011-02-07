using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using MySql.Data.MySqlClient;

using Sienna;

namespace Sienna.Database
{
    public class QueryCallback
    {
        public delegate void QueryCB(List<Row> Result, Object UserData);

        public void Execute()
        {
            try
            {
                MySqlCommand Request = new MySqlCommand(Query, DB);
                MySqlDataReader Result;

                List<Row> Ret = new List<Row>();

                // Check if query have a return value
                if (Query.StartsWith("SELECT ") == true)
                {
                    Result = Request.ExecuteReader();

                    while (Result.Read())
                    {
                        Row Res = new Row(Result.FieldCount);

                        for (int i = 0; i < Result.FieldCount; i++)
                            Res.AddToRow(Result.GetName(i), Result.GetValue(i).ToString());

                        Ret.Add(Res);
                    }

                    Result.Close();
                }
                else
                {
                    Request.ExecuteNonQuery();
                }

                Callback.Invoke(Ret, UserData);
            }
            catch (Exception e)
            {
                Log.Error(e.Message + " " + e.Source + " " + e.StackTrace);
            }
        }

        public MySqlConnection DB;
        public QueryCB Callback;
        public String Query;
        public Object UserData;
    }

    public class Row
    {
        public Row(int FieldsCount)
        {
            Fields = new Dictionary<String, String>();
            FieldsCt = FieldsCount;
        }

        public void AddToRow(String FieldName, String FieldVal)
        {
            Fields.Add(FieldName, FieldVal);
        }

        private int GetFieldsCount()
        {
            return FieldsCt;
        }

        public String this[String Index]
        {
            get
            {
                return Fields[Index];
            }
        }

        public String this[int Index]
        {
            get
            {
                try
                {
                    Dictionary<String, String>.Enumerator en = Fields.GetEnumerator();

                    for (int i = 0; i < Index; i++)
                        en.MoveNext();

                    return en.Current.Value;
                }
                catch (Exception e)
                {
                    Log.Error("[Database Error] " + e.Message + " " + e.Source + " " + e.StackTrace);
                    return null;
                }

            }
        }

        private int FieldsCt;
        private Dictionary<String, String> Fields;
    }

    public class SQLDatabase
    {
        public SQLDatabase(String DBName, String Server, int Port, String Login, String Password)
        {
            try
            {
                string ConnectInfo = "Server=" + Server + ";" + "Port=" + Port + ";" + "Database=" + DBName + ";" + "User ID=" + Login + ";" + "Password=" + Password + ";";
                DB = new MySqlConnection(ConnectInfo);
                DB.Open();
                Log.Info(">> Successfully Connected to MySQL Server");
            }
            catch (MySqlException e)
            {
                Log.Error("Unable to connect to Database : " + e.Message + " " + e.Source + " " + e.StackTrace);
            }
            catch (Exception e)
            {
                Log.Error("Database Exception: " + e.Message + " " + e.Source + " " + e.StackTrace);
            }
        }

        public SQLDatabase(String Credentials)
        {
            try
            {
                string ConnectInfo = Credentials;
                DB = new MySqlConnection(ConnectInfo);
                DB.Open();
                Log.Info(">> Successfully Connected to MySQL Server");
            }
            catch (MySqlException e)
            {
                Log.Error("Unable to connect to Database : " + e.Message + " " + e.Source + " " + e.StackTrace);
            }
            catch (Exception e)
            {
                Log.Error("Database Exception : " + e.Message + " " + e.Source + " " + e.StackTrace);
            }
        }

        public String EscapeString(String Str)
        {
            return MySqlHelper.EscapeString(Str);
        }

        public List<Row> Execute(String Query)
        {
            try
            {
                MySqlCommand Request = new MySqlCommand(Query, DB);
                MySqlDataReader Result;

                List<Row> Ret = new List<Row>();

                // Check if query have a return value
                if (Query.StartsWith("SELECT ") == true)
                {
                    Result = Request.ExecuteReader();

                    while (Result.Read())
                    {
                        Row Res = new Row(Result.FieldCount);

                        for (int i = 0; i < Result.FieldCount; i++)
                            Res.AddToRow(Result.GetName(i), Result.GetValue(i).ToString());

                        Ret.Add(Res);
                    }

                    Result.Close();
                }
                else
                {
                    Request.ExecuteNonQuery();
                }

                return Ret;
            }
            catch (Exception e)
            {
                Log.Error("Error On Query: " + Query + "\n" + e.Message + " " + e.Source + " " + e.StackTrace);
                return new List<Row>();
            }
        }

        public void DelayedExecute(String Query, QueryCallback.QueryCB CallbackMethod, Object UserData)
        {
            QueryCallback CB = new QueryCallback();
            CB.Callback = CallbackMethod;
            CB.DB = DB;
            CB.Query = Query;
            CB.UserData = UserData;

            DatabaseWorker.BeginInvoke(() => { CB.Execute(); });
        }

        protected MySqlConnection DB;
    }
}
