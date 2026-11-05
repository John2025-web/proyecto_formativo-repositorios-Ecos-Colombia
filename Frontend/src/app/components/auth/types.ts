import type { UserRole } from "@/services/api";

export type UserType = {
  name: string;
  email: string;
  token: string;
  role: UserRole;
  expiresAt: number;
};