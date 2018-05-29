#region Usings

using Dnn.Authentication.Auth0.Components;
using System;

#endregion

namespace Dnn.Authentication.Auth0
{
    public partial class Settings : Auth0SettingsBase
    {
        protected override string AuthSystemApplicationName
        {
            get { return "Auth0"; }
        }
    }
}