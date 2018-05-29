<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Login.ascx.cs" Inherits="Dnn.Authentication.Auth0.Login" %>

<li id="loginItem" runat="server" class="Auth0" >
    <asp:LinkButton runat="server" ID="loginButton" CausesValidation="False">
        <span><%=LocalizeString("LoginAuth0")%></span>
    </asp:LinkButton>
</li>
<li id="registerItem" runat="Server" class="Auth0">
    <asp:LinkButton ID="registerButton" runat="server" CausesValidation="False">
        <span><%=LocalizeString("RegisterAuth0") %></span>
    </asp:LinkButton>
</li>
