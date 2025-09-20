import type { paths } from "./schema";

export type PostTopicBody = NonNullable<
  paths["/api/Topic"]["post"]["requestBody"]
>["content"]["application/json"];
