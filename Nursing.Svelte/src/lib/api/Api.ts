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
import { ContentType, HttpClient, type RequestParams } from "./http-client";

export class Api<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Sync
   * @name Sync
   * @request POST:/api/sync
   * @secure
   * @response `200` `NursingAPIEndpointsSyncResponse` Success
   * @response `400` `string` Bad Request
   * @response `401` `string` Unauthorized
   */
  sync = (data: NursingAPIEndpointsSyncRequest, params: RequestParams = {}) =>
    this.request<NursingAPIEndpointsSyncResponse, string>({
      path: `/api/sync`,
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
   * @tags Userinfo
   * @name UserInformation
   * @request GET:/api/userinfo
   * @secure
   * @response `200` `NursingAPIEndpointsUserInformationResponse` Success
   * @response `401` `string` Unauthorized
   */
  userInformation = (params: RequestParams = {}) =>
    this.request<NursingAPIEndpointsUserInformationResponse, string>({
      path: `/api/userinfo`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
}
