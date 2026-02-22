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
            public const string Prefix = rule + "proudct/";
            public const string cropInMarket = "list-crop-in-market";
        }
        public static class OrganizationRole
        {
            public const string Prefix = rule + "organizationrole/";
            public const string CreateOrganizationRole = Prefix + "createorganizationrole";
            public const string GetOrganizationRolesByOrganizationId = Prefix + "getorganizationrolesbyorganizationid" + singelroute;

        }
        public static class Organization
        {
            public const string Prefix = rule + "organizations/";
            public const string GetAllOrganizations = Prefix + "getallorganizations";
            public const string CreateOrganization = Prefix + "createorganization";
            public const string GetById = Prefix + "getorganizationbyid" + singelroute;
            public const string GetMyOrganizations = Prefix + "getmyorganizations";
            public const string UpdateById = Prefix + "upfatebyid" + singelroute;

        }

        public static class AnalyzePlants
        {
            public const string Prefix = rule + "modelanalysis/";
            public const string AnalyzePlant = Prefix + "analyze";
        }
    }
}