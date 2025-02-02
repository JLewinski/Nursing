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
import { HttpClient, type RequestParams } from "./http-client";

export class Userinfo<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Userinfo
   * @name UserInformation
   * @request GET:/userinfo
   * @secure
   * @response `200` `NursingAPIEndpointsUserInformationResponse` Success
   * @response `401` `string` Unauthorized
   */
  userInformation = (params: RequestParams = {}) =>
    this.request<NursingAPIEndpointsUserInformationResponse, string>({
      path: `/userinfo`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });
}
