using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Authentication;
using DotNetNuke.UI.WebControls;
using System.Globalization;

namespace Dnn.Authentication.Auth0.Components
{
    public class Auth0ConfigBase : AuthenticationConfigBase
    {

        #region Constants

        private const string _Service = "Auth0";
        private const string _cacheKey = "Authentication";
        private const string _ClientId = "_ClientId";
        private const string _ClientSecret = "_ClientSecret";
        private const string _ConnectionName = "_ConnectionName";
        private const string _TenantDomain = "_TenantDomain";
        private const string _Enabled = "_Enabled";

        #endregion

        protected Auth0ConfigBase(int portalId) : base(portalId)
        {
            ClientId = PortalController.GetPortalSetting(_Service + _ClientId, portalId, "");
            ClientSecret = PortalController.GetPortalSetting(_Service + _ClientSecret, portalId, "");
            ConnectionName = PortalController.GetPortalSetting(_Service + _ConnectionName, portalId, "");
            TenantDomain = PortalController.GetPortalSetting(_Service + _TenantDomain, portalId, "");
            Enabled = PortalController.GetPortalSettingAsBoolean(_Service + _Enabled, portalId, false);
        }

        #region Properties

        [SortOrder(1)]
        public string ClientId { get; set; }

        [SortOrder(2)]
        public string ClientSecret { get; set; }

        [SortOrder(3)]
        public string ConnectionName { get; set; }

        [SortOrder(0)]
        public string TenantDomain { get; set; }

        public bool Enabled { get; set; }

        #endregion

        private static string GetCacheKey(int portalId)
        {
            return $"{_cacheKey}.{_Service}_{portalId}";
        }

        public static void ClearConfig(int portalId)
        {
            DataCache.RemoveCache(GetCacheKey(portalId));
        }

        public static Auth0ConfigBase GetConfig(int portalId)
        {
            string key = GetCacheKey(portalId);
            var config = (Auth0ConfigBase)DataCache.GetCache(key);
            if (config == null)
            {
                config = new Auth0ConfigBase(portalId);
                DataCache.SetCache(key, config);
            }
            return config;
        }

        public static void UpdateConfig(Auth0ConfigBase config)
        {
            PortalController.UpdatePortalSetting(config.PortalID, _Service + _ClientId, config.ClientId);
            PortalController.UpdatePortalSetting(config.PortalID, _Service + _ClientSecret, config.ClientSecret);
            PortalController.UpdatePortalSetting(config.PortalID, _Service + _ConnectionName, config.ConnectionName);
            PortalController.UpdatePortalSetting(config.PortalID, _Service + _TenantDomain, config.TenantDomain);
            PortalController.UpdatePortalSetting(config.PortalID, _Service + _Enabled, config.Enabled.ToString(CultureInfo.InvariantCulture));
            ClearConfig(config.PortalID);
        }
    }
}