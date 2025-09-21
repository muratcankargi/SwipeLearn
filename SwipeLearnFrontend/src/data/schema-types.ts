import type { paths } from "./schema";

export type PostTopicBody = NonNullable<
  paths["/api/Topic"]["post"]["requestBody"]
>["content"]["application/json"];

export type GetTopicShortInfoQuery =
  paths["/api/Topic/short-info"]["get"]["parameters"]["query"];

export type GetIsVideosReadyQuery =
  paths["/api/Topic/is-videos-ready"]["get"]["parameters"]["query"];

export type GetVideoQuery = paths["/api/video"]["get"]["parameters"]["query"];

export type GetTopicQuizQuery =
  paths["/api/Topic/quiz"]["get"]["parameters"]["query"];

export type PostTopicQuizBody = NonNullable<
  paths["/api/Topic/quiz"]["post"]["requestBody"]
>["content"]["application/json"];
