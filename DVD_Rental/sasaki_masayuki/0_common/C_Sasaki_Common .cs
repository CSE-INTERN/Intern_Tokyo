using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Collections.Specialized;

namespace DVD_Rental.sasaki_masayuki._0_common
{
    public class C_Sasaki_Common
    {

        //例外をデバッグ出力します。
        public static void Catch_An_Exception(Exception e)
        {
            System.Diagnostics.Debug.WriteLine("例外をキャッチ");
            System.Diagnostics.Debug.Write(e.Message);
            System.Diagnostics.Debug.WriteLine("\n");
        }

        private static string Get_SQL_Connection_Str_Ver_Remote_SV()
        {
            string sConnectlonStrlng =
                    "Addr = 192.168.10.201;"
                + "User Id = sa;"
                + "password = P@ssw0rd;"
                + "Initial Catalog = DVDRentalDB;"
                + "Integrated Security = false";
            return sConnectlonStrlng;
        }

        private static string Get_SQL_Connection_Str_Ver_Local_SV()
        {
            string connection_csring = "Data Source=.\\SQLEXPRESS;Initial Catalog=DVDRentalDB;Integrated Security=True";
            return connection_csring;
        }

        //SQLに接続するための文字列を生成する関数
        //[out] ref string(a_connection_str_storehouse) 格納先
        public static void Generate_A_Strin_To_Connect_To_The_SQL(ref string a_connection_str_storehouse)
        {
            //ローカルサーバーなら
            {
                a_connection_str_storehouse = Get_SQL_Connection_Str_Ver_Local_SV();
            }
            //リモートサーバーなら
            {
                //a_connection_str_storehouse = Get_SQL_Connection_Str_Ver_Remote_SV();
            }
            
        }

        //SQLのSELECT実行関数(データリーダ)※一つの項目しか取得できないのでおすすめしません。またsql_strにselect以外の命令はしないでください。
        //[in] sql_str(string) SQL文を書いて下さい。
        //[in] a_in_want(string) 取得したい項目
        //[out] a_out_want(List<string>) 取得した項目
        public static void Select_SQL(string sql_str, string a_in_want, List<string> a_out_want)
        {
            try
            {
                string connection_csring = null;
                Generate_A_Strin_To_Connect_To_The_SQL(ref connection_csring);
                SqlConnection sql_connection = new SqlConnection(connection_csring);

                sql_connection.Open();
                SqlCommand sqlCommand = sql_connection.CreateCommand();

                //sqlCommand.CommandText = "Select * From [dbo].[BelongTo] ";
                sqlCommand.CommandText = sql_str;
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                sqlCommand.Dispose();


                while (sqlDataReader.Read())
                {
                    a_out_want.Add(sqlDataReader[a_in_want].ToString());
                }

                sqlDataReader.Close();
                sql_connection.Close();
                sql_connection.Dispose();
            }
            catch (Exception exec)
            {
                C_Sasaki_Common.Catch_An_Exception(exec);
            }
        }

        //SQLの実行関数(データリーダ)※一つの項目しか取得できないのでおすすめしません。
        //[in] sql_str(string) SQL文を書いて下さい。
        //[in] a_in_want(string) 取得したい項目
        //[out] a_out_want(string) 取得した項目
        public static void Select_SQL(string sql_str, string a_in_want,ref string a_out_want)
        {
            try
            {
                string connection_csring = null;
                Generate_A_Strin_To_Connect_To_The_SQL(ref connection_csring);
                SqlConnection sql_connection = new SqlConnection(connection_csring);

                sql_connection.Open();
                SqlCommand sqlCommand = sql_connection.CreateCommand();

                //sqlCommand.CommandText = "Select * From [dbo].[BelongTo] ";
                sqlCommand.CommandText = sql_str;
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                sqlCommand.Dispose();


                while (sqlDataReader.Read())
                {
                    a_out_want = sqlDataReader[a_in_want].ToString();
                }

                sqlDataReader.Close();
                sql_connection.Close();
                sql_connection.Dispose();
            }
            catch (Exception exec)
            {
                C_Sasaki_Common.Catch_An_Exception(exec);
            }
        }

        //SQLにUpdateを実行する関数
        //[in] a_sql_str(string) Update文
        //Update例"Update [dbo].[BelongTo]  SET BelongID = 10  WHERE BelongID = 12 "
        public static void Update_SQL(string a_sql_str)
        {
            try 
            {
                string connection_csring = null;
                Generate_A_Strin_To_Connect_To_The_SQL(ref connection_csring);
                SqlConnection sql_connection = new SqlConnection(connection_csring);

                sql_connection.Open();
                SqlCommand sqlCommand = sql_connection.CreateCommand();


                sqlCommand.CommandText = a_sql_str;
                int num = sqlCommand.ExecuteNonQuery();

                sqlCommand.Dispose();

                sql_connection.Close();
                sql_connection.Dispose();
            }
            catch(Exception exec)
            {
                C_Sasaki_Common.Catch_An_Exception(exec);
            }
           
        }

