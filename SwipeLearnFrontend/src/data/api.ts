import { client } from "./client";
import type {
  GetIsVideosReadyQuery,
  GetTopicShortInfoQuery,
  GetVideoQuery,
  PostTopicBody,
} from "./schema-types";

export async function postTopic({ body }: { body: PostTopicBody }) {
  return await client.POST("/api/Topic", { body });
}

export async function getTopicShortInfo({
  query,
}: {
  query: GetTopicShortInfoQuery;
}) {
  return await client.GET("/api/Topic/short-info", { params: { query } });
}

export async function getIsVideosReady({
  query,
}: {
  query: GetIsVideosReadyQuery;
}) {
  return await client.GET("/api/Topic/is-videos-ready", { params: { query } });
}

export async function getVideo({ query }: { query: GetVideoQuery }) {
  return await client.GET("/api/video", { params: { query } });
}
