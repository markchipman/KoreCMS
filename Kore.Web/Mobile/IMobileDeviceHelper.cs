using System.Web;
using Kore.Infrastructure;
using Kore.Security.Membership;
using Kore.Threading;
using Kore.Web.Configuration;

namespace Kore.Web.Mobile
{
    /// <summary>
    /// Mobile device helper interface
    /// </summary>
    public partial interface IMobileDeviceHelper
    {
        /// <summary>
        /// Returns a value indicating whether request is made by a mobile device
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Result</returns>
        bool IsMobileDevice(HttpContextBase httpContext);

        /// <summary>
        /// Returns a value indicating whether mobile devices support is enabled
        /// </summary>
        bool MobileDevicesSupported();

        /// <summary>
        /// Returns a value indicating whether current customer prefer to use full desktop version (even request is made by a mobile device)
        /// </summary>
        bool UserDontUseMobileVersion();
    }

    public partial class MobileDeviceHelper : IMobileDeviceHelper
    {
        #region Fields

        private readonly IWebWorkContext _workContext;

        #endregion Fields

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="workContext">Work context</param>
        public MobileDeviceHelper(IWebWorkContext workContext)
        {
            this._workContext = workContext;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Returns a value indicating whether request is made by a mobile device
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Result</returns>
        public virtual bool IsMobileDevice(HttpContextBase httpContext)
        {
            if (KoreWebConfigurationSection.Instance.Mobile.EmulateMobileDevice)
            {
                return true;
            }

            try
            {
                //comment the code below if you want tablets to be recognized as mobile devices.
                //kore uses the free edition of the 51degrees.mobi library for detecting browser mobile properties.
                //by default this property (IsTablet) is always false. you will need the premium edition in order to get it supported.
                bool isTablet = false;
                if (bool.TryParse(httpContext.Request.Browser["IsTablet"], out isTablet) && isTablet)
                {
                    return false;
                }

                if (httpContext.Request.Browser.IsMobileDevice)
                {
                    return true;
                }

                return false;
            }
            catch // FakeHttpContext will throw NotImplementedException
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether mobile devices support is enabled
        /// </summary>
        public virtual bool MobileDevicesSupported()
        {
            return KoreWebConfigurationSection.Instance.Mobile.MobileDevicesSupported;
        }

        /// <summary>
        /// Returns a value indicating whether current customer prefer to use full desktop version (even request is made by a mobile device)
        /// </summary>
        public virtual bool UserDontUseMobileVersion()
        {
            if (_workContext.CurrentUser == null)
            {
                return false;
            }

            var membershipService = EngineContext.Current.Resolve<IMembershipService>();
            string dontUseMobileVersion = AsyncHelper.RunSync(() => membershipService.GetProfileEntry(_workContext.CurrentUser.Id, MobileUserProfileProvider.Fields.DontUseMobileVersion));
            return !string.IsNullOrEmpty(dontUseMobileVersion) && bool.Parse(dontUseMobileVersion);
            //return _workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.DontUseMobileVersion, _tenantContext.CurrentStore.Id);
        }

        #endregion Methods
    }
}