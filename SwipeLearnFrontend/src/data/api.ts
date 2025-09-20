import { client } from "./client";
import { PostTopicBody } from "./schema-types";

export async function postTopic({ body }: { body: PostTopicBody }) {
  return await client.POST("/api/Topic", { body });
}
