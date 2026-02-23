using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Constants
{
    public class Permissions
    {

    }
    public static class Farms
    {
        public const string View = "Permissions.Farms.View";
        public const string Create = "Permissions.Farms.Create";
        public const string Edit = "Permissions.Farms.Edit";
        public const string Delete = "Permissions.Farms.Delete";
    }
    public static class OrganizationsPermissions
    {
        public const string ManageOrganization = "MANAGE_ORGANIZATION";
        public const string CreateRole = "CREATE_ROLE";
        public const string AssignRole = "ASSIGN_ROLE";
        public const string InviteMember = "INVITE_MEMBER";
        public const string BidOnProduct = "BID_ON_PRODUCT";
        public const string CreateAuction = "CREATE_AUCTION";
        public static List<string> GetAll()
        {
            return typeof(OrganizationsPermissions)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Select(f => f.GetValue(null)!.ToString()!)
                .ToList();
        }

    }

    public static class Crops
    {
        public const string View = "Permissions.Crops.View";
        public const string ManageActivities = "Permissions.Crops.ManageActivities"; // ري وتسميد
        public const string Harvest = "Permissions.Crops.Harvest";
    }

    public static class Employees
    {
        public const string View = "Permissions.Employees.View";
        public const string Invite = "Permissions.Employees.Invite";
        public const string ManageRoles = "Permissions.Employees.ManageRoles";
    }
}
