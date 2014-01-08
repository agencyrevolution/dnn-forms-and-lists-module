using DotNetNuke.Entities.Users;

namespace DotNetNuke.Modules.UserDefinedTable.Helpers
{
    public class UserInfoHelpers
    {
        /// <summary>
        /// Get DisplayName by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetDisplayNameById(int userId)
        {
            var portalSettings = Entities.Portals.PortalController.GetCurrentPortalSettings();
            var userInfo = UserController.GetUserById(portalSettings.PortalId, userId);

            return userInfo == null ? null : userInfo.DisplayName;
        }
    }
}