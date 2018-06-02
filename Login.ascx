<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Login.ascx.cs" Inherits="Dnn.Authentication.Auth0.Login" %>

<asp:LinkButton runat="server" ID="loginButton" CausesValidation="False" CssClass="Auth0-Login">
    <span><%=LocalizeString("LoginAuth0")%></span>
</asp:LinkButton>
