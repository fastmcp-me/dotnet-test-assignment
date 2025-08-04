﻿using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Api.Tools;

/// <summary>
/// Sample MCP tools for demonstration purposes.
/// These tools can be invoked by MCP clients to perform various operations.
/// </summary>
public class RandomNumberTools
{
    [McpServerTool]
    [Description("Generates a random number between the specified minimum and maximum values.")]
    public int GetRandomNumber(
        [Description("Minimum value (inclusive)")]
        int min = 0,
        [Description("Maximum value (exclusive)")]
        int max = 100)
    {
        return Random.Shared.Next(min, max);
    }
}