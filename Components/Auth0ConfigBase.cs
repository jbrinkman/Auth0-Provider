#region Usings

using System;
using System.Globalization;

using DotNetNuke.Entities.Portals;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Authentication; 

#endregion

namespace Dnn.Authentication.Auth0.Components
{
    /// <summary>
    /// The Config class provides a central area for management of Module Configuration Settings.
    /// </summary>
    [Serializable]
    public class Auth0ConfigBase : AuthenticationConfigBase
    {
        private const string _cacheKey = "Authentication";

        protected Auth0ConfigBase(string service, int portalId)
            : base(portalId)
        {
            Service = service;

            APIKey = PortalController.GetPortalSetting(Service + "_APIKey", portalId, "");
            APISecret = PortalController.GetPortalSetting(Service + "_APISecret", portalId, "");
            Auth0Domain = PortalController.GetPortalSetting(Service + "_Auth0Domain", portalId, "");
            Enabled = PortalController.GetPortalSettingAsBoolean(Service + "_Enabled", portalId, false);
        }

        protected string Service { get; set; }

        public string APIKey { get; set; }

        public string APISecret { get; set; }

        public string Auth0Domain { get; set; }

        public bool Enabled { get; set; }

        private static string GetCacheKey(string service, int portalId)
        {
            return _cacheKey + "." + service + "_" + portalId;
        }

        public static void ClearConfig(string service, int portalId)
        {
            DataCache.RemoveCache(GetCacheKey(service, portalId));
        }

        public static Auth0ConfigBase GetConfig(string service, int portalId)
        {
            string key = GetCacheKey(service, portalId);
            var config = (Auth0ConfigBase)DataCache.GetCache(key);
            if (config == null)
            {
                config = new Auth0ConfigBase(service, portalId);
                DataCache.SetCache(key, config);
            }
            return config;
        }

        public static void UpdateConfig(Auth0ConfigBase config)
        {
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_APIKey", config.APIKey);
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_APISecret", config.APISecret);
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_Auth0Domain", config.Auth0Domain);
            PortalController.UpdatePortalSetting(config.PortalID, config.Service + "_Enabled", config.Enabled.ToString(CultureInfo.InvariantCulture));
            ClearConfig(config.Service, config.PortalID);
        }
    }
}
