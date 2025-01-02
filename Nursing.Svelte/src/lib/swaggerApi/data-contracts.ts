/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface AccessTokenResponse {
  tokenType?: string | null;
  accessToken: string;
  /** @format int64 */
  expiresIn: number;
  refreshToken: string;
}

export interface FeedingDto {
  /** @format uuid */
  id?: string;
  /** @pattern ^-?(\d+\.)?\d{2}:\d{2}:\d{2}(\.\d{1,7})?$ */
  leftBreastTotal?: string;
  /** @pattern ^-?(\d+\.)?\d{2}:\d{2}:\d{2}(\.\d{1,7})?$ */
  rightBreastTotal?: string;
  /** @pattern ^-?(\d+\.)?\d{2}:\d{2}:\d{2}(\.\d{1,7})?$ */
  totalTime?: string;
  /** @format date-time */
  started?: Date;
  /** @format date-time */
  finished?: Date | null;
  lastIsLeft?: boolean;
  /** @format date-time */
  lastUpdated?: Date;
  /** @format date-time */
  deleted?: Date | null;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface HttpValidationProblemDetails {
  type?: string | null;
  title?: string | null;
  /** @format int32 */
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  errors?: Record<string, string[]>;
}

export interface InfoRequest {
  newEmail?: string | null;
  newPassword?: string | null;
  oldPassword?: string | null;
}

export interface InfoResponse {
  email: string;
  isEmailConfirmed: boolean;
}

export interface InviteViewModel {
  users: string[];
  /** @format uuid */
  id?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
  twoFactorCode?: string | null;
  twoFactorRecoveryCode?: string | null;
}

export interface RefreshRequest {
  refreshToken: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export interface ResendConfirmationEmailRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  resetCode: string;
  newPassword: string;
}

export interface SyncModel {
  /** @format date-time */
  lastSync?: Date;
  feedings?: FeedingDto[];
}

export interface SyncResult {
  success?: boolean;
  feedings: FeedingDto[];
  badIds: string[];
  /** @format int32 */
  updates?: number;
}

export interface TwoFactorRequest {
  enable?: boolean | null;
  twoFactorCode?: string | null;
  resetSharedKey?: boolean;
  resetRecoveryCodes?: boolean;
  forgetMachine?: boolean;
}

export interface TwoFactorResponse {
  sharedKey: string;
  /** @format int32 */
  recoveryCodesLeft: number;
  recoveryCodes?: string[] | null;
  isTwoFactorEnabled: boolean;
  isMachineRemembered: boolean;
}
