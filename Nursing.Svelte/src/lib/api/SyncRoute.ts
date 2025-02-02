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

import type { NursingAPIEndpointsSyncRequest, NursingAPIEndpointsSyncResponse } from "./data-contracts";

export namespace Sync {
  /**
   * No description
   * @tags Sync
   * @name Sync
   * @request POST:/sync
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
}