        //DVDの名前からIDに変換します。
        //[in] a_dvd_name(string) DVDの名前
        //return DVDのID
        public static string Convert_DVD_Name_To_ID(string a_dvd_name)
        {
            //変換したIDの格納先
            string name_to_id_storehouse = null;

            //dvdの名前とidの変換
            C_Sasaki_Common.Select_SQL("Select * From [dbo].[DVD] Where Name = '" + a_dvd_name  + "'", "Id", ref name_to_id_storehouse);

            return name_to_id_storehouse;
        }

        //入力されたメンバーIDの未返却のDVD_IDを抽出
        //[in] a_inputed_member_id(string) 入力されたメンバーID
        //return 未返却のDVD_ID(IDの重複有り)
        public static List<string> Extract_The_Unreturned_DVD_IDs_Of_The_Entered_Member_ID(string a_inputed_member_id)
        {
            //抽出したdvdidデータの格納先
            List<string> dvdid_storehouse = new List<string>();

            //入力されたメンバーIDと一致する物で未返却のDvdIdを抽出
            C_Sasaki_Common.Select_SQL("Select * From [dbo].[Rental] Where MemberId = " + a_inputed_member_id + "AND IsReturned = 0", "DvdId", dvdid_storehouse);

            return dvdid_storehouse;
        }

        //DVD_IDと一致するDVDの名前を抽出
        //[in] a_dvd_id(string) DVDのID
        //return string DVDの名前
        public static string Extract_The_Name_Of_The_DVD_That_Matches_The_DVD_ID(string a_dvd_id)
        {
            //抽出したDVDの名前の格納先
            string dvd_name_storehouse = null;

            //抽出したDvdIdと一致するDVDの名前を抽出
            C_Sasaki_Common.Select_SQL("Select * From [dbo].[DVD] Where " + a_dvd_id + " = Id ", "Name", ref dvd_name_storehouse);

            return dvd_name_storehouse;
        }

        //ログインネームからIDに変換します。
        //[in] a_request(HttpRequest) 
        //return string IDが出力されます。
        public static int Login_Name_To_Id(HttpRequest a_request)
        {
            //変換した物の出力先
            string id_storehouse = null;
            C_Sasaki_Common.Select_SQL("Select * From [dbo].[User] Where LoginName = '" + a_request.Cookies["login"].Value + "'", "Id", ref id_storehouse);
            return int.Parse(id_storehouse);
        }

        //更新日時のゲット
        //[in] string(a_db_name) データベースの名前 dboとか[]とかはいらない。純粋に名前。
        //[in] string(a_where) どの箇所をゲットするか。
        //[out] string(a_storehouse)　取得したDateTimeの格納先。※複数あった場合は最後にSelectされた一つしか取得できません。
        public static void SQL_Get_Update_Date_Time(string a_db_name, string a_where, ref string a_storehouse)
        {
            C_Sasaki_Common.Select_SQL("Select * From [dbo].[" + a_db_name + "] Where " + a_where, "UpdateDateTime",ref a_storehouse);
        }

        //Dvd_IDからStockのUpdateDateTimeを求める
        //[in] List<int>(a_selected_dvd_id) 選択されたdvd_id群
        //return List<string> a_selected_dvd_idの順に格納されたUpdateDateTime
        public static List<string> Get_Stock_Update_Date_Time_For_DVD_Id(List<string> a_selected_dvd_id)
        {
            //ストックDBのUpdateDateTimeの格納先。
            List<string> Stock_Update_Date_Time_Storehouse = new List<string>();

            int selected_id_num = a_selected_dvd_id.Count;
            for (int i = 0; i < selected_id_num; i++)
            {
                string temp = null;
                C_Sasaki_Common.SQL_Get_Update_Date_Time("Stock", "DVDId = " + a_selected_dvd_id[i], ref temp);
                Stock_Update_Date_Time_Storehouse.Add(temp);
            }

            return Stock_Update_Date_Time_Storehouse;
        }

        //RentalDBのIdからUpdate_Date_Timeを求める
        //[in] List<int>(a_selected_rentaldb_id) 選択されたレンタルDBのid群
        //return List<string> a_selected_rentaldb_idの順に格納されたUpdateDateTime
        public static List<string> Get_Rental_Update_Date_Time_For_Rental_Id(List<string> a_selected_rentaldb_id)
        {
            //ストックDBのUpdateDateTimeの格納先。
            List<string> Update_Date_Time_Storehouse = new List<string>();

            int selected_id_num = a_selected_rentaldb_id.Count;
            for (int i = 0; i < selected_id_num; i++)
            {
                string temp = null;
                C_Sasaki_Common.SQL_Get_Update_Date_Time("Rental", "Id = " + a_selected_rentaldb_id[i], ref temp);
                Update_Date_Time_Storehouse.Add(temp);
            }

            return Update_Date_Time_Storehouse;
        }

    }
}