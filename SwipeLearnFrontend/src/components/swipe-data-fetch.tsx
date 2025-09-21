import { useGetVideo } from "@/data/query";
import { Swipe } from "@/pages/Swipe";
import { useParams } from "react-router";
import { LoadingIndicator } from "./ui/loading-indicator";

export function SwipeDataFetch() {
  const params = useParams<{ id: string }>();

  const videoQuery = useGetVideo({ query: { id: params.id } });

  if (videoQuery.isLoading) return <LoadingIndicator />;
  if (!videoQuery.data?.videoUrls || videoQuery.isError)
    return <div>Bir şeyler ters gitti.</div>;

  return (
    <Swipe
      videoUrls={videoQuery.data.videoUrls}
      transcripts={videoQuery.data.descriptions ?? []}
    />
  );
}
