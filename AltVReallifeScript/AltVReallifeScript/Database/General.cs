using AltV.Net;
using AltVReallifeScript.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltVReallifeScript.Database
{
    public class General : IScript
    {

        public static bool CheckMySqlConnection()
        {
            MySqlConnection mysql = new MySqlConnection(Variables.MySqlConnectionString);
            try
            {
                mysql.Open();
            }   
            catch(Exception e)
            {
                Alt.Log($"{Variables.Prefix} " + e.Message);
                return false;
            } 
            

            if (mysql.State.ToString() == "Open")
                return true;
            else
                return false;
      
        }
    } 
}
