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

import { InviteViewModel, SyncModel, SyncResult } from "./data-contracts";

export namespace Api {
  /**
   * No description
   * @tags Sync
   * @name Sync
   * @request POST:/api/Sync/sync
   * @secure
   * @response `200` `SyncResult` OK
   * @response `401` `string` Unauthorized
   */
  export namespace Sync {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = SyncModel;
    export type RequestHeaders = {};
    export type ResponseBody = SyncResult;
  }

  /**
   * No description
   * @tags Sync
   * @name GetInvites
   * @request GET:/api/Sync/invites
   * @secure
   * @response `200` `(InviteViewModel)[]` OK
   * @response `401` `string` Unauthorized
   */
  export namespace GetInvites {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = InviteViewModel[];
  }

  /**
   * No description
   * @tags Sync
   * @name SendInvite
   * @request POST:/api/Sync/sendInvite
   * @secure
   * @response `200` `string` OK
   * @response `401` `string` Unauthorized
   */
  export namespace SendInvite {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = string;
    export type RequestHeaders = {};
    export type ResponseBody = string;
  }

  /**
   * No description
   * @tags Sync
   * @name AcceptInvite
   * @request POST:/api/Sync/acceptInvite
   * @secure
   * @response `200` `string` OK
   * @response `401` `string` Unauthorized
   */
  export namespace AcceptInvite {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = string;
    export type RequestHeaders = {};
    export type ResponseBody = string;
  }

  /**
   * No description
   * @tags Sync
   * @name DeclineInvite
   * @request POST:/api/Sync/declineInvite
   * @secure
   * @response `200` `string` OK
   * @response `401` `string` Unauthorized
   */
  export namespace DeclineInvite {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = string;
    export type RequestHeaders = {};
    export type ResponseBody = string;
  }

  /**
   * No description
   * @tags Sync
   * @name Delete
   * @request POST:/api/Sync/delete
   * @secure
   * @response `200` `string` OK
   * @response `401` `string` Unauthorized
   */
  export namespace Delete {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = string[];
    export type RequestHeaders = {};
    export type ResponseBody = string;
  }
}
