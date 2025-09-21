import { useMutation, useQuery } from "@tanstack/react-query";
import {
  getIsVideosReady,
  getTopicQuiz,
  getTopicShortInfo,
  getVideo,
  postTopic,
  postTopicQuiz,
} from "./api";
import type {
  GetIsVideosReadyQuery,
  GetTopicQuizQuery,
  GetTopicShortInfoQuery,
  GetVideoQuery,
  PostTopicBody,
  PostTopicQuizBody,
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

export function useGetVideo({ query }: { query: GetVideoQuery }) {
  return useQuery({
    queryKey: ["get-video", query],
    queryFn: async () => {
      const { data, error } = await getVideo({ query });

      if (error) {
        console.log("Error: ", error);
        throw error;
      }

      return data;
    },
  });
}

export function useGetTopicQuiz({ query }: { query: GetTopicQuizQuery }) {
  return useQuery({
    queryKey: ["get-topic-quiz", query],
    queryFn: async () => {
      const { data, error } = await getTopicQuiz({ query });

      if (error) {
        console.log("Error: ", error);
        throw error;
      }

      return data;
    },
  });
}

export function usePostTopicQuiz() {
  return useMutation({
    mutationKey: ["post-topic-quiz"],
    mutationFn: async ({ body }: { body: PostTopicQuizBody }) => {
      const { data, error } = await postTopicQuiz({ body });

      if (error) {
        console.log("Error: ", error);
        throw error;
      }

      return data;
    },
  });
}
