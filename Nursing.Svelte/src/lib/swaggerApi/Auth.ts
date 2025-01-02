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

import type {
  AccessTokenResponse,
  ForgotPasswordRequest,
  HttpValidationProblemDetails,
  InfoRequest,
  InfoResponse,
  LoginRequest,
  RefreshRequest,
  RegisterRequest,
  ResendConfirmationEmailRequest,
  ResetPasswordRequest,
  TwoFactorRequest,
  TwoFactorResponse,
} from "./data-contracts";
import { ContentType, HttpClient, type RequestParams } from "./http-client";

export class Auth<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Nursing.API
   * @name RegisterCreate
   * @request POST:/auth/register
   * @response `200` `void` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `void` Unauthorized
   */
  registerCreate = (data: RegisterRequest, params: RequestParams = {}) =>
    this.request<void, HttpValidationProblemDetails | void>({
      path: `/auth/register`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      ...params,
    });
  /**
   * No description
   *
   * @tags Nursing.API
   * @name LoginCreate
   * @request POST:/auth/login
   * @response `200` `AccessTokenResponse` OK
   * @response `401` `void` Unauthorized
   */
  loginCreate = (
    data: LoginRequest,
    query?: {
      useCookies?: boolean;
      useSessionCookies?: boolean;
    },
    params: RequestParams = {},
  ) =>
    this.request<AccessTokenResponse, void>({
      path: `/auth/login`,
      method: "POST",
      query: query,
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Nursing.API
   * @name RefreshCreate
   * @request POST:/auth/refresh
   * @response `200` `AccessTokenResponse` OK
   * @response `401` `void` Unauthorized
   */
  refreshCreate = (data: RefreshRequest, params: RequestParams = {}) =>
    this.request<AccessTokenResponse, void>({
      path: `/auth/refresh`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Nursing.API
   * @name MapIdentityApiAuthConfirmEmail
   * @request GET:/auth/confirmEmail
   * @response `200` `void` OK
   * @response `401` `void` Unauthorized
   */
  mapIdentityApiAuthConfirmEmail = (
    query: {
      userId: string;
      code: string;
      changedEmail?: string;
    },
    params: RequestParams = {},
  ) =>
    this.request<void, void>({
      path: `/auth/confirmEmail`,
      method: "GET",
      query: query,
      ...params,
    });
  /**
   * No description
   *
   * @tags Nursing.API
   * @name ResendConfirmationEmailCreate
   * @request POST:/auth/resendConfirmationEmail
   * @response `200` `void` OK
   * @response `401` `void` Unauthorized
   */
  resendConfirmationEmailCreate = (data: ResendConfirmationEmailRequest, params: RequestParams = {}) =>
    this.request<void, void>({
      path: `/auth/resendConfirmationEmail`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      ...params,
    });
  /**
   * No description
   *
   * @tags Nursing.API
   * @name ForgotPasswordCreate
   * @request POST:/auth/forgotPassword
   * @response `200` `void` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `void` Unauthorized
   */
  forgotPasswordCreate = (data: ForgotPasswordRequest, params: RequestParams = {}) =>
    this.request<void, HttpValidationProblemDetails | void>({
      path: `/auth/forgotPassword`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      ...params,
    });
  /**
   * No description
   *
   * @tags Nursing.API
   * @name ResetPasswordCreate
   * @request POST:/auth/resetPassword
   * @response `200` `void` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `void` Unauthorized
   */
  resetPasswordCreate = (data: ResetPasswordRequest, params: RequestParams = {}) =>
    this.request<void, HttpValidationProblemDetails | void>({
      path: `/auth/resetPassword`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      ...params,
    });
  /**
   * No description
   *
   * @tags Nursing.API
   * @name Manage2FaCreate
   * @request POST:/auth/manage/2fa
   * @response `200` `TwoFactorResponse` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `void` Unauthorized
   * @response `404` `void` Not Found
   */
  manage2FaCreate = (data: TwoFactorRequest, params: RequestParams = {}) =>
    this.request<TwoFactorResponse, HttpValidationProblemDetails | void>({
      path: `/auth/manage/2fa`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Nursing.API
   * @name ManageInfoList
   * @request GET:/auth/manage/info
   * @response `200` `InfoResponse` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `void` Unauthorized
   * @response `404` `void` Not Found
   */
  manageInfoList = (params: RequestParams = {}) =>
    this.request<InfoResponse, HttpValidationProblemDetails | void>({
      path: `/auth/manage/info`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Nursing.API
   * @name ManageInfoCreate
   * @request POST:/auth/manage/info
   * @response `200` `InfoResponse` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `void` Unauthorized
   * @response `404` `void` Not Found
   */
  manageInfoCreate = (data: InfoRequest, params: RequestParams = {}) =>
    this.request<InfoResponse, HttpValidationProblemDetails | void>({
      path: `/auth/manage/info`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
}
