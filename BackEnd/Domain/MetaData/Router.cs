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
            public const string ForgetPassword = Prefix + "/forget-password";
            public const string ResetPassword = Prefix + "/reset-password";
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
            public const string CancelOrder = Prefix + "/Cancel/{id}";
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
            public const string GetByStatus = Prefix + "/status/{status}";
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

        public static class Auction
        {
            public const string Prefix = rule + "/auctions";
            public const string CreateAuction = Prefix + "/create";
            public const string UpdateAuction = Prefix + "/{auctionId}/update";
            public const string CancelAuction = Prefix + "/{auctionId}/cancel";
            public const string GetById = Prefix + "/{id}";
            public const string GetActive = Prefix + "/GetActive";
            public const string GetCompleted = Prefix + "/GetCompleted";
            public const string Checkout = Prefix + "/{id}/checkout";
            public const string GetMyOrgAuctions = Prefix + "/my-organization/{organizationId}";
            public const string GetMyWonAuctions = Prefix + "/my-won";
            public const string GetMyParticipated = Prefix + "/my-participated";
            public const string PlaceBid = Prefix + "/{auctionId}/bid";
        }

        public static class OrganizationInvitation
        {
            public const string Prefix = rule + "/organization-invitations";
            public const string SendInvitation = Prefix + "/send";
            public const string GetAllInvitationsForUser = Prefix + "/my";
            public const string AcceptInvitation = Prefix + "/accept";
            public const string Revoken = Prefix + "/revoke"; 
        }

        public static class Chat
        {
            public const string Prefix = rule + "/chat";
            public const string StartChat = Prefix + "/start";
            public const string SendMessage = Prefix + "/send";
            public const string AcceptOffer = Prefix + "/accept-offer";
            public const string GetMyRooms = Prefix + "/my-rooms";
            public const string GetHistory = Prefix + "/{chatRoomId}/history";
            public const string GetRoomDetails = Prefix + "/{roomId}/details";
            public const string CloseChat = Prefix + "/{roomId}/close";
            public const string RejectOffer = Prefix + "/reject-offer";
            public const string DeleteMessage = Prefix + "/messages/{messageId}";
        }

        public static class Shipment
        {
            public const string Prefix = rule + "/shipments";
            public const string Dispatch = Prefix + "/dispatch";
            public const string GetMyAddresses = Prefix + "/addresses";
            public const string AddAddress = Prefix + "/addresses";
            public const string UpdateStatus = Prefix + "/{id}/status";
        }

        public static class Review
        {
            public const string Prefix = rule + "/reviews";
            public const string Add = Prefix;
            public const string GetByOrganization = Prefix + "/organization/{orgId}";
        }

        public static class Notification
        {
            public const string Prefix = rule + "/notifications";
            public const string GetMy = Prefix;
            public const string UnreadCount = Prefix + "/unread-count";
            public const string MarkRead = Prefix + "/{id}/read";
            public const string MarkAllRead = Prefix + "/read-all";
        }
    }
}