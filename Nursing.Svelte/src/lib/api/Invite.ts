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
import { ContentType, HttpClient, type RequestParams } from "./http-client";

export class Invite<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Invite
   * @name Invite
   * @request POST:/invite
   * @secure
   * @response `204` `string` No Content
   * @response `401` `string` Unauthorized
   */
  invite = (data: NursingAPIEndpointsInviteRequest, params: RequestParams = {}) =>
    this.request<string, string>({
      path: `/invite`,
      method: "POST",
      body: data,
      secure: true,
      type: ContentType.Json,
      ...params,
    });
}
