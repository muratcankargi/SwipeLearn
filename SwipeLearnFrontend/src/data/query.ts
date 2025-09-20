import { useMutation } from "@tanstack/react-query";
import { postTopic } from "./api";
import type { PostTopicBody } from "./schema-types";

export function usePostTopic() {
  return useMutation({
    mutationKey: ["post-topic"],
    mutationFn: async ({ body }: { body: PostTopicBody }) => {
      const { data, error } = await postTopic({ body });

      if (error) {
        console.log("Error: ", error);
        throw error;
      }

      return data;
    },
  });
}
