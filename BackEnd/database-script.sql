IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    IF SCHEMA_ID(N'security') IS NULL EXEC(N'CREATE SCHEMA [security];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [PlantInfos] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [ScientificName] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [IdealSoil] nvarchar(max) NOT NULL,
        [MedicalBenefits] nvarchar(max) NOT NULL,
        [PlantingSeason] nvarchar(max) NULL,
        [ImageUrl] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PlantInfos] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [security].[Roles] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [security].[Users] (
        [Id] uniqueidentifier NOT NULL,
        [FullName] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [UserType] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [PlantGuideSteps] (
        [Id] uniqueidentifier NOT NULL,
        [StepOrder] int NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Instruction] nvarchar(max) NOT NULL,
        [PlantInfoId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PlantGuideSteps] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PlantGuideSteps_PlantInfos_PlantInfoId] FOREIGN KEY ([PlantInfoId]) REFERENCES [PlantInfos] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [security].[RoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [security].[Roles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [Chats] (
        [Id] uniqueidentifier NOT NULL,
        [SenderId] uniqueidentifier NOT NULL,
        [ReceiverId] uniqueidentifier NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [Timestamp] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Chats] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Chats_Users_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Chats_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [Orders] (
        [Id] uniqueidentifier NOT NULL,
        [OrderDate] datetime2 NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [BuyerId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Orders_Users_BuyerId] FOREIGN KEY ([BuyerId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [Organizations] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [ContactEmail] nvarchar(max) NULL,
        [ContactPhone] nvarchar(max) NULL,
        [LogoUrl] nvarchar(max) NULL,
        [Type] int NOT NULL,
        [OwnerId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Organizations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Organizations_Users_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [Reviews] (
        [Id] uniqueidentifier NOT NULL,
        [Rating] int NOT NULL,
        [Comment] nvarchar(500) NULL,
        [ReviewerId] uniqueidentifier NOT NULL,
        [TargetUserId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reviews_Users_ReviewerId] FOREIGN KEY ([ReviewerId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reviews_Users_TargetUserId] FOREIGN KEY ([TargetUserId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [security].[UserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [security].[UserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [security].[UserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [security].[Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [security].[UserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [Farms] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Location] nvarchar(max) NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Farms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Farms_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [MarketItems] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(max) NULL,
        [ImageUrl] nvarchar(max) NULL,
        [StockQuantity] int NOT NULL,
        [IsAvailableForDirectSale] bit NOT NULL,
        [DirectSalePrice] decimal(18,2) NOT NULL,
        [IsAvailableForAuction] bit NOT NULL,
        [StartBiddingPrice] decimal(18,2) NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [ItemType] nvarchar(13) NOT NULL,
        [OrganizationId1] uniqueidentifier NULL,
        [Barcode] nvarchar(max) NULL,
        [ExpiryDate] datetime2 NULL,
        [WeightUnit] int NULL,
        [SourceCropId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_MarketItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MarketItems_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MarketItems_Organizations_OrganizationId1] FOREIGN KEY ([OrganizationId1]) REFERENCES [Organizations] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [OrganizationRoles] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [IsSystemDefault] bit NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_OrganizationRoles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrganizationRoles_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [Crops] (
        [Id] uniqueidentifier NOT NULL,
        [BatchNumber] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [PlantedArea] float NOT NULL,
        [PlantingDate] datetime2 NOT NULL,
        [ExpectedHarvestDate] datetime2 NULL,
        [PlantInfoId] uniqueidentifier NULL,
        [FarmId] uniqueidentifier NOT NULL,
        [ActualYieldQuantity] float NULL,
        [YieldUnit] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Crops] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Crops_Farms_FarmId] FOREIGN KEY ([FarmId]) REFERENCES [Farms] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Crops_PlantInfos_PlantInfoId] FOREIGN KEY ([PlantInfoId]) REFERENCES [PlantInfos] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [Auctions] (
        [Id] uniqueidentifier NOT NULL,
        [Title] nvarchar(150) NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [StartPrice] decimal(18,2) NOT NULL,
        [CurrentHighestBid] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [MarketItemId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Auctions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Auctions_MarketItems_MarketItemId] FOREIGN KEY ([MarketItemId]) REFERENCES [MarketItems] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [OrderItems] (
        [Id] uniqueidentifier NOT NULL,
        [OrderId] uniqueidentifier NOT NULL,
        [MarketItemId] uniqueidentifier NOT NULL,
        [Quantity] int NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderItems_MarketItems_MarketItemId] FOREIGN KEY ([MarketItemId]) REFERENCES [MarketItems] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [OrganizationMembers] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [OrganizationRoleId] uniqueidentifier NULL,
        [IsActive] bit NOT NULL,
        [JoinedAt] datetime2 NOT NULL,
        [ApplicationUserId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_OrganizationMembers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrganizationMembers_OrganizationRoles_OrganizationRoleId] FOREIGN KEY ([OrganizationRoleId]) REFERENCES [OrganizationRoles] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_OrganizationMembers_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrganizationMembers_Users_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [security].[Users] ([Id]),
        CONSTRAINT [FK_OrganizationMembers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [OrganizationRolePermissions] (
        [Id] uniqueidentifier NOT NULL,
        [PermissionsClaim] nvarchar(max) NOT NULL,
        [OrganizationRoleId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_OrganizationRolePermissions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrganizationRolePermissions_OrganizationRoles_OrganizationRoleId] FOREIGN KEY ([OrganizationRoleId]) REFERENCES [OrganizationRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [CropActivityLogs] (
        [Id] uniqueidentifier NOT NULL,
        [ActivityType] int NOT NULL,
        [Description] nvarchar(500) NULL,
        [ActivityDate] datetime2 NOT NULL,
        [CropId] uniqueidentifier NOT NULL,
        [PerformedById] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CropActivityLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CropActivityLogs_Crops_CropId] FOREIGN KEY ([CropId]) REFERENCES [Crops] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CropActivityLogs_Users_PerformedById] FOREIGN KEY ([PerformedById]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE TABLE [Bids] (
        [Id] uniqueidentifier NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [BidTime] datetime2 NOT NULL,
        [AuctionId] uniqueidentifier NOT NULL,
        [BidderId] uniqueidentifier NOT NULL,
        [ApplicationUserId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Bids] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Bids_Auctions_AuctionId] FOREIGN KEY ([AuctionId]) REFERENCES [Auctions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Bids_Users_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [security].[Users] ([Id]),
        CONSTRAINT [FK_Bids_Users_BidderId] FOREIGN KEY ([BidderId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Auctions_MarketItemId] ON [Auctions] ([MarketItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Bids_ApplicationUserId] ON [Bids] ([ApplicationUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Bids_AuctionId] ON [Bids] ([AuctionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Bids_BidderId] ON [Bids] ([BidderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Chats_ReceiverId] ON [Chats] ([ReceiverId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Chats_SenderId] ON [Chats] ([SenderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_CropActivityLogs_CropId] ON [CropActivityLogs] ([CropId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_CropActivityLogs_PerformedById] ON [CropActivityLogs] ([PerformedById]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Crops_FarmId] ON [Crops] ([FarmId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Crops_PlantInfoId] ON [Crops] ([PlantInfoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Farms_OrganizationId] ON [Farms] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_MarketItems_OrganizationId] ON [MarketItems] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_MarketItems_OrganizationId1] ON [MarketItems] ([OrganizationId1]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_OrderItems_MarketItemId] ON [OrderItems] ([MarketItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Orders_BuyerId] ON [Orders] ([BuyerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_OrganizationMembers_ApplicationUserId] ON [OrganizationMembers] ([ApplicationUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_OrganizationMembers_OrganizationId] ON [OrganizationMembers] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_OrganizationMembers_OrganizationRoleId] ON [OrganizationMembers] ([OrganizationRoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_OrganizationMembers_UserId] ON [OrganizationMembers] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_OrganizationRolePermissions_OrganizationRoleId] ON [OrganizationRolePermissions] ([OrganizationRoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_OrganizationRoles_OrganizationId] ON [OrganizationRoles] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Organizations_OwnerId] ON [Organizations] ([OwnerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_PlantGuideSteps_PlantInfoId] ON [PlantGuideSteps] ([PlantInfoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Reviews_ReviewerId] ON [Reviews] ([ReviewerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_Reviews_TargetUserId] ON [Reviews] ([TargetUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_RoleClaims_RoleId] ON [security].[RoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [security].[Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_UserClaims_UserId] ON [security].[UserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_UserLogins_UserId] ON [security].[UserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [security].[UserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [security].[Users] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [security].[Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201015932_AddCropActivityLog_FixCycle'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260201015932_AddCropActivityLog_FixCycle', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260201035314_Addsoftdelete'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260201035314_Addsoftdelete', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    ALTER TABLE [Chats] DROP CONSTRAINT [FK_Chats_Users_ReceiverId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    ALTER TABLE [Chats] DROP CONSTRAINT [FK_Chats_Users_SenderId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    DROP INDEX [IX_Chats_SenderId] ON [Chats];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    EXEC sp_rename N'[Chats].[ReceiverId]', N'ConversationId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    EXEC sp_rename N'[Chats].[IX_Chats_ReceiverId]', N'IX_Chats_ConversationId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketItems]') AND [c].[name] = N'ItemType');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [MarketItems] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [MarketItems] ALTER COLUMN [ItemType] nvarchar(8) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    ALTER TABLE [MarketItems] ADD [ProductType] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Crops]') AND [c].[name] = N'ExpectedHarvestDate');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Crops] DROP CONSTRAINT ' + @var1 + ';');
    EXEC(N'UPDATE [Crops] SET [ExpectedHarvestDate] = ''0001-01-01T00:00:00.0000000'' WHERE [ExpectedHarvestDate] IS NULL');
    ALTER TABLE [Crops] ALTER COLUMN [ExpectedHarvestDate] datetime2 NOT NULL;
    ALTER TABLE [Crops] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [ExpectedHarvestDate];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    ALTER TABLE [Crops] ADD [IsConvertedToProduct] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    ALTER TABLE [Chats] ADD [AttachmentUrl] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    ALTER TABLE [Chats] ADD [IsRead] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    ALTER TABLE [Chats] ADD [IsSystemMessage] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    CREATE TABLE [Conversations] (
        [Id] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NULL,
        [BuyerId] uniqueidentifier NOT NULL,
        [SellerId] uniqueidentifier NOT NULL,
        [LastMessageDate] datetime2 NOT NULL,
        [LastMessageContent] nvarchar(max) NOT NULL,
        [IsClosed] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Conversations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Conversations_MarketItems_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [MarketItems] ([Id]),
        CONSTRAINT [FK_Conversations_Users_BuyerId] FOREIGN KEY ([BuyerId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Conversations_Users_SellerId] FOREIGN KEY ([SellerId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    CREATE INDEX [IX_MarketItems_SourceCropId] ON [MarketItems] ([SourceCropId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    CREATE INDEX [IX_Conversations_BuyerId] ON [Conversations] ([BuyerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    CREATE INDEX [IX_Conversations_ProductId] ON [Conversations] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    CREATE INDEX [IX_Conversations_SellerId] ON [Conversations] ([SellerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    ALTER TABLE [Chats] ADD CONSTRAINT [FK_Chats_Conversations_ConversationId] FOREIGN KEY ([ConversationId]) REFERENCES [Conversations] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    ALTER TABLE [MarketItems] ADD CONSTRAINT [FK_MarketItems_Crops_SourceCropId] FOREIGN KEY ([SourceCropId]) REFERENCES [Crops] ([Id]) ON DELETE SET NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260207223522_InitialFullSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260207223522_InitialFullSchema', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260211023955_RemovUserType'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[security].[Users]') AND [c].[name] = N'UserType');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [security].[Users] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [security].[Users] DROP COLUMN [UserType];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260211023955_RemovUserType'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260211023955_RemovUserType', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260211061419_OrganizationInvitation'
)
BEGIN
    CREATE TABLE [OrganizationInvitations] (
        [Id] uniqueidentifier NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [SenderId] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [Token] nvarchar(max) NOT NULL,
        [ExpiryDate] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_OrganizationInvitations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrganizationInvitations_OrganizationRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [OrganizationRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrganizationInvitations_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260211061419_OrganizationInvitation'
)
BEGIN
    CREATE INDEX [IX_OrganizationInvitations_OrganizationId] ON [OrganizationInvitations] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260211061419_OrganizationInvitation'
)
BEGIN
    CREATE INDEX [IX_OrganizationInvitations_RoleId] ON [OrganizationInvitations] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260211061419_OrganizationInvitation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260211061419_OrganizationInvitation', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221130835_AddRowVersionToAuction'
)
BEGIN
    ALTER TABLE [MarketItems] ADD [RowVersion] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221130835_AddRowVersionToAuction'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260221130835_AddRowVersionToAuction', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221134743_HighestBidderId'
)
BEGIN
    ALTER TABLE [Auctions] ADD [HighestBidderId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221134743_HighestBidderId'
)
BEGIN
    CREATE INDEX [IX_Auctions_HighestBidderId] ON [Auctions] ([HighestBidderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221134743_HighestBidderId'
)
BEGIN
    ALTER TABLE [Auctions] ADD CONSTRAINT [FK_Auctions_Users_HighestBidderId] FOREIGN KEY ([HighestBidderId]) REFERENCES [security].[Users] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221134743_HighestBidderId'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260221134743_HighestBidderId', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [Reviews] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [PlantInfos] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [PlantGuideSteps] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [Organizations] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [Organizations] ADD [OrganizationStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [OrganizationRoles] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [OrganizationRolePermissions] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [OrganizationMembers] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [OrganizationInvitations] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [Orders] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [OrderItems] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [MarketItems] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [Farms] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [Crops] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [CropActivityLogs] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [Conversations] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [Chats] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [Bids] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    ALTER TABLE [Auctions] ADD [DeleteAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260221171957_DeleteAt'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260221171957_DeleteAt', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260222225149_updateDatabase'
)
BEGIN
    ALTER TABLE [MarketItems] DROP CONSTRAINT [FK_MarketItems_Crops_SourceCropId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260222225149_updateDatabase'
)
BEGIN
    DROP TABLE [CropActivityLogs];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260222225149_updateDatabase'
)
BEGIN
    DROP TABLE [Crops];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260222225149_updateDatabase'
)
BEGIN
    DROP TABLE [Farms];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260222225149_updateDatabase'
)
BEGIN
    DROP INDEX [IX_MarketItems_SourceCropId] ON [MarketItems];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260222225149_updateDatabase'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260222225149_updateDatabase', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309012550_UpdateRolesAndRelations'
)
BEGIN
    ALTER TABLE [OrganizationMembers] DROP CONSTRAINT [FK_OrganizationMembers_OrganizationRoles_OrganizationRoleId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309012550_UpdateRolesAndRelations'
)
BEGIN
    DROP INDEX [IX_OrganizationMembers_OrganizationRoleId] ON [OrganizationMembers];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309012550_UpdateRolesAndRelations'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrganizationMembers]') AND [c].[name] = N'OrganizationRoleId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [OrganizationMembers] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [OrganizationMembers] DROP COLUMN [OrganizationRoleId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309012550_UpdateRolesAndRelations'
)
BEGIN
    CREATE TABLE [OrganizationMemberOrganizationRole] (
        [OrganizationMemberId] uniqueidentifier NOT NULL,
        [OrganizationRolesId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_OrganizationMemberOrganizationRole] PRIMARY KEY ([OrganizationMemberId], [OrganizationRolesId]),
        CONSTRAINT [FK_OrganizationMemberOrganizationRole_OrganizationMembers_OrganizationMemberId] FOREIGN KEY ([OrganizationMemberId]) REFERENCES [OrganizationMembers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrganizationMemberOrganizationRole_OrganizationRoles_OrganizationRolesId] FOREIGN KEY ([OrganizationRolesId]) REFERENCES [OrganizationRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309012550_UpdateRolesAndRelations'
)
BEGIN
    CREATE INDEX [IX_OrganizationMemberOrganizationRole_OrganizationRolesId] ON [OrganizationMemberOrganizationRole] ([OrganizationRolesId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309012550_UpdateRolesAndRelations'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260309012550_UpdateRolesAndRelations', N'10.0.1');
END;

COMMIT;
GO

