<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="return_confirmation.aspx.cs" Inherits="DVD_Rental.sasaki_masayuki._101_return_confirmation.return_confirmation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <%-- 1行目--%>
            <p>
            </p>

            <%-- 2行目--%>
            <p>
            </p>

            <%-- 3行目--%>
            <p>
                <asp:Label ID="Label1" runat="server" Text="以下の〇点を返却します"></asp:Label>
            </p>

            <%-- 4行目--%>
            <p>
                <%--リスト --%>
                <asp:BulletedList ID="BulletedList1" runat="server">
                </asp:BulletedList>
            </p>

            <%-- 5行目--%>
            <p>

                <asp:Button ID="Button1" runat="server" Text="キャンセル" OnClick="Button1_Click" />
                <asp:Button ID="Button2" runat="server" Text="確定" OnClick="Button2_Click" />

            </p>
        </div>
    </form>
</body>
</html>
