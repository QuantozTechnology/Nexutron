using System;
using Xunit;

namespace Nexutron.LocalNode.Tests;

/// <summary>
/// These tests need a running java-tron node (see tests/local-node). They run only when
/// NEXUTRON_LOCAL_NODE is set to the node's host name, e.g. NEXUTRON_LOCAL_NODE=localhost.
/// </summary>
public sealed class LocalNodeFactAttribute : FactAttribute
{
    public LocalNodeFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("NEXUTRON_LOCAL_NODE")))
        {
            Skip = "Set NEXUTRON_LOCAL_NODE to run against a local java-tron node (tests/local-node/start.sh).";
        }
    }
}
