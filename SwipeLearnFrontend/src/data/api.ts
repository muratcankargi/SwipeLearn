import { client } from "./client";
import type {
  GetIsVideosReadyQuery,
  GetTopicQuizQuery,
  GetTopicShortInfoQuery,
  GetVideoQuery,
  PostTopicBody,
  PostTopicQuizBody,
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

export async function getTopicQuiz({ query }: { query: GetTopicQuizQuery }) {
  return await client.GET("/api/Topic/quiz", { params: { query } });
}

export async function postTopicQuiz({ body }: { body: PostTopicQuizBody }) {
  return await client.POST("/api/Topic/quiz", { body });
}
