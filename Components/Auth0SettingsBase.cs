#region Usings

using DotNetNuke.Services.Authentication;
using DotNetNuke.UI.WebControls;
using System;

#endregion

namespace Dnn.Authentication.Auth0.Components
{
    public class Auth0SettingsBase : AuthenticationSettingsBase
    {
        protected PropertyEditorControl SettingsEditor;

        protected virtual string AuthSystemApplicationName { get { return String.Empty; } }

        public override void UpdateSettings()
        {
            if (SettingsEditor.IsValid && SettingsEditor.IsDirty)
            {
                var config = (Auth0ConfigBase)SettingsEditor.DataSource;
                Auth0ConfigBase.UpdateConfig(config);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Auth0ConfigBase config = Auth0ConfigBase.GetConfig(AuthSystemApplicationName, PortalId);
            SettingsEditor.DataSource = config;
            SettingsEditor.DataBind();
        }
    }
}
