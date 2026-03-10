namespace Domain.MetaData
{
    public static class Router
    {
        public const string plantNameRoute = "/{plantName}";
        public const string singelroute = "/{id}";
        public const string root = "api";
        public const string varsion = "v1";
        public const string rule = root + "/" + varsion + "/";

        public static class Authentication
        {
            public const string Prefix = rule + "authentication/";
            public const string Regsiter = Prefix + "regsiter";
            public const string Login = Prefix + "Login";
            public const string verifyOtp = Prefix + "verify-otp";
            public const string resendOtp = Prefix + "resend-otp";
        }
        public static class PlantInfo
        {
            public const string Prefix = rule + "plantinfo/";
            public const string GetAllPlantInfos = Prefix + "getallplantinfos";
            public const string CreatePlantInfo = Prefix + "createplantinfo";
            public const string EditPlantInfo = Prefix + "editplantinfo";
            public const string DeletePlantInfo = Prefix + "deleteplantinfo";
            public const string Paginated = Prefix + "paginated";

        }
        public static class PlantGuideStep
        {
            public const string Prefix = rule + "plantguidestep/";
            public const string GetAllPlantGuideSteps = Prefix + "getallplantguidesteps";
            public const string GetPlantGuideStepById = Prefix + "getplantguidestepbyid" + singelroute;
            public const string GetPlantGuideStepsByPlantId = Prefix + "getplantguidestepsbyplantid" + singelroute;
            public const string GetPlantGuideStepsByPlantName = Prefix + "getplantguidestepsbyplantname" + plantNameRoute;
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
            public const string Prefix = rule + "organizationrole/";
            public const string CreateOrganizationRole = Prefix + "createorganizationrole";
            public const string GetOrganizationRolesByOrganizationId = Prefix + "getorganizationrolesbyorganizationid" + singelroute;

        }
        public static class Organization
        {
            public const string Prefix = rule + "organizations";
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

        public static class AnalyzePlants
        {
            public const string Prefix = rule + "modelanalysis/";
            public const string AnalyzePlant = Prefix + "analyze";
        }
        public static class OrganizationInvitation
        {
            public const string Prefix = rule + "organization-invitations";
            public const string SendInvitation = Prefix;
            public const string GetAllInvitationsForUser = Prefix + "/my";
            public const string AcceptInvitation = Prefix + "/accept";
            public const string Revoken = Prefix + "/revoken";
            // public const string OrganizationInvitation = Prefix + "/organizationinvitation";
        }
    }
}