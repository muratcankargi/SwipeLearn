import type { paths } from "./schema";

export type PostTopicBody = NonNullable<
  paths["/api/Topic"]["post"]["requestBody"]
>["content"]["application/json"];

export type GetTopicShortInfoQuery =
  paths["/api/Topic/short-info"]["get"]["parameters"]["query"];

export type GetIsVideosReadyQuery =
  paths["/api/Topic/is-videos-ready"]["get"]["parameters"]["query"];

export type GetVideoQuery = paths["/api/video"]["get"]["parameters"]["query"];
