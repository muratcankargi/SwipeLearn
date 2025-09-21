import { useMutation, useQuery } from "@tanstack/react-query";
import { getIsVideosReady, getTopicShortInfo, postTopic } from "./api";
import type {
  GetIsVideosReadyQuery,
  GetTopicShortInfoQuery,
  PostTopicBody,
} from "./schema-types";
import { useNavigate } from "react-router";

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

export function useGetTopicShortInfo({
  query,
}: {
  query: GetTopicShortInfoQuery;
}) {
  return useQuery({
    retry: true,
    refetchOnWindowFocus: false,
    queryKey: ["get-topic-short-info", query],
    queryFn: async () => {
      const { data, error } = await getTopicShortInfo({ query });

      if (error) {
        console.log("Error: ", error);
        throw error;
      }

      return data;
    },
  });
}

export function useGetIsVideosReady({
  query,
}: {
  query: GetIsVideosReadyQuery;
}) {
  const navigate = useNavigate();

  return useQuery({
    refetchInterval: 10000,
    queryKey: ["get-is-videos-ready", query],
    queryFn: async () => {
      const { data, error } = await getIsVideosReady({ query });

      if (error) {
        console.log("Error: ", error);
        throw error;
      }

      if (data.isReady) {
        navigate(`/kaydir/${query?.id}`);
      }

      return data;
    },
  });
}
