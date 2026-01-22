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
        }
        public static class PlantGuideStep
        {
            public const string Prefix = rule + "plantguidestep/";
            public const string GetAllPlantGuideSteps = Prefix + "getallplantguidesteps";
            public const string GetPlantGuideStepById = Prefix + "getplantguidestepbyid" + singelroute;
            public const string GetPlantGuideStepsByPlantId = Prefix + "getplantguidestepsbyplantid" + singelroute;
            public const string GetPlantGuideStepsByPlantName = Prefix + "getplantguidestepsbyplantname" + plantNameRoute;
        }

    }
}