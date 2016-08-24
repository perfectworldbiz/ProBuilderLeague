using UnityEngine;
using System.Collections;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;

public class KodSQL_Unity : MonoBehaviour {

    #region Global Variables
    //Database
    string DatabaseInstantion;
    string DatabaseName;
    string Login;
    string Password;
    #endregion Global Variables
    #region Database
    private void LoadConfig()
    {
        DatabaseInstantion = "sql2008.webio.pl,2401";
        DatabaseName = "zych_ProBuilderLeague";
        Login = "zych_ProBuilder";
        Password = "Supergra$6";
    }
    SqlConnection ConnectWithSql()
    {
        LoadConfig();
        return new SqlConnection(@"Data Source=" + DatabaseInstantion + ";database=" + DatabaseName + ";User id=" + Login + ";Password=" + Password + ";");
    }
    #endregion

    public string GetStringFromSQL()
    {
        string result = "error";
        SqlConnection Connection = ConnectWithSql();
        Connection.Open();
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
