﻿using System;

namespace Elders.Cronus.Projections.Snapshotting
{
    public interface ISnapshot
    {
        IBlobId Id { get; }
        int Revision { get; }
        object State { get; }
        string ProjectionName { get; }
    }

    public class SnapshotMeta
    {
        SnapshotMeta() { }

        public SnapshotMeta(int revision, string projectionName)
        {
            if (revision < 0) throw new ArgumentException($"{revision} must be >= 0", nameof(revision));
            if (string.IsNullOrEmpty(projectionName)) throw new ArgumentNullException(nameof(projectionName));

            Revision = revision;
            ProjectionName = projectionName;
        }

        public int Revision { get; private set; }

        public string ProjectionName { get; private set; }

        public static SnapshotMeta From(ISnapshot snapshot) => new SnapshotMeta(snapshot.Revision, snapshot.ProjectionName);
    }
}
