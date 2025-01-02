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

export interface AccessTokenResponse {
  tokenType?: string | null;
  accessToken: string;
  /** @format int64 */
  expiresIn: number;
  refreshToken: string;
}

export interface FeedingDto {
  /** @format uuid */
  id?: string;
  /** @pattern ^-?(\d+\.)?\d{2}:\d{2}:\d{2}(\.\d{1,7})?$ */
  leftBreastTotal?: string;
  /** @pattern ^-?(\d+\.)?\d{2}:\d{2}:\d{2}(\.\d{1,7})?$ */
  rightBreastTotal?: string;
  /** @pattern ^-?(\d+\.)?\d{2}:\d{2}:\d{2}(\.\d{1,7})?$ */
  totalTime?: string;
  /** @format date-time */
  started?: Date;
  /** @format date-time */
  finished?: Date;
  lastIsLeft?: boolean;
  /** @format date-time */
  lastUpdated?: Date;
  /** @format date-time */
  deleted?: Date;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface HttpValidationProblemDetails {
  type?: string | null;
  title?: string | null;
  /** @format int32 */
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  errors?: Record<string, string[]>;
}

export interface InfoRequest {
  newEmail?: string | null;
  newPassword?: string | null;
  oldPassword?: string | null;
}

export interface InfoResponse {
  email: string;
  isEmailConfirmed: boolean;
}

export interface InviteViewModel {
  users: string[];
  /** @format uuid */
  id?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
  twoFactorCode?: string | null;
  twoFactorRecoveryCode?: string | null;
}

export interface RefreshRequest {
  refreshToken: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export interface ResendConfirmationEmailRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  resetCode: string;
  newPassword: string;
}

export interface SyncModel {
  /** @format date-time */
  lastSync?: Date;
  feedings?: FeedingDto[];
}

export interface SyncResult {
  success?: boolean;
  feedings: FeedingDto[];
  badIds: string[];
  /** @format int32 */
  updates?: number;
}

export interface TwoFactorRequest {
  enable?: boolean | null;
  twoFactorCode?: string | null;
  resetSharedKey?: boolean;
  resetRecoveryCodes?: boolean;
  forgetMachine?: boolean;
}

export interface TwoFactorResponse {
  sharedKey: string;
  /** @format int32 */
  recoveryCodesLeft: number;
  recoveryCodes?: string[] | null;
  isTwoFactorEnabled: boolean;
  isMachineRemembered: boolean;
}

export type QueryParamsType = Record<string | number, any>;
export type ResponseFormat = keyof Omit<Body, "body" | "bodyUsed">;

export interface FullRequestParams extends Omit<RequestInit, "body"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseFormat;
  /** request body */
  body?: unknown;
  /** base url */
  baseUrl?: string;
  /** request cancellation token */
  cancelToken?: CancelToken;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> {
  baseUrl?: string;
  baseApiParams?: Omit<RequestParams, "baseUrl" | "cancelToken" | "signal">;
  securityWorker?: (securityData: SecurityDataType | null) => Promise<RequestParams | void> | RequestParams | void;
  customFetch?: typeof fetch;
}

export interface HttpResponse<D extends unknown, E extends unknown = unknown> extends Response {
  data: D;
  error: E;
}

type CancelToken = Symbol | string | number;

export enum ContentType {
  Json = "application/json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public baseUrl: string = "/";
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private abortControllers = new Map<CancelToken, AbortController>();
  private customFetch = (...fetchParams: Parameters<typeof fetch>) => fetch(...fetchParams);

  private baseApiParams: RequestParams = {
    credentials: "same-origin",
    headers: {},
    redirect: "follow",
    referrerPolicy: "no-referrer",
  };

