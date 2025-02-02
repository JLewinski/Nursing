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

import type { NursingAPIEndpointsUserInformationResponse } from "./data-contracts";

export namespace Userinfo {
  /**
   * No description
   * @tags Userinfo
   * @name UserInformation
   * @request GET:/userinfo
   * @secure
   * @response `200` `NursingAPIEndpointsUserInformationResponse` Success
   * @response `401` `string` Unauthorized
   */
  export namespace UserInformation {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = NursingAPIEndpointsUserInformationResponse;
  }
}
