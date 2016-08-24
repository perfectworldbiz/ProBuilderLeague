using UnityEngine;
using System.Collections;
using System.Data.Sql;
using System.Data.SqlClient;

public class SQL_Controller : MonoBehaviour {

    string DatabaseInstantion;
    string DatabaseName;
    string Login;
    string Password;
    string conString;

    private void LoadConfig()
    {
        DatabaseInstantion = "sql2008.webio.pl,2401";
        DatabaseName = "zych_ProBuilderLeague";
        Login = "zych_ProBuilder";
        Password = "Supergra$6";
        conString = @"Data Source = " + DatabaseInstantion +
            ";user id = " + Login +
            ";password = " + Password +
            ";Initial Catalog = " + DatabaseName + ";";
    }
    SqlConnection ConnectWithSql()
    {
        LoadConfig();
        //return new SqlConnection(@"Data Source=" + DatabaseInstantion + ";database=" + DatabaseName + ";User id=" + Login + ";Password=" + Password + ";");
        return new SqlConnection(conString);
    }


    public string SimpleQuery(string _query)
    {
        LoadConfig();
        using (SqlConnection dbCon = new SqlConnection(conString))
        {
            SqlCommand cmd = new SqlCommand(_query, dbCon);
            try
            {
                dbCon.Open();
                string _returnQuery = (string)cmd.ExecuteScalar();
                return _returnQuery;
            }
            catch (SqlException _exception)
            {
                Debug.LogWarning(_exception.ToString());
                return null;
            }
        }
    }


    public string GetStringFromSQL()
    {
        string result = "error";
        SqlConnection Connection = ConnectWithSql();
        Connection.Open();
        Debug.Log(Connection.State);
        SqlCommand Command = Connection.CreateCommand();
        Command.CommandText = "select * from Artykuly2";
        SqlDataReader ThisReader = Command.ExecuteReader();
        while (ThisReader.Read())
        {
            result = ThisReader.GetString(0);
        }
        ThisReader.Close();
        Connection.Close();

        return result;
    }
}