  constructor(apiConfig: ApiConfig<SecurityDataType> = {}) {
    Object.assign(this, apiConfig);
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected encodeQueryParam(key: string, value: any) {
    const encodedKey = encodeURIComponent(key);
    return `${encodedKey}=${encodeURIComponent(typeof value === "number" ? value : `${value}`)}`;
  }

  protected addQueryParam(query: QueryParamsType, key: string) {
    return this.encodeQueryParam(key, query[key]);
  }

  protected addArrayQueryParam(query: QueryParamsType, key: string) {
    const value = query[key];
    return value.map((v: any) => this.encodeQueryParam(key, v)).join("&");
  }

  protected toQueryString(rawQuery?: QueryParamsType): string {
    const query = rawQuery || {};
    const keys = Object.keys(query).filter((key) => "undefined" !== typeof query[key]);
    return keys
      .map((key) => (Array.isArray(query[key]) ? this.addArrayQueryParam(query, key) : this.addQueryParam(query, key)))
      .join("&");
  }

  protected addQueryParams(rawQuery?: QueryParamsType): string {
    const queryString = this.toQueryString(rawQuery);
    return queryString ? `?${queryString}` : "";
  }

  private contentFormatters: Record<ContentType, (input: any) => any> = {
    [ContentType.Json]: (input: any) =>
      input !== null && (typeof input === "object" || typeof input === "string") ? JSON.stringify(input) : input,
    [ContentType.Text]: (input: any) => (input !== null && typeof input !== "string" ? JSON.stringify(input) : input),
    [ContentType.FormData]: (input: any) =>
      Object.keys(input || {}).reduce((formData, key) => {
        const property = input[key];
        formData.append(
          key,
          property instanceof Blob
            ? property
            : typeof property === "object" && property !== null
              ? JSON.stringify(property)
              : `${property}`,
        );
        return formData;
      }, new FormData()),
    [ContentType.UrlEncoded]: (input: any) => this.toQueryString(input),
  };

  protected mergeRequestParams(params1: RequestParams, params2?: RequestParams): RequestParams {
    return {
      ...this.baseApiParams,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...(this.baseApiParams.headers || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected createAbortSignal = (cancelToken: CancelToken): AbortSignal | undefined => {
    if (this.abortControllers.has(cancelToken)) {
      const abortController = this.abortControllers.get(cancelToken);
      if (abortController) {
        return abortController.signal;
      }
      return void 0;
    }

    const abortController = new AbortController();
    this.abortControllers.set(cancelToken, abortController);
    return abortController.signal;
  };

  public abortRequest = (cancelToken: CancelToken) => {
    const abortController = this.abortControllers.get(cancelToken);

    if (abortController) {
      abortController.abort();
      this.abortControllers.delete(cancelToken);
    }
  };

  public request = async <T = any, E = any>({
    body,
    secure,
    path,
    type,
    query,
    format,
    baseUrl,
    cancelToken,
    ...params
  }: FullRequestParams): Promise<HttpResponse<T, E>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.baseApiParams.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const queryString = query && this.toQueryString(query);
    const payloadFormatter = this.contentFormatters[type || ContentType.Json];
    const responseFormat = format || requestParams.format;

    return this.customFetch(`${baseUrl || this.baseUrl || ""}${path}${queryString ? `?${queryString}` : ""}`, {
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type && type !== ContentType.FormData ? { "Content-Type": type } : {}),
      },
      signal: (cancelToken ? this.createAbortSignal(cancelToken) : requestParams.signal) || null,
      body: typeof body === "undefined" || body === null ? null : payloadFormatter(body),
    }).then(async (response) => {
      const r = response.clone() as HttpResponse<T, E>;
      r.data = null as unknown as T;
      r.error = null as unknown as E;

      const data = !responseFormat
        ? r
        : await response[responseFormat]()
            .then((data) => {
              if (r.ok) {
                r.data = data;
              } else {
                r.error = data;
              }
              return r;
            })
            .catch((e) => {
              r.error = e;
              return r;
            });

      if (cancelToken) {
        this.abortControllers.delete(cancelToken);
      }

      if (!response.ok) throw data;
      return data;
    });
  };
}

/**
 * @title Nursing API
 * @version 1.0.0
 * @baseUrl /
 *
 * API for Nursing App
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
  auth = {
    /**
     * No description
     *
     * @tags Nursing.API
     * @name RegisterCreate
     * @request POST:/auth/register
     */
    registerCreate: (data: RegisterRequest, params: RequestParams = {}) =>
      this.request<void, HttpValidationProblemDetails | void>({
        path: `/auth/register`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Nursing.API
     * @name LoginCreate
     * @request POST:/auth/login
     */
    loginCreate: (
      data: LoginRequest,
      query?: {
        useCookies?: boolean;
        useSessionCookies?: boolean;
      },
      params: RequestParams = {},
    ) =>
      this.request<AccessTokenResponse, void>({
        path: `/auth/login`,
        method: "POST",
        query: query,
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Nursing.API
     * @name RefreshCreate
     * @request POST:/auth/refresh
     */
    refreshCreate: (data: RefreshRequest, params: RequestParams = {}) =>
      this.request<AccessTokenResponse, void>({
        path: `/auth/refresh`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Nursing.API
     * @name MapIdentityApiAuthConfirmEmail
     * @request GET:/auth/confirmEmail
     */
    mapIdentityApiAuthConfirmEmail: (
      query: {
        userId: string;
        code: string;
        changedEmail?: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<void, void>({
        path: `/auth/confirmEmail`,
        method: "GET",
        query: query,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Nursing.API
     * @name ResendConfirmationEmailCreate
     * @request POST:/auth/resendConfirmationEmail
     */
    resendConfirmationEmailCreate: (data: ResendConfirmationEmailRequest, params: RequestParams = {}) =>
      this.request<void, void>({
        path: `/auth/resendConfirmationEmail`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Nursing.API
     * @name ForgotPasswordCreate
     * @request POST:/auth/forgotPassword
     */
    forgotPasswordCreate: (data: ForgotPasswordRequest, params: RequestParams = {}) =>
      this.request<void, HttpValidationProblemDetails | void>({
        path: `/auth/forgotPassword`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Nursing.API
     * @name ResetPasswordCreate
     * @request POST:/auth/resetPassword
     */
    resetPasswordCreate: (data: ResetPasswordRequest, params: RequestParams = {}) =>
      this.request<void, HttpValidationProblemDetails | void>({
        path: `/auth/resetPassword`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Nursing.API
     * @name Manage2FaCreate
     * @request POST:/auth/manage/2fa
     */
    manage2FaCreate: (data: TwoFactorRequest, params: RequestParams = {}) =>
      this.request<TwoFactorResponse, HttpValidationProblemDetails | void>({
        path: `/auth/manage/2fa`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Nursing.API
     * @name ManageInfoList
     * @request GET:/auth/manage/info
     */
    manageInfoList: (params: RequestParams = {}) =>
      this.request<InfoResponse, HttpValidationProblemDetails | void>({
        path: `/auth/manage/info`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Nursing.API
     * @name ManageInfoCreate
     * @request POST:/auth/manage/info
     */
    manageInfoCreate: (data: InfoRequest, params: RequestParams = {}) =>
      this.request<InfoResponse, HttpValidationProblemDetails | void>({
        path: `/auth/manage/info`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),
  };
  api = {
    /**
     * No description
     *
     * @tags Sync
     * @name Sync
     * @request POST:/api/Sync/sync
     * @secure
     */
    sync: (data: SyncModel, params: RequestParams = {}) =>
      this.request<SyncResult, void>({
        path: `/api/Sync/sync`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Sync
     * @name GetInvites
     * @request GET:/api/Sync/invites
     * @secure
     */
    getInvites: (params: RequestParams = {}) =>
      this.request<InviteViewModel[], void>({
        path: `/api/Sync/invites`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Sync
     * @name SendInvite
     * @request POST:/api/Sync/sendInvite
     * @secure
     */
    sendInvite: (data: string, params: RequestParams = {}) =>
      this.request<void, void>({
        path: `/api/Sync/sendInvite`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Sync
     * @name AcceptInvite
     * @request POST:/api/Sync/acceptInvite
     * @secure
     */
    acceptInvite: (data: string, params: RequestParams = {}) =>
      this.request<void, void>({
        path: `/api/Sync/acceptInvite`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Sync
     * @name DeclineInvite
     * @request POST:/api/Sync/declineInvite
     * @secure
     */
    declineInvite: (data: string, params: RequestParams = {}) =>
      this.request<void, void>({
        path: `/api/Sync/declineInvite`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Sync
     * @name Delete
     * @request POST:/api/Sync/delete
     * @secure
     */
    delete: (data: string[], params: RequestParams = {}) =>
      this.request<void, void>({
        path: `/api/Sync/delete`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),
  };
}
