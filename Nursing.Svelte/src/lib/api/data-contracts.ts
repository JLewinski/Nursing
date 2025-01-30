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

export interface NursingAPIEndpointsSyncResponse {
  feedings?: NursingAPIModelsFeeding[];
}

export interface NursingAPIModelsFeeding {
  /** @format guid */
  id?: string;
  userId?: string;
  /** @format duration */
  leftDuration?: string;
  /** @format duration */
  rightDuration?: string;
  /** @format date-time */
  created?: Date;
  /** @format date-time */
  lastUpdated?: Date;
  /** @format date-time */
  deleted?: Date | null;
}

export interface NursingAPIEndpointsSyncRequest {
  /** @format date-time */
  lastSync?: Date;
  feedings?: NursingAPIModelsFeeding[];
}

export interface NursingAPIEndpointsUserInformationResponse {
  email?: string;
}
