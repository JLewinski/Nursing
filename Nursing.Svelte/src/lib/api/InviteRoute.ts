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

import type { NursingAPIEndpointsInviteRequest } from "./data-contracts";

export namespace Invite {
  /**
   * No description
   * @tags Invite
   * @name Invite
   * @request POST:/invite
   * @secure
   * @response `204` `string` No Content
   * @response `401` `string` Unauthorized
   */
  export namespace Invite {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = NursingAPIEndpointsInviteRequest;
    export type RequestHeaders = {};
    export type ResponseBody = string;
  }
}
