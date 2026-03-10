using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domain.Constants
{
    public static class Permissions
    {
        // ==========================================
        // 1. Organization & Settings
        // ==========================================
        public static class Organization
        {
            public const string View = "Permissions.Organization.View";
            public const string Edit = "Permissions.Organization.Edit";
            public const string Delete = "Permissions.Organization.Delete";
            public const string ManageSettings = "Permissions.Organization.ManageSettings";
        }

        // ==========================================
        // 2. Members & HR
        // ==========================================
        public static class Members
        {
            public const string View = "Permissions.Members.View";
            public const string Invite = "Permissions.Members.Invite";
            public const string Remove = "Permissions.Members.Remove";
            public const string EditProfile = "Permissions.Members.EditProfile";
        }

        // ==========================================
        // 3. Roles & Access Control
        // ==========================================
        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Create = "Permissions.Roles.Create";
            public const string Edit = "Permissions.Roles.Edit";
            public const string Delete = "Permissions.Roles.Delete";
            public const string Assign = "Permissions.Roles.Assign";
        }

        // ==========================================
        // 4. Market & Products (MarketItems)
        // ==========================================
        public static class Market
        {
            public const string ViewProducts = "Permissions.Market.ViewProducts";
            public const string CreateProduct = "Permissions.Market.CreateProduct";
            public const string EditProduct = "Permissions.Market.EditProduct";
            public const string DeleteProduct = "Permissions.Market.DeleteProduct";
        }

        // ==========================================
        // 5. Auctions & Bidding
        // ==========================================
        public static class Auctions
        {
            public const string View = "Permissions.Auctions.View";
            public const string Create = "Permissions.Auctions.Create";
            public const string Manage = "Permissions.Auctions.Manage"; // مثل إغلاق المزاد أو تعديله
            public const string Bid = "Permissions.Auctions.Bid"; // المشاركة والمزايدة
        }

        // ==========================================
        // 6. Farms & Assets (أصول المؤسسة)
        // ==========================================
        public static class Farms
        {
            public const string View = "Permissions.Farms.View";
            public const string Create = "Permissions.Farms.Create";
            public const string Edit = "Permissions.Farms.Edit";
            public const string Delete = "Permissions.Farms.Delete";
        }

        // ==========================================
        // 7. Crops & Activities (المحاصيل والعمليات)
        // ==========================================
        public static class Crops
        {
            public const string View = "Permissions.Crops.View";
            public const string ManageActivities = "Permissions.Crops.ManageActivities"; // ري وتسميد وغيرها
            public const string Harvest = "Permissions.Crops.Harvest"; // الحصاد
        }

        // ==========================================
        // Helper Method: Get All Permissions in the System
        // ==========================================
        /// <summary>
        /// هذه الدالة تقوم بجمع كافة الصلاحيات من جميع الكلاسات الداخلية أوتوماتيكياً
        /// </summary>
        public static List<string> GetAllPermissions()
        {
            var permissions = new List<string>();

            // الحصول على جميع الكلاسات الداخلية (Nested Classes) داخل كلاس Permissions
            var nestedClasses = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var nestedClass in nestedClasses)
            {
                // الحصول على جميع الثوابت (const strings) داخل كل كلاس داخلي
                var fields = nestedClass.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                foreach (var field in fields)
                {
                    if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                    {
                        var value = field.GetValue(null)?.ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            permissions.Add(value);
                        }
                    }
                }
            }

            return permissions;
        }
    }
}