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
  NursingAPIEndpointsSyncRequest,
  NursingAPIEndpointsSyncResponse,
  NursingAPIEndpointsUserInformationResponse,
} from "./data-contracts";

export namespace Api {
  /**
   * No description
   * @tags Sync
   * @name Sync
   * @request POST:/api/sync
   * @secure
   * @response `200` `NursingAPIEndpointsSyncResponse` Success
   * @response `400` `string` Bad Request
   * @response `401` `string` Unauthorized
   */
  export namespace Sync {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = NursingAPIEndpointsSyncRequest;
    export type RequestHeaders = {};
    export type ResponseBody = NursingAPIEndpointsSyncResponse;
  }

  /**
   * No description
   * @tags Userinfo
   * @name UserInformation
   * @request GET:/api/userinfo
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
