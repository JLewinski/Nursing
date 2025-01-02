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

import type { InviteViewModel, SyncModel, SyncResult } from "./data-contracts";
import { ContentType, HttpClient, type RequestParams } from "./http-client";

export class Api<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Sync
   * @name Sync
   * @request POST:/api/Sync/sync
   * @secure
   * @response `200` `SyncResult` OK
   * @response `401` `string` Unauthorized
   */
  sync = (data: SyncModel, params: RequestParams = {}) =>
    this.request<SyncResult, string>({
      path: `/api/Sync/sync`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Sync
   * @name GetInvites
   * @request GET:/api/Sync/invites
   * @secure
   * @response `200` `(InviteViewModel)[]` OK
   * @response `401` `string` Unauthorized
   */
  getInvites = (params: RequestParams = {}) =>
    this.request<InviteViewModel[], string>({
      path: `/api/Sync/invites`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Sync
   * @name SendInvite
   * @request POST:/api/Sync/sendInvite
   * @secure
   * @response `200` `string` OK
   * @response `401` `string` Unauthorized
   */
  sendInvite = (data: string, params: RequestParams = {}) =>
    this.request<string, string>({
      path: `/api/Sync/sendInvite`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      ...params,
    });
  /**
   * No description
   *
   * @tags Sync
   * @name AcceptInvite
   * @request POST:/api/Sync/acceptInvite
   * @secure
   * @response `200` `string` OK
   * @response `401` `string` Unauthorized
   */
  acceptInvite = (data: string, params: RequestParams = {}) =>
    this.request<string, string>({
      path: `/api/Sync/acceptInvite`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      ...params,
    });
  /**
   * No description
   *
   * @tags Sync
   * @name DeclineInvite
   * @request POST:/api/Sync/declineInvite
   * @secure
   * @response `200` `string` OK
   * @response `401` `string` Unauthorized
   */
  declineInvite = (data: string, params: RequestParams = {}) =>
    this.request<string, string>({
      path: `/api/Sync/declineInvite`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      ...params,
    });
  /**
   * No description
   *
   * @tags Sync
   * @name Delete
   * @request POST:/api/Sync/delete
   * @secure
   * @response `200` `string` OK
   * @response `401` `string` Unauthorized
   */
  delete = (data: string[], params: RequestParams = {}) =>
    this.request<string, string>({
      path: `/api/Sync/delete`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      ...params,
    });
}
