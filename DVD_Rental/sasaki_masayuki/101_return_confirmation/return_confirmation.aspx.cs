using DVD_Rental.sasaki_masayuki._0_common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DVD_Rental.sasaki_masayuki._101_return_confirmation
{
    public partial class return_confirmation : System.Web.UI.Page
    {
        private List<int> return_dvd_id_list = new List<int>();
        private int m_member_id;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Cookies["login"] != null)
            {
                if (Request.Cookies["login"].Value != "")
                {
                    if (Session[Request.Cookies["login"].Value] != null)
                    {
                        if (Session[Request.Cookies["login"].Value].ToString() == "0")
                        {
                            Response.Redirect("./../../RentalForm.aspx");
                        }
                    }
                    else
                    {
                        Session[Request.Cookies["login"].Value] = null;
                        Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("./../../login.aspx");
                    }
                }
                else
                {
                    Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                    Response.Redirect("./../../login.aspx");
                }
            }
            else
            {
                Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect("./../../login.aspx");
            }


            //dvdidの文字列取得
            String selected_dvd_id = Session["Select_DVD_ID"].ToString();
            //,で区切られたdvdidを分割
            string[] str_arr = selected_dvd_id.Split(',');

            //メンバーID格納
            m_member_id = int.Parse(Session["Member_ID"].ToString());

            //リストに格納
            int dvd_id_num = str_arr.Length;

            for (int i = 0; i < dvd_id_num; i++)
            {
                return_dvd_id_list.Add(int.Parse(str_arr[i]));
            }

            if (!IsPostBack)
            {
                //ポストバックじゃなければ

                //Listのクリア
                BulletedList1.Items.Clear();

                for (int i = 0; i < dvd_id_num; i++)
                {
                    string dvd_name = C_Sasaki_Common.Extract_The_Name_Of_The_DVD_That_Matches_The_DVD_ID(str_arr[i]);
                    BulletedList1.Items.Add(dvd_name);
                }

            }

            Label1.Text = "以下の" + BulletedList1.Items.Count + "点の商品を返却します。";
        }

        //キャンセルボタン
        protected void Button1_Click(object sender, EventArgs e)
        {
            Termination_Process();
            Redirect_Regression_Management();
        }

        //確定ボタン
        protected void Button2_Click(object sender, EventArgs e)
        {
            //返却管理画面時のUpdate_datetimeの取り出し。
            string management_time_stock_update_datetime_str = Session["management_time_stock_update_datetime"].ToString();
            string management_time_rental_update_datetime_str = Session["management_time_rental_update_datetime"].ToString();

            string[] management_time_stock_update_datetime = management_time_stock_update_datetime_str.Split(',');
            string[] management_time_rental_update_datetime = management_time_rental_update_datetime_str.Split(',');


            //dvdidの文字列取得
            String selected_dvd_id_str = Session["Select_DVD_ID"].ToString();
            //,で区切られたdvdidを分割
            string[] selected_dvd_id_arr = selected_dvd_id_str.Split(',');
            //リストに変換
            List<string> selected_dvd_id = new List<string>();
            selected_dvd_id.AddRange(selected_dvd_id_arr);

            //選択されたIDの取得
            string selected_id_str = Session["Selected_ID"].ToString();
            //Session.Remove("Selected_ID");
            //選択されたIDを配列に格納
            string[] selcted_id_arr = selected_id_str.Split(',');
            List<string> selected_id = new List<string>();
            selected_id.AddRange(selcted_id_arr);


            //返却確認画面時のUpdate_datetimeの取り出し
            List<string> confirmation_time_stock_update_datetime = C_Sasaki_Common.Get_Stock_Update_Date_Time_For_DVD_Id(selected_dvd_id);
            List<string> confirmation_time_rental_update_datetime = C_Sasaki_Common.Get_Rental_Update_Date_Time_For_Rental_Id(selected_id);

            //datetimeが一致するか確認
            //一致していない場合falseがはいるbool
            bool b_sync = true;
            //stockの数を比較の回数とする
            int check_num = confirmation_time_stock_update_datetime.Count;
            for (int i = 0; i < check_num; i++)
            {
                //管理画面時の更新日時と確認時の更新日時が一致しているかの判定をしていく


                if (management_time_stock_update_datetime[i] != confirmation_time_stock_update_datetime[i])
                {
                    //ストックの更新日時が一致していない場合

                    b_sync = false;
                }

                if (management_time_rental_update_datetime[i] != confirmation_time_rental_update_datetime[i])
                {
                    //レンタルの更新日時が一致していない場合

                    b_sync = false;
                }
            }

            if (b_sync)
            {
                //一致している場合

                int user_id = C_Sasaki_Common.Login_Name_To_Id(Request);
                int return_dvd_num = return_dvd_id_list.Count();
                for (int i = 0; i < return_dvd_num; i++)
                {
                    //日付を求める
                    DateTime dt = DateTime.Now;
                    
                    //StockDBの処理
                    {
                        //更新日時を更新
                        C_Sasaki_Common.Update_SQL("Update [dbo].[Stock] SET UpdateDateTime = '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' Where DVDId = " + return_dvd_id_list[i]);
                        //更新者を現在のユーザーに設定
                        C_Sasaki_Common.Update_SQL("Update [dbo].[Stock] SET UpdateUserId = " + user_id + " Where DVDId = " + return_dvd_id_list[i]);
                        //Stockを増やす。
                        C_Sasaki_Common.Update_SQL("Update [dbo].[Stock] SET Quantity += 1 Where DVDId = " + return_dvd_id_list[i]);
                    }

                    //RentalDBの処理
                    {
                        //更新日時を更新
                        C_Sasaki_Common.Update_SQL("Update [dbo].[Rental] SET UpdateDateTime = '" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' Where Id = " + selected_id[i]);
                        //更新者を現在のユーザーに設定
                        C_Sasaki_Common.Update_SQL("Update [dbo].[Rental] SET UpdateUserId = " + user_id + " Where Id = " + selected_id[i]);
                        //RentalDBのIDと選択されたIDが一致する物のIsReturnedフラグをtrueに設定。(※一つだけ)
                        C_Sasaki_Common.Update_SQL("Update [dbo].[Rental] SET IsReturned = 1 Where " + selected_id[i] + " =  Id");
                    }
                  
                }
            }
            else
            {
                //一致していない
                Session["confirmation_error"] = "返却確認時にエラー(競合)が発生しました。もう一度やり直してください。";
            }

            Termination_Process();
            Redirect_Regression_Management();
        }

        //返却管理画面に遷移する関数
        private void Redirect_Regression_Management()
        {
            Response.Redirect("/sasaki_masayuki/100_regression_management/regression_management.aspx");
        }

        //終了処理
        private void Termination_Process()
        {
            return_dvd_id_list.Clear();
        }


    }

}