export interface MatchResult {
  id: number;
  orderId: string;
  currency: string;
  systemAmount: number | null;
  providerAmount: number | null;
  status: string;
  isResolved: boolean;
  resolutionSide: string | null;
  createdDate: string;
  updatedDate: string | null;
}
