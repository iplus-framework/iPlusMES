[ACMethodInfo("MCP", "en{'Connect MCP'}de{'MCP verbinden'}", 102, 
    Description = @"A connection is established to the configured MCP servers. 
    This method is asynchronous and must wait until the connections are established, which is signaled by the McpConnected property. 
    This method mustn't be called explicitly. It is called implicitly with the first call of SendMessage.")]
public async Task ConnectMCP()
{
    try
    {
        // Initialize MCP clients dictionary if needed
        if (_McpClients == null)
            _McpClients = new Dictionary<string, IMcpClient>();

        // Parse the JSON configuration
        McpServerConfig config;
        try
        {
            string configJson = !string.IsNullOrEmpty(MCPServerConfigFromFile) ? MCPServerConfigFromFile : MCPServerConfig;
            config = JsonSerializer.Deserialize<McpServerConfig>(configJson);
            if (config?.mcpServers == null || config.mcpServers.Count == 0)
            {
                ChatOutput = "No MCP servers configured in JSON config";
                return;
            }
        }
        catch (JsonException ex)
        {
            ChatOutput = $"Error parsing MCP server configuration JSON: {ex.Message}";
            return;
        }

        // Clear existing connections properly
        await DisconnectMCP().ConfigureAwait(false);

        // Ensure AI clients are initialized
        if (_CurrentChatClient == null)
            EnsureAIClientsInitialized();

        int totalTools = 0;
        var connectedServers = new List<string>();

        // Create MCP client with sampling capability
        var samplingHandler = _CurrentChatClient?.CreateSamplingHandler();
        var clientOptions = new McpClientOptions
        {
            Capabilities = new ClientCapabilities
            {
                Sampling = samplingHandler != null ? new SamplingCapability { SamplingHandler = samplingHandler } : null
            }
        };

        // Connect to each MCP server with proper error handling
        var connectionTasks = new List<Task>();
        var connectionResults = new ConcurrentBag<(string serverName, IMcpClient client, List<AITool> tools, Exception error)>();

        foreach (var serverEntry in config.mcpServers)
        {
            string serverName = serverEntry.Key;
            McpServerInfo serverInfo = serverEntry.Value;

            var connectionTask = ConnectToSingleServer(serverName, serverInfo, clientOptions, connectionResults);
            connectionTasks.Add(connectionTask);
        }

        // Wait for all connections to complete
        await Task.WhenAll(connectionTasks).ConfigureAwait(false);

        // Process results
        foreach (var result in connectionResults)
        {
            if (result.error == null && result.client != null)
            {
                _McpClients[result.serverName] = result.client;
                AvailableTools.AddRange(result.tools);
                totalTools += result.tools.Count;
                connectedServers.Add($"{result.serverName} ({result.tools.Count} tools)");
            }
            else
            {
                ChatOutput += $"Failed to connect to MCP server '{result.serverName}': {result.error?.Message}\n";
            }
        }

        // Update connection status
        if (_McpClients.Count > 0)
        {
            McpConnected = true;
            ChatOutput = $"Connected to {_McpClients.Count} MCP server(s): {string.Join(", ", connectedServers)}. Total {totalTools} tools available.";

            // Populate the AvailableToolsWithSelection list after loading tools
            PopulateToolsWithSelection();
        }
        else
        {
            McpConnected = false;
            ChatOutput = "Failed to connect to any MCP servers.";
        }

        OnPropertyChanged(nameof(AvailableTools));
        OnPropertyChanged(nameof(AvailableToolsNames));
    }
    catch (Exception ex)
    {
        McpConnected = false;
        ChatOutput = $"Error connecting MCP clients: {ex.Message}";
    }
}

private async Task ConnectToSingleServer(
    string serverName, 
    McpServerInfo serverInfo, 
    McpClientOptions clientOptions,
    ConcurrentBag<(string serverName, IMcpClient client, List<AITool> tools, Exception error)> results)
{
    try
    {
        // Create stdio transport for MCP server
        var transport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Command = serverInfo.command,
            Arguments = serverInfo.args ?? new string[0],
            Name = serverName,
        }, _LoggerFactory);

        // Connect to MCP server with timeout
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // 30-second timeout
        var mcpClient = await McpClientFactory.CreateAsync(
            transport,
            clientOptions,
            _LoggerFactory,
            cts.Token).ConfigureAwait(false);

        // Get available tools from this server
        var tools = await mcpClient.ListToolsAsync(cts.Token).ConfigureAwait(false);
        
        results.Add((serverName, mcpClient, tools.ToList(), null));
    }
    catch (Exception ex)
    {
        results.Add((serverName, null, new List<AITool>(), ex));
    }
}
