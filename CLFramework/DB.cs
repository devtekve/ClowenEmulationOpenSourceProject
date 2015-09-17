using System;
using System.Data;
using System.Data.SqlClient;
namespace CLFramework
{
    public class DB
    {
        #region public
        public static SqlConnection connection;
        public SqlCommand Command;
        public string TempSQLQueryString;
        public SqlDataReader reader;

        public DB(string sql)
        {
            if (reader != null) { Close(); TempSQLQueryString = null; Command = null; }

            TempSQLQueryString = sql;
        }
        public SqlDataReader Read()
        {
            try
            {
                lock ("SQLREADER") // maybe that helps avoiding multithreading errors while executing queries, TEST
                {
                    Command = new SqlCommand(TempSQLQueryString, connection);
                    reader = Command.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(TempSQLQueryString, ex);
            }
            return reader;
        }
        public void Close()
        {
            if (reader != null && !reader.IsClosed)
            {
                reader.Close();
                reader.Dispose();
            }
        }

        public int Count()
        {
            int count = 0;
            try
            {
                da = new SqlDataAdapter(TempSQLQueryString, connection);
                DataSet ds = new DataSet();

                da.Fill(ds);
                if (ds.Tables.Count > 0)
                    count = ds.Tables[0].Rows.Count;
            }
            catch (Exception ex)
            {
                Log.Exception(TempSQLQueryString, ex);
            }
            return count;
        }


        public static SqlDataAdapter da;
        #endregion
        #region Baglanti
        public static void Connection(string connections)
        {
            string Connection = connections;
            if (connection != null)
                connection.Close();

            try
            {
                connection = new SqlConnection();

                connection.ConnectionString = Connection;
                connection.Open();
                Console.WriteLine("Database Connection SuccessFully");
            }
            catch (Exception ex)
            {
                Log.Exception(Connection, ex);
            }

        }
        #endregion
        #region Single data
        public static string GetData(string sql, string column)
        {
            string GetResults = null;

            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand(sql, connection);


            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetResults = reader[column].ToString();
                }

            }
            catch (Exception ex)
            {
                Log.Exception(sql, ex);
            }
            finally
            {
                if (!reader.IsClosed)
                    reader.Close();
            }
            return GetResults;
        }
        #endregion
        #region Rows Count
        public static int GetRowsCount(string command)
        {
            int count = 0;
            try
            {
                da = new SqlDataAdapter(command, connection);
                DataSet ds = new DataSet();
                da.Fill(ds);
                count = ds.Tables[0].Rows.Count;
                da.Dispose();
            }
            catch (Exception ex)
            {
                Log.Exception(command, ex);
            }
            return count;
        }
        #endregion
        public static void query(string command)
        {
            SqlCommand cmd = new SqlCommand(command, connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Exception(command, ex);
            }
        }
    }
}
