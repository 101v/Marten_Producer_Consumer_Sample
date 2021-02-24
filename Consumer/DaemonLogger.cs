using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baseline;
using Marten.Events.Projections.Async;
using Topshelf.Logging;

namespace Consumer
{
    public class DaemonLogger : IDaemonLogger
    {
        private readonly LogWriter logger;

        public DaemonLogger(LogWriter logger)
        {
            this.logger = logger;
        }

        public void BeginStartAll(IEnumerable<IProjectionTrack> values)
        {
            logger.Info($"Starting all tracks: {values.Select(x => x.ViewType.FullName).Join(", ")}");
        }

        public void DeterminedStartingPosition(IProjectionTrack track)
        {
            logger.Info($"Projection {track.ViewType.FullName} is starting > {track.LastEncountered}");
        }

        public void FinishedStartingAll()
        {
            logger.Info("Finished starting the async daemon");
        }

        public void BeginRebuildAll(IEnumerable<IProjectionTrack> values)
        {
            logger.Info($"Beginning a Rebuild of {values.Select(x => x.ViewType.FullName).Join(", ")}");
        }

        public void FinishRebuildAll(TaskStatus status, AggregateException exception)
        {
            logger.Info($"Finished RebuildAll with status {status}");

            if (exception == null) return;

            var flattened = exception.Flatten();
            logger.Error(flattened.ToString(), flattened);
        }

        public void BeginStopAll()
        {
            logger.Info("Beginning to stop the Async Daemon");
        }

        public void AllStopped()
        {
            logger.Info("Daemon stopped successfully");
        }

        public void PausingFetching(IProjectionTrack track, long lastEncountered)
        {
            logger.Info($"Pausing fetching for {track.ViewType.FullName}, last encountered {lastEncountered}");
        }

        public void FetchStarted(IProjectionTrack track)
        {
            logger.Info($"Starting fetching for {track.ViewType.FullName}");
        }

        public void FetchingIsAtEndOfEvents(IProjectionTrack track)
        {
            logger.Info($"Fetching is at the end of the event log for {track.ViewType.FullName}");
        }

        public void FetchingStopped(IProjectionTrack track)
        {
            logger.Info($"Stopped event fetching for {track.ViewType.FullName}");
        }

        public void PageExecuted(EventPage page, IProjectionTrack track)
        {
            logger.Info($"{page} executed for {track.ViewType.FullName}");
        }

        public void FetchingFinished(IProjectionTrack track, long lastEncountered)
        {
            logger.Info($"Fetching finished for {track.ViewType.FullName} at event {lastEncountered}");
        }

        public void StartingProjection(IProjectionTrack track, DaemonLifecycle lifecycle)
        {
            logger.Info($"Starting projection {track.ViewType.FullName} running as {lifecycle}");
        }

        public void Stopping(IProjectionTrack track)
        {
            logger.Info($"Stopping projection {track.ViewType.FullName}");
        }

        public void Stopped(IProjectionTrack track)
        {
            logger.Info($"Stopped projection {track.ViewType.FullName}");
        }

        public void ProjectionBackedUp(IProjectionTrack track, int cachedEventCount, EventPage page)
        {
            logger.Info($"Projection {track.ViewType.FullName} is backed up with {cachedEventCount} events in memory, last page fetched was {page}");
        }

        public void ClearingExistingState(IProjectionTrack track)
        {
            logger.Info($"Clearing the existing state for projection {track.ViewType.FullName}");
        }

        public void Error(Exception exception)
        {
            logger.Error(exception.ToString(), exception);
        }
    }
}
