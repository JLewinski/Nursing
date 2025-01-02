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

import {
  AccessTokenResponse,
  ForgotPasswordRequest,
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

export namespace Auth {
  /**
   * No description
   * @tags Nursing.API
   * @name RegisterCreate
   * @request POST:/auth/register
   * @response `200` `string` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `string` Unauthorized
   */
  export namespace RegisterCreate {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = RegisterRequest;
    export type RequestHeaders = {};
    export type ResponseBody = string;
  }

  /**
   * No description
   * @tags Nursing.API
   * @name LoginCreate
   * @request POST:/auth/login
   * @response `200` `AccessTokenResponse` OK
   * @response `401` `string` Unauthorized
   */
  export namespace LoginCreate {
    export type RequestParams = {};
    export type RequestQuery = {
      useCookies?: boolean;
      useSessionCookies?: boolean;
    };
    export type RequestBody = LoginRequest;
    export type RequestHeaders = {};
    export type ResponseBody = AccessTokenResponse;
  }

  /**
   * No description
   * @tags Nursing.API
   * @name RefreshCreate
   * @request POST:/auth/refresh
   * @response `200` `AccessTokenResponse` OK
   * @response `401` `string` Unauthorized
   */
  export namespace RefreshCreate {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = RefreshRequest;
    export type RequestHeaders = {};
    export type ResponseBody = AccessTokenResponse;
  }

  /**
   * No description
   * @tags Nursing.API
   * @name MapIdentityApiAuthConfirmEmail
   * @request GET:/auth/confirmEmail
   * @response `200` `string` OK
   * @response `401` `string` Unauthorized
   */
  export namespace MapIdentityApiAuthConfirmEmail {
    export type RequestParams = {};
    export type RequestQuery = {
      userId: string;
      code: string;
      changedEmail?: string;
    };
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = string;
  }

  /**
   * No description
   * @tags Nursing.API
   * @name ResendConfirmationEmailCreate
   * @request POST:/auth/resendConfirmationEmail
   * @response `200` `string` OK
   * @response `401` `string` Unauthorized
   */
  export namespace ResendConfirmationEmailCreate {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = ResendConfirmationEmailRequest;
    export type RequestHeaders = {};
    export type ResponseBody = string;
  }

  /**
   * No description
   * @tags Nursing.API
   * @name ForgotPasswordCreate
   * @request POST:/auth/forgotPassword
   * @response `200` `string` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `string` Unauthorized
   */
  export namespace ForgotPasswordCreate {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = ForgotPasswordRequest;
    export type RequestHeaders = {};
    export type ResponseBody = string;
  }

  /**
   * No description
   * @tags Nursing.API
   * @name ResetPasswordCreate
   * @request POST:/auth/resetPassword
   * @response `200` `string` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `string` Unauthorized
   */
  export namespace ResetPasswordCreate {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = ResetPasswordRequest;
    export type RequestHeaders = {};
    export type ResponseBody = string;
  }

  /**
   * No description
   * @tags Nursing.API
   * @name Manage2FaCreate
   * @request POST:/auth/manage/2fa
   * @response `200` `TwoFactorResponse` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `string` Unauthorized
   * @response `404` `string` Not Found
   */
  export namespace Manage2FaCreate {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = TwoFactorRequest;
    export type RequestHeaders = {};
    export type ResponseBody = TwoFactorResponse;
  }

  /**
   * No description
   * @tags Nursing.API
   * @name ManageInfoList
   * @request GET:/auth/manage/info
   * @response `200` `InfoResponse` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `string` Unauthorized
   * @response `404` `string` Not Found
   */
  export namespace ManageInfoList {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = InfoResponse;
  }

  /**
   * No description
   * @tags Nursing.API
   * @name ManageInfoCreate
   * @request POST:/auth/manage/info
   * @response `200` `InfoResponse` OK
   * @response `400` `HttpValidationProblemDetails` Bad Request
   * @response `401` `string` Unauthorized
   * @response `404` `string` Not Found
   */
  export namespace ManageInfoCreate {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = InfoRequest;
    export type RequestHeaders = {};
    export type ResponseBody = InfoResponse;
  }
}
