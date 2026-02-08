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

        }
        public static class Organization
        {
            public const string Prefix = rule + "organizations/";
            public const string GetAllOwnerOrganizations = Prefix + "getallownerorganizations";
        }
        public static class Farm
        {
            public const string Prefix = rule + "farm/";
            public const string CreateFarm = Prefix + "CreateFarm";

        }
        public static class Crop
        {
            public const string Prefix = rule + "crop/";

            // عرض البيانات
            public const string List = Prefix + "list"; // api/v1/crop/list
            public const string GetById = Prefix + "getbyid" + singelroute; // api/v1/crop/getbyid/{id}
            public const string GetByFarmId = Prefix + "getbyfarmid" + singelroute; // api/v1/crop/getbyfarmid/{id}

            // العمليات
            public const string Create = Prefix + "create";
            public const string Edit = Prefix + "edit";
            public const string Delete = Prefix + "delete" + singelroute;

            // عملية خاصة
            public const string RegisterHarvest = Prefix + "registerharvest";
        }

    }
}