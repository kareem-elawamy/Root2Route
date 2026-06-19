export interface MyOrganization {
  id: string;
  name: string;
  ownerId?: string;
  description?: string;
  address?: string;
  contactEmail?: string;
  contactPhone?: string;
  logoPath?: string;
  type?: number;
  organizationStatus?: number;
  rejectionReason?: string;
}
