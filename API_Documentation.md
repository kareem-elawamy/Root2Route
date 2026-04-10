# Root2Route API Documentation

This document provides a comprehensive overview of all the API endpoints defined in the Root2Route application.
The base URL for the API is `api/v1`.

> [!WARNING]
> **Important Bug Found:** In `Domain/MetaData/Router.cs`, the routing prefix for **Product** and **Order** is missing a forward slash (`/`).
> For example: `public const string Prefix = rule + "product";` results in `api/v1product` instead of `api/v1/product`. You should fix this in `Router.cs` by adding `"/product"` and `"/order"`.

## 1. Authentication (`api/v1/auth`)
- `POST /register` - Register a new user
- `POST /login` - User login
- `POST /verify-otp` - Verify OTP email/phone
- `POST /resend-otp` - Resend OTP
- `POST /forget-password` - Request password reset
- `POST /reset-password` - Execute password reset

## 2. Plant Information (`api/v1/plant-info`)
- `GET /all` - Get all plant information
- `GET /paginated` - Get paginated plant info
- `POST /create` - Create new plant info
- `PUT /edit` - Edit existing plant info
- `DELETE /delete` - Delete a plant info

## 3. Plant Guide Steps (`api/v1/plant-guide-steps`)
- `GET /all` - Get all guide steps
- `GET /{id}` - Get a specific step by ID
- `GET /by-plant/{id}` - Get steps by Plant ID
- `GET /by-name/{plantName}` - Get steps by Plant Name

## 4. Product (`api/v1product` - *See Bug Warning*)
- `POST /Add` - Add a new product
- `GET /GetAll` - Get all products
- `GET /{id}` - Get product by ID
- `GET /Organization/{organizationId}` - Get products by Org ID
- `PUT /Update` - Update a product
- `PUT /ChangeStatus` - Change product status
- `DELETE /Delete/{id}` - Delete product

## 5. Order (`api/v1order` - *See Bug Warning*)
- `POST /Create` - Create a new order
- `GET /MyOrders` - Get current user's orders
- `GET /{id}` - Get order by ID
- `PUT /ChangeStatus` - Change order status
- `DELETE /Cancel/{id}` - Cancel an order

## 6. Organization (`api/v1/organizations`)
- `GET /` - Get all organizations
- `POST /` - Create organization
- `PUT /` - Update organization
- `GET /{id}` - Get organization details
- `DELETE /{id}` - Soft delete organization
- `PUT /{id}/restore` - Restore organization
- `PUT /{id}/change-owner` - Change organization's owner
- `PUT /{id}/status` - Change organization status
- `POST /{id}/logo` - Upload org logo
- `GET /{id}/statistics` - View org statistics
- `GET /my` - Get user's own organizations
- `GET /status/{status}` - Get organizations by status

## 7. Organization Roles & Members
**Roles** (`api/v1/organization-roles`):
- `POST /create` - Create role
- `GET /by-organization/{organizationId}` - Get roles for org
- `GET /system-permissions` - Get all system permissions

**Members** (`api/v1/organization-members`):
- `POST /add` - Add member to org
- `GET /by-organization/{organizationId}` - Get org members
- `DELETE /{organizationMemberId}/remove` - Remove member

**Invitations** (`api/v1/organization-invitations`):
- `POST /send` - Send invitation
- `GET /my` - Get current user invitations
- `POST /accept` - Accept invitation
- `POST /revoke` - Revoke invitation

## 8. Machine Learning Model Analysis (`api/v1/model-analysis`)
- `POST /analyze` - Analyze a plant image through the model

## 9. Auctions (`api/v1/auctions`)
- `POST /create` - Create new auction
- `PUT /{auctionId}/update` - Update auction details
- `PUT /{auctionId}/cancel` - Cancel active auction
- `GET /{id}` - Get auction details
- `GET /GetActive` - Query active auctions
- `GET /GetCompleted` - Query completed auctions
- `POST /{id}/checkout` - Checkout won auction
- `GET /my-organization/{organizationId}` - View org's auctions
- `GET /my-won` - View won auctions
- `GET /my-participated` - View participated auctions
- `POST /{auctionId}/bid` - Place a bid

## 10. Real-time Chat (`api/v1/chat`)
- `POST /start` - Start new chat room
- `POST /send` - Send message
- `POST /accept-offer` / `/reject-offer` - Respond to chat offers
- `GET /my-rooms` - List my chat rooms
- `GET /{chatRoomId}/history` - Load chat history
- `GET /{roomId}/details` - Room details
- `PUT /{roomId}/close` - Close chat
- `DELETE /messages/{messageId}` - Delete message

## 11. Shipments (`api/v1/shipments`)
- `POST /dispatch` - Dispatch shipment
- `GET /addresses` - Get my saved addresses
- `POST /addresses` - Add new address
- `PUT /{id}/status` - Update shipment status

## 12. Reviews (`api/v1/reviews`)
- `POST /` - Add a review
- `GET /organization/{orgId}` - Get org reviews

## 13. Notifications (`api/v1/notifications`)
- `GET /` - Get my notifications
- `GET /unread-count` - Unread items count
- `PUT /{id}/read` - Mark one as read
- `PUT /read-all` - Mark all as read
