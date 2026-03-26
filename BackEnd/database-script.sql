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

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [Auctions] DROP CONSTRAINT [FK_Auctions_MarketItems_MarketItemId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [Bids] DROP CONSTRAINT [FK_Bids_Users_ApplicationUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [Conversations] DROP CONSTRAINT [FK_Conversations_MarketItems_ProductId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [MarketItems] DROP CONSTRAINT [FK_MarketItems_Organizations_OrganizationId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [MarketItems] DROP CONSTRAINT [FK_MarketItems_Organizations_OrganizationId1];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [OrderItems] DROP CONSTRAINT [FK_OrderItems_MarketItems_MarketItemId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [OrganizationMembers] DROP CONSTRAINT [FK_OrganizationMembers_Users_ApplicationUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    DROP INDEX [IX_OrganizationMembers_ApplicationUserId] ON [OrganizationMembers];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    DROP INDEX [IX_Bids_ApplicationUserId] ON [Bids];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [MarketItems] DROP CONSTRAINT [PK_MarketItems];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrganizationMembers]') AND [c].[name] = N'ApplicationUserId');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [OrganizationMembers] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [OrganizationMembers] DROP COLUMN [ApplicationUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Bids]') AND [c].[name] = N'ApplicationUserId');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Bids] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [Bids] DROP COLUMN [ApplicationUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketItems]') AND [c].[name] = N'ItemType');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [MarketItems] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [MarketItems] DROP COLUMN [ItemType];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarketItems]') AND [c].[name] = N'SourceCropId');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [MarketItems] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [MarketItems] DROP COLUMN [SourceCropId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    EXEC sp_rename N'[MarketItems]', N'Products', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    EXEC sp_rename N'[OrderItems].[MarketItemId]', N'productid', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    EXEC sp_rename N'[OrderItems].[IX_OrderItems_MarketItemId]', N'IX_OrderItems_productid', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    EXEC sp_rename N'[Auctions].[MarketItemId]', N'productid', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    EXEC sp_rename N'[Auctions].[IX_Auctions_MarketItemId]', N'IX_Auctions_productid', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    EXEC sp_rename N'[Products].[ImageUrl]', N'RejectionReason', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    EXEC sp_rename N'[Products].[IX_MarketItems_OrganizationId1]', N'IX_Products_OrganizationId1', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    EXEC sp_rename N'[Products].[IX_MarketItems_OrganizationId]', N'IX_Products_OrganizationId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Products]') AND [c].[name] = N'ProductType');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Products] DROP CONSTRAINT ' + @var8 + ';');
    EXEC(N'UPDATE [Products] SET [ProductType] = 0 WHERE [ProductType] IS NULL');
    ALTER TABLE [Products] ALTER COLUMN [ProductType] int NOT NULL;
    ALTER TABLE [Products] ADD DEFAULT 0 FOR [ProductType];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [Products] ADD [Status] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [Products] ADD CONSTRAINT [PK_Products] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    CREATE TABLE [ProductImages] (
        [Id] uniqueidentifier NOT NULL,
        [ImageUrl] nvarchar(max) NOT NULL,
        [IsMain] bit NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ProductImages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductImages_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    CREATE INDEX [IX_ProductImages_ProductId] ON [ProductImages] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [Auctions] ADD CONSTRAINT [FK_Auctions_Products_productid] FOREIGN KEY ([productid]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [Conversations] ADD CONSTRAINT [FK_Conversations_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [OrderItems] ADD CONSTRAINT [FK_OrderItems_Products_productid] FOREIGN KEY ([productid]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [Products] ADD CONSTRAINT [FK_Products_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    ALTER TABLE [Products] ADD CONSTRAINT [FK_Products_Organizations_OrganizationId1] FOREIGN KEY ([OrganizationId1]) REFERENCES [Organizations] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064210_Update'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260310064210_Update', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064751_UpdateAddDb'
)
BEGIN
    ALTER TABLE [Products] DROP CONSTRAINT [FK_Products_Organizations_OrganizationId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064751_UpdateAddDb'
)
BEGIN
    ALTER TABLE [Products] DROP CONSTRAINT [FK_Products_Organizations_OrganizationId1];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064751_UpdateAddDb'
)
BEGIN
    DROP INDEX [IX_Products_OrganizationId1] ON [Products];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064751_UpdateAddDb'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Products]') AND [c].[name] = N'OrganizationId1');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Products] DROP CONSTRAINT ' + @var9 + ';');
    ALTER TABLE [Products] DROP COLUMN [OrganizationId1];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064751_UpdateAddDb'
)
BEGIN
    ALTER TABLE [Products] ADD CONSTRAINT [FK_Products_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310064751_UpdateAddDb'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260310064751_UpdateAddDb', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310065322_UpdateAddDb2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260310065322_UpdateAddDb2', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319181830_UpdateOrganizationType'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260319181830_UpdateOrganizationType', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319183256_AddOrderShippingDetails'
)
BEGIN
    ALTER TABLE [Orders] ADD [BuildingNumber] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319183256_AddOrderShippingDetails'
)
BEGIN
    ALTER TABLE [Orders] ADD [ReceiverName] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319183256_AddOrderShippingDetails'
)
BEGIN
    ALTER TABLE [Orders] ADD [ReceiverPhone] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319183256_AddOrderShippingDetails'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingCity] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319183256_AddOrderShippingDetails'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingStreet] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319183256_AddOrderShippingDetails'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260319183256_AddOrderShippingDetails', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319194008_AddOrderPayment'
)
BEGIN
    ALTER TABLE [Orders] ADD [PaymentMethod] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319194008_AddOrderPayment'
)
BEGIN
    ALTER TABLE [Orders] ADD [PaymentStatus] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319194008_AddOrderPayment'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260319194008_AddOrderPayment', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319194528_AddOrderCancelled'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260319194528_AddOrderCancelled', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319200400_AddShippingFees'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingFees] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319200400_AddShippingFees'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260319200400_AddShippingFees', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260320143556_AddAuctionB2BFields'
)
BEGIN
    ALTER TABLE [Auctions] ADD [MinimumBidIncrement] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260320143556_AddAuctionB2BFields'
)
BEGIN
    ALTER TABLE [Auctions] ADD [ReservePrice] decimal(18,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260320143556_AddAuctionB2BFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260320143556_AddAuctionB2BFields', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260321221013_AddAuctionOrderId'
)
BEGIN
    ALTER TABLE [Auctions] ADD [OrderId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260321221013_AddAuctionOrderId'
)
BEGIN
    CREATE INDEX [IX_Auctions_OrderId] ON [Auctions] ([OrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260321221013_AddAuctionOrderId'
)
BEGIN
    ALTER TABLE [Auctions] ADD CONSTRAINT [FK_Auctions_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260321221013_AddAuctionOrderId'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260321221013_AddAuctionOrderId', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260321225424_FinalizeAuctionEngine'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260321225424_FinalizeAuctionEngine', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    ALTER TABLE [Chats] DROP CONSTRAINT [FK_Chats_Conversations_ConversationId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    DROP TABLE [Conversations];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    ALTER TABLE [Chats] DROP CONSTRAINT [PK_Chats];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    DECLARE @var10 nvarchar(max);
    SELECT @var10 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Chats]') AND [c].[name] = N'AttachmentUrl');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Chats] DROP CONSTRAINT ' + @var10 + ';');
    ALTER TABLE [Chats] DROP COLUMN [AttachmentUrl];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    DECLARE @var11 nvarchar(max);
    SELECT @var11 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Chats]') AND [c].[name] = N'IsSystemMessage');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Chats] DROP CONSTRAINT ' + @var11 + ';');
    ALTER TABLE [Chats] DROP COLUMN [IsSystemMessage];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    EXEC sp_rename N'[Chats]', N'ChatMessages', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    EXEC sp_rename N'[ChatMessages].[Timestamp]', N'SentAt', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    EXEC sp_rename N'[ChatMessages].[Message]', N'Content', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    EXEC sp_rename N'[ChatMessages].[ConversationId]', N'ChatRoomId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    EXEC sp_rename N'[ChatMessages].[IX_Chats_ConversationId]', N'IX_ChatMessages_ChatRoomId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    ALTER TABLE [ChatMessages] ADD [ProposedPrice] decimal(18,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    ALTER TABLE [ChatMessages] ADD [ProposedQuantity] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    ALTER TABLE [ChatMessages] ADD [RelatedOrderId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    ALTER TABLE [ChatMessages] ADD [Type] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    ALTER TABLE [ChatMessages] ADD CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    CREATE TABLE [ChatRooms] (
        [Id] uniqueidentifier NOT NULL,
        [BuyerId] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [LastMessageAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ChatRooms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChatRooms_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ChatRooms_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_ChatRooms_Users_BuyerId] FOREIGN KEY ([BuyerId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    CREATE INDEX [IX_ChatMessages_SenderId] ON [ChatMessages] ([SenderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    CREATE INDEX [IX_ChatRooms_BuyerId] ON [ChatRooms] ([BuyerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    CREATE INDEX [IX_ChatRooms_OrganizationId] ON [ChatRooms] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    CREATE INDEX [IX_ChatRooms_ProductId] ON [ChatRooms] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    ALTER TABLE [ChatMessages] ADD CONSTRAINT [FK_ChatMessages_ChatRooms_ChatRoomId] FOREIGN KEY ([ChatRoomId]) REFERENCES [ChatRooms] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    ALTER TABLE [ChatMessages] ADD CONSTRAINT [FK_ChatMessages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322122940_AddB2BChatAndNegotiations'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260322122940_AddB2BChatAndNegotiations', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322151218_FinalizeChatAndNegotiation'
)
BEGIN
    DECLARE @var12 nvarchar(max);
    SELECT @var12 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'ShippingStreet');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT ' + @var12 + ';');
    ALTER TABLE [Orders] ALTER COLUMN [ShippingStreet] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322151218_FinalizeChatAndNegotiation'
)
BEGIN
    DECLARE @var13 nvarchar(max);
    SELECT @var13 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'ShippingCity');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT ' + @var13 + ';');
    ALTER TABLE [Orders] ALTER COLUMN [ShippingCity] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322151218_FinalizeChatAndNegotiation'
)
BEGIN
    DECLARE @var14 nvarchar(max);
    SELECT @var14 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'ReceiverPhone');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT ' + @var14 + ';');
    ALTER TABLE [Orders] ALTER COLUMN [ReceiverPhone] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322151218_FinalizeChatAndNegotiation'
)
BEGIN
    DECLARE @var15 nvarchar(max);
    SELECT @var15 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'ReceiverName');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT ' + @var15 + ';');
    ALTER TABLE [Orders] ALTER COLUMN [ReceiverName] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322151218_FinalizeChatAndNegotiation'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [IsRead] bit NOT NULL,
        [RelatedEntityId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322151218_FinalizeChatAndNegotiation'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322151218_FinalizeChatAndNegotiation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260322151218_FinalizeChatAndNegotiation', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322173536_FinalizeChatEnterpriseFeatures'
)
BEGIN
    ALTER TABLE [ChatRooms] ADD [IsClosed] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322173536_FinalizeChatEnterpriseFeatures'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260322173536_FinalizeChatEnterpriseFeatures', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Users_BuyerId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    ALTER TABLE [Orders] ADD [OrganizationId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    CREATE TABLE [OrderStatusHistories] (
        [Id] uniqueidentifier NOT NULL,
        [OrderId] uniqueidentifier NOT NULL,
        [OldStatus] int NOT NULL,
        [NewStatus] int NOT NULL,
        [ChangedById] uniqueidentifier NOT NULL,
        [ChangedAt] datetime2 NOT NULL,
        [Note] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_OrderStatusHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderStatusHistories_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderStatusHistories_Users_ChangedById] FOREIGN KEY ([ChangedById]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] uniqueidentifier NOT NULL,
        [OrderId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PaymentMethod] int NOT NULL,
        [PaymentStatus] int NOT NULL,
        [PaidAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Payments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    CREATE INDEX [IX_Orders_OrganizationId] ON [Orders] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    CREATE INDEX [IX_OrderStatusHistories_ChangedById] ON [OrderStatusHistories] ([ChangedById]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    CREATE INDEX [IX_OrderStatusHistories_OrderId] ON [OrderStatusHistories] ([OrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    CREATE INDEX [IX_Payments_OrderId] ON [Payments] ([OrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    CREATE INDEX [IX_Payments_UserId] ON [Payments] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Users_BuyerId] FOREIGN KEY ([BuyerId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322190617_OrderLifecycle_Payment_StatusHistory'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260322190617_OrderLifecycle_Payment_StatusHistory', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Auctions] DROP CONSTRAINT [FK_Auctions_Products_productid];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Notifications] DROP CONSTRAINT [FK_Notifications_Users_UserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [OrderItems] DROP CONSTRAINT [FK_OrderItems_Products_productid];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Reviews] DROP CONSTRAINT [FK_Reviews_Users_TargetUserId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    EXEC sp_rename N'[Reviews].[TargetUserId]', N'TargetOrganizationId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    EXEC sp_rename N'[Reviews].[IX_Reviews_TargetUserId]', N'IX_Reviews_TargetOrganizationId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    EXEC sp_rename N'[OrderItems].[productid]', N'ProductId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    EXEC sp_rename N'[OrderItems].[IX_OrderItems_productid]', N'IX_OrderItems_ProductId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    EXEC sp_rename N'[Auctions].[productid]', N'ProductId', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    EXEC sp_rename N'[Auctions].[IX_Auctions_productid]', N'IX_Auctions_ProductId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Reviews] ADD [OrderId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Reviews] ADD [ProductId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Orders] ADD [ApplicationUserId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    CREATE TABLE [Shipments] (
        [Id] uniqueidentifier NOT NULL,
        [OrderId] uniqueidentifier NOT NULL,
        [TrackingNumber] nvarchar(max) NOT NULL,
        [CarrierName] nvarchar(max) NOT NULL,
        [DriverPhone] nvarchar(max) NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Shipments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Shipments_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    CREATE TABLE [ShippingAddresses] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Label] nvarchar(max) NOT NULL,
        [City] nvarchar(max) NOT NULL,
        [Street] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [IsDefault] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ShippingAddresses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ShippingAddresses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    CREATE INDEX [IX_Reviews_OrderId] ON [Reviews] ([OrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    CREATE INDEX [IX_Reviews_ProductId] ON [Reviews] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    CREATE INDEX [IX_Orders_ApplicationUserId] ON [Orders] ([ApplicationUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    CREATE INDEX [IX_Shipments_OrderId] ON [Shipments] ([OrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    CREATE INDEX [IX_ShippingAddresses_UserId] ON [ShippingAddresses] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Auctions] ADD CONSTRAINT [FK_Auctions_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [OrderItems] ADD CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Users_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [security].[Users] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Reviews] ADD CONSTRAINT [FK_Reviews_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Reviews] ADD CONSTRAINT [FK_Reviews_Organizations_TargetOrganizationId] FOREIGN KEY ([TargetOrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    ALTER TABLE [Reviews] ADD CONSTRAINT [FK_Reviews_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322210621_Logistics_Reviews_Notifications'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260322210621_Logistics_Reviews_Notifications', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322213232_Hotfix_Security_And_ReviewIndex'
)
BEGIN
    DROP INDEX [IX_Reviews_OrderId] ON [Reviews];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322213232_Hotfix_Security_And_ReviewIndex'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Reviews_OrderId_ReviewerId] ON [Reviews] ([OrderId], [ReviewerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322213232_Hotfix_Security_And_ReviewIndex'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260322213232_Hotfix_Security_And_ReviewIndex', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260322214640_Notifications_Integration'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260322214640_Notifications_Integration', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325040607_AddSuperAdminDashboardFields'
)
BEGIN
    ALTER TABLE [Orders] ADD [PlatformFee] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325040607_AddSuperAdminDashboardFields'
)
BEGIN
    CREATE TABLE [DiagnosisLogs] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Prediction] nvarchar(max) NOT NULL,
        [Confidence] float NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DiagnosisLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DiagnosisLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325040607_AddSuperAdminDashboardFields'
)
BEGIN
    CREATE INDEX [IX_DiagnosisLogs_UserId] ON [DiagnosisLogs] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325040607_AddSuperAdminDashboardFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260325040607_AddSuperAdminDashboardFields', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325232742_Update5'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260325232742_Update5', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325235704_EnhanceSuperAdminModels'
)
BEGIN
    ALTER TABLE [DiagnosisLogs] ADD [City] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325235704_EnhanceSuperAdminModels'
)
BEGIN
    ALTER TABLE [DiagnosisLogs] ADD [Latitude] float NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325235704_EnhanceSuperAdminModels'
)
BEGIN
    ALTER TABLE [DiagnosisLogs] ADD [Longitude] float NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325235704_EnhanceSuperAdminModels'
)
BEGIN
    ALTER TABLE [DiagnosisLogs] ADD [Region] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325235704_EnhanceSuperAdminModels'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Action] nvarchar(max) NOT NULL,
        [EntityName] nvarchar(max) NOT NULL,
        [OldValues] nvarchar(max) NOT NULL,
        [NewValues] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AuditLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325235704_EnhanceSuperAdminModels'
)
BEGIN
    CREATE TABLE [OrganizationDocuments] (
        [Id] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [Type] int NOT NULL,
        [FileUrl] nvarchar(max) NOT NULL,
        [UploadedAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_OrganizationDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrganizationDocuments_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325235704_EnhanceSuperAdminModels'
)
BEGIN
    CREATE TABLE [SystemSettings] (
        [Id] uniqueidentifier NOT NULL,
        [PlatformFeePercentage] decimal(18,2) NOT NULL,
        [StandardShippingFee] decimal(18,2) NOT NULL,
        [LastUpdated] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        [DeleteAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_SystemSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325235704_EnhanceSuperAdminModels'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325235704_EnhanceSuperAdminModels'
)
BEGIN
    CREATE INDEX [IX_OrganizationDocuments_OrganizationId] ON [OrganizationDocuments] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325235704_EnhanceSuperAdminModels'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260325235704_EnhanceSuperAdminModels', N'10.0.1');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326013923_EnhanceSuperAdmin'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Token] nvarchar(max) NOT NULL,
        [ExpiresOn] datetime2 NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [RevokedOn] datetime2 NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [security].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326013923_EnhanceSuperAdmin'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326013923_EnhanceSuperAdmin'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260326013923_EnhanceSuperAdmin', N'10.0.1');
END;

COMMIT;
GO

