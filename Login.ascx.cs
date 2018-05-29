#region Usings

using System;
using System.Collections.Specialized;
using Dnn.Authentication.Auth0.Components;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Authentication.OAuth;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;

#endregion

namespace Dnn.Authentication.Auth0
{
    public partial class Login : Auth0LoginBase
    {
        protected override string AuthSystemApplicationName
        {
            get { return "Auth0"; }
        }

        public override bool SupportsRegistration
        {
            get { return true; }
        }

        protected override UserData GetCurrentUser()
        {
            return Auth0Client.GetCurrentUser<Auth0UserData>();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            loginButton.Click += loginButton_Click;
            registerButton.Click += loginButton_Click;

            Auth0Client = new Auth0Client(PortalId, Mode);

            loginItem.Visible = (Mode == AuthMode.Login);
            registerItem.Visible = (Mode == AuthMode.Register);
        }

        protected override void AddCustomProperties(NameValueCollection properties)
        {
            base.AddCustomProperties(properties);

            //properties.Add("Auth0", Auth0Client.GetCurrentUser<Auth0UserData>().Link.ToString());
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            AuthorisationResult result = Auth0Client.Authorize();
            if (result == AuthorisationResult.Denied)
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("PrivateConfirmationMessage", Localization.SharedResourceFile), ModuleMessage.ModuleMessageType.YellowWarning);
                
            }
        }
    }
}