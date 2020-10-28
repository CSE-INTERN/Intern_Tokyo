using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using DVD_Rental.sasaki_masayuki._0_common;

namespace DVD_Rental
{
    public class ConnectDB
    {
        public string [] auth(string id , string pw)
        {
            //String connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=DVDRentalDB;Integrated Security=true";
            String connectionString = null;
            C_Sasaki_Common.Generate_A_Strin_To_Connect_To_The_SQL(ref connectionString);
            SqlConnection objConn = new SqlConnection(connectionString);
            objConn.Open();

            string sql = "select [IsAdministrator] from [dbo].[User] where [LoginName] = '" + id + "' and [LoginPassword] = '" + pw + "'";

            SqlCommand sqlCommand = objConn.CreateCommand();
            sqlCommand.CommandText = sql;

            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            sqlCommand.Dispose();
            

            if (sqlDataReader.HasRows)
            {
                //IDとパスワードが一致した場合
                string[] array = {"" , "" };
                while(sqlDataReader.Read())
                {
                    array[0] = sqlDataReader["IsAdministrator"].ToString();
                    array[1] = id;
                }
                objConn.Close();
                return array;
            }
            else
            {
                //IDとパスワードの組み合わせがない場合
                string[] array = {"-1", id};
                objConn.Close();
                return array;
            }
        }
    }
}