export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expireAt: string;
  role: string[] | null;      // null when user has no Identity role (org user only)
  fullName: string;
  message: string | null;
  firstOrganizationId: string | null;
}
