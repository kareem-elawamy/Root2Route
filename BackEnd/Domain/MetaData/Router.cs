namespace Domain.MetaData
{
    public static class Router
    {
        public const string plantNameRoute = "/{plantName}";
        public const string singelroute = "/{id}";
        public const string root = "api";
        public const string varsion = "v1";

        public const string rule = root + "/" + varsion;

        public static class Authentication
        {
            public const string Prefix = rule + "/auth";
            public const string Regsiter = Prefix + "/register"; // تعديل شكل اللينك ليكون أوضح
            public const string Login = Prefix + "/login";
            public const string verifyOtp = Prefix + "/verify-otp";
            public const string resendOtp = Prefix + "/resend-otp";
        }

        public static class PlantInfo
        {
            public const string Prefix = rule + "/plant-info";
            public const string GetAllPlantInfos = Prefix + "/all";
            public const string CreatePlantInfo = Prefix + "/create";
            public const string EditPlantInfo = Prefix + "/edit";
            public const string DeletePlantInfo = Prefix + "/delete";
            public const string Paginated = Prefix + "/paginated";
        }

        public static class PlantGuideStep
        {
            public const string Prefix = rule + "/plant-guide-steps";
            public const string GetAllPlantGuideSteps = Prefix + "/all";
            public const string GetPlantGuideStepById = Prefix + singelroute;
            public const string GetPlantGuideStepsByPlantId = Prefix + "/by-plant" + singelroute;
            public const string GetPlantGuideStepsByPlantName = Prefix + "/by-name" + plantNameRoute;
        }

        public static class Product
        {
            public const string Prefix = rule + "product";

            public const string AddProduct = Prefix + "/Add";
            public const string GetAll = Prefix + "/GetAll";
            public const string GetById = Prefix + "/{id}";
            public const string ChangeStatus = Prefix + "/ChangeStatus";
            // 👇 المسارات الجديدة
            public const string UpdateProduct = Prefix + "/Update";
            public const string DeleteProduct = Prefix + "/Delete/{id}";
            public const string GetByOrgId = Prefix + "/Organization/{organizationId}";
        }
        public static class Order
        {
            public const string Prefix = rule + "order";

            public const string CreateOrder = Prefix + "/Create";
            public const string GetMyOrders = Prefix + "/MyOrders";
            public const string GetOrderById = Prefix + "/{id}";
            public const string ChangeStatus = Prefix + "/ChangeStatus";
        }

        public static class OrganizationRole
        {
            public const string Prefix = rule + "/organization-roles";
            public const string CreateOrganizationRole = Prefix + "/create";
            public const string GetOrganizationRolesByOrganizationId = Prefix + "/by-organization/{organizationId}"; // تم إصلاح الـ Slash الناقصة هنا
            public const string GetSystemPermissions = Prefix + "/system-permissions";
        }

        public static class Organization
        {
            public const string Prefix = rule + "/organizations";
            public const string GetAllOrganizations = Prefix;
            public const string GetById = Prefix + singelroute;
            public const string GetMyOrganizations = Prefix + "/my";
            public const string CreateOrganization = Prefix;
            public const string UpdateById = Prefix;
            public const string SoftDelete = Prefix + singelroute;
            public const string Restore = Prefix + singelroute + "/restore";
            public const string ChangeOwner = Prefix + singelroute + "/change-owner";
            public const string UpdateStatus = Prefix + singelroute + "/status";
            public const string UploadLogo = Prefix + singelroute + "/logo";
            public const string GetStatistics = Prefix + singelroute + "/statistics";
        }

        public static class OrganizationMember
        {
            public const string Prefix = rule + "/organization-members";
            public const string AddOrganizationMember = Prefix + "/add";
            public const string GetOrganizationMembersByOrganizationId = Prefix + "/by-organization/{organizationId}";
            public const string RemoveOrganizationMember = Prefix + "/{organizationMemberId}/remove";
        }

        public static class AnalyzePlants
        {
            public const string Prefix = rule + "/model-analysis";
            public const string AnalyzePlant = Prefix + "/analyze";
        }

        public static class OrganizationInvitation
        {
            public const string Prefix = rule + "/organization-invitations";
            public const string SendInvitation = Prefix + "/send";
            public const string GetAllInvitationsForUser = Prefix + "/my";
            public const string AcceptInvitation = Prefix + "/accept";
            public const string Revoken = Prefix + "/revoke"; // تم تعديل revoken لـ revoke كفعل صحيح
        }
    }
}