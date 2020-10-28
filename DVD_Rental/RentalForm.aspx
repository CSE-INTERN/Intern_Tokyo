<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RentalForm.aspx.cs" Inherits="Rental_Form.RentalForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            <br />
        </div>
        &nbsp;
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" style="margin-left: 255px" Text="ログアウト" Width="122px" />
        <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 　&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="Label2" runat="server" Text="会員ID"></asp:Label>
        &nbsp;<asp:TextBox ID="TextBox1" runat="server" style="margin-top: 0px" OnTextChanged="TextBox1_TextChanged" TextMode ="Number"></asp:TextBox>
        <br />
        <asp:Label ID="Label3" runat="server" Text="商品一覧"></asp:Label>
        <asp:CheckBoxList ID="CheckBoxList1" runat="server" Height="21px" Width="383px">
        </asp:CheckBoxList>
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" style="margin-left: 135px" Text="選択した商品をレンタル" Width="250px" />
        <br />
    </form>
</body>
</html>
