using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using DVD_Rental.sasaki_masayuki._0_common;

namespace Rental_Form
{
    public partial class RentalConfirm : System.Web.UI.Page
    {

        int Id;
        List<int> DvdID = new List<int>();
        List<string> DvdID_str = new List<string>();
        int MemberID;
        DateTime InsertDateTime;

        protected void Page_Load(object sender, EventArgs e)
        {
            // ログイン関連処理
            if (Request.Cookies["login"] != null)
            {
                if (Request.Cookies["login"].Value != "")
                {
                    if (Session[Request.Cookies["login"].Value] != null)
                    {
                        if (Session[Request.Cookies["login"].Value].ToString() == "1")
                        {
                            Response.Redirect("/sasaki_masayuki/100_regression_management/regression_management.aspx");
                        }
                    }
                    else
                    {
                        Session[Request.Cookies["login"].Value] = null;
                        Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("./login.aspx");
                    }
                }
                else
                {
                    Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                    Response.Redirect("./login.aspx");
                }
            }
            else
            {
                Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect("./login.aspx");
            }


            Button1.Text = "キャンセル";
            Button2.Text = "確定";



            // 入力されたメンバーIDを取得
            MemberID = int.Parse(Session["MemberID"].ToString());

            if (!IsPostBack)
            {

                // ポストバックじゃなければ
                BulletedList1.Items.Clear();

                // レンタルしたDVDを名前からIDに変換し、リストに追加
                for (int i = 1; i <= 12; i++)
                {
                    if (Session["DVD" + i.ToString()] != null)
                    {
                        if (Session["DVD" + i.ToString()].ToString() != "")
                        {

                            BulletedList1.Items.Add(Session["DVD" + i.ToString()] as string);
                            DvdID.Add(int.Parse(C_Sasaki_Common.Convert_DVD_Name_To_ID(Session["DVD" + i.ToString()].ToString())));
                            DvdID_str.Add(C_Sasaki_Common.Convert_DVD_Name_To_ID(Session["DVD" + i.ToString()].ToString()));

                        }
                    }

                }
                Session["DvdID"] = string.Join(",", DvdID);
            }

            Label1.Text = "以下の" + BulletedList1.Items.Count + "点の商品をレンタルします";

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // キャンセルボタン
            Response.Redirect("RentalForm.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            // 確定ボタン
            if (Session["DvdID"] != null)
            {
                //セッションの中身がnullじゃない場合

                string dvd_id = null;
                dvd_id = Session["DvdID"].ToString();
                string[] dvd_id_arr = dvd_id.Split(',');
                foreach (string i in dvd_id_arr)
                {
                    DvdID.Add(int.Parse(i));
                }
            }            
            
            InsertDateTime = DateTime.Now;

            string connection_csring = null;
            C_Sasaki_Common.Generate_A_Strin_To_Connect_To_The_SQL(ref connection_csring);
            SqlConnection sql_connection = new SqlConnection(connection_csring);
            sql_connection.Open();
            SqlCommand sqlcommand = sql_connection.CreateCommand();

            // Rentalの最後のIDを取得
            string max_id = null;
            C_Sasaki_Common.Select_SQL("Select MAX(Id) as Id  From [dbo].[Rental]", "Id", ref max_id);
            Id = int.Parse(max_id);

            int limit = DvdID.Count();

            // ログインしているユーザーIDを持ってくる
            int InsertUserID = C_Sasaki_Common.Login_Name_To_Id(Request);

            // レンタル画面時のUpdateDateTimeを格納
            string RentalForm_Stock_UpdateDateTime_str = Session["RentalForm_Stock_UpdateDateTime"].ToString();
            string[] RentalForm_UpdateDateTime = RentalForm_Stock_UpdateDateTime_str.Split(',');

            List<string> Stock_Quantity_str = new List<string>();
            C_Sasaki_Common.Select_SQL("Select * From [dbo].[Stock] ", "Quantity", Stock_Quantity_str);


            // レンタル確認画面時のUpdateDateTimeを格納
            List<string> Confirm_Stock_UpdateDateTime = C_Sasaki_Common.Get_Stock_Update_Date_Time_For_DVD_Id(DvdID_str);

            //datetimeが一致するか確認
            //一致していない場合falseがはいるbool
            bool b_sync = true;

            //stockの数を比較の回数とする
            int check_num = Confirm_Stock_UpdateDateTime.Count;
            for (int i = 0; i < check_num; i++)
            {
                //管理画面時の更新日時と確認時の更新日時が一致しているかの判定をしていく


                if (RentalForm_UpdateDateTime[i] != Confirm_Stock_UpdateDateTime[i])
                {
                    //ストックの更新日時が一致していない場合

                    b_sync = false;
                }

            }

            if (b_sync)
            {
                // 日時を求める
                DateTime dt = DateTime.Now;

                for (int i = 0; i < limit; i++)
                {
                    // レンタルしたものをインサートする
                    sqlcommand.CommandText = Up_To_Insert_SQL() +
                            "( "
                            + (Id + i + 1) + ","                                                      // ID
                            + MemberID + ","                                                          // MemberID
                            + DvdID[i] + ","                                                          // DVDID
                            + 0 + ","                                                                 // IsReturned
                            + "'" + InsertDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" + ","    // InsertDateTime
                            + InsertUserID + ","                                                      // InsertUserID　ログインユーザーID 
                            + "NULL" + ","                                                            // UpdateDateTime
                            + "NULL" +                                                                // UpdateDateUserID
                            " )";

                    //StockDBの処理
                    {
                        //更新日時を更新
                        C_Sasaki_Common.Update_SQL("Update [dbo].[Stock] SET UpdateDateTime = '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' Where DVDId = " + DvdID[i]);
                        //更新者を現在のユーザーに設定
                        C_Sasaki_Common.Update_SQL("Update [dbo].[Stock] SET UpdateUserId = " + InsertUserID + " Where DVDId = " + DvdID[i]);

                        if (DvdID[i] != 11)
                        {
                            if (Stock_Quantity_str[DvdID[i]-1] != "0")
                                C_Sasaki_Common.Update_SQL("Update[dbo].[Stock] SET Quantity -= 1 Where DVDId = " + DvdID[i]);
                        }
                        else
                        {
                            if (Stock_Quantity_str[9] != "0")
                                C_Sasaki_Common.Update_SQL("Update[dbo].[Stock] SET Quantity -= 1 Where DVDId = " + DvdID[i]);


                        }

                    }

                    sqlcommand.ExecuteNonQuery();
                }

            }
            else
            {
                //一致していない
                Session["confirmation_error"] = "レンタル確認時にエラー(競合)が発生しました。もう一度やり直してください。";
            }
            sqlcommand.Dispose();

            sql_connection.Close();
            sql_connection.Dispose();

            Response.Redirect("RentalForm.aspx");


        }

        //InsertのSQLのValueまでの文字列
        private string Up_To_Insert_SQL()
        {
            string temp = null;
            temp = "INSERT INTO [dbo].[Rental] ([Id],[MemberId],[DvDId],[IsReturned],[InsertDateTime],[InsertUserId],[UpdateDateTime],[UpdateUserId]) VALUES";
            return temp;

        }

    }

}

