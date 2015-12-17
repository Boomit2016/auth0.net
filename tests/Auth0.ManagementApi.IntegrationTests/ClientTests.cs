﻿using System;
using System.Threading.Tasks;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi.Models;
using FluentAssertions;
using NUnit.Framework;
using Auth0.Tests.Shared;

namespace Auth0.ManagementApi.IntegrationTests
{
    [TestFixture]
    public class ClientTests : TestBase
    {
        [Test]
        public async Task Test_client_crud_sequence()
        {
            var scopes = new
            {
                clients = new
                {
                    actions = new string[] { "read", "create", "delete", "update" }
                },
                client_keys = new
                {
                    actions = new string[] { "read", "update" }
                }
            };
            string token = GenerateToken(scopes);

            var apiClient = new ManagementApiClient(token, new Uri(GetVariable("AUTH0_MANAGEMENT_API_URL")));

            // Get all clients
            var clientsBefore = await apiClient.Clients.GetAll();

            // Add a new client
            var newClientRequest = new ClientCreateRequest
            {
                Name = $"Integration testing {Guid.NewGuid().ToString("N")}"
            };
            var newClientResponse = await apiClient.Clients.Create(newClientRequest);
            newClientResponse.Should().NotBeNull();
            newClientResponse.Name.Should().Be(newClientRequest.Name);

            // Get all clients again, and ensure we have one client more
            var clientsAfter = await apiClient.Clients.GetAll();
            clientsAfter.Count.Should().Be(clientsBefore.Count + 1);

            // Update the client
            var updateClientRequest = new ClientUpdateRequest
            {
                Name = $"Integration testing {Guid.NewGuid().ToString("N")}"
            };
            var updateClientResponse = await apiClient.Clients.Update(newClientResponse.ClientId, updateClientRequest);
            updateClientResponse.Should().NotBeNull();
            updateClientResponse.Name.Should().Be(updateClientRequest.Name);

            // Get a single client
            var client = await apiClient.Clients.Get(newClientResponse.ClientId);
            client.Should().NotBeNull();
            client.Name.Should().Be(updateClientResponse.Name);

            // Delete the client, and ensure we get exception when trying to fetch client again
            await apiClient.Clients.Delete(client.ClientId);
            Func<Task> getFunc = async () => await apiClient.Clients.Get(client.ClientId);
            getFunc.ShouldThrow<ApiException>().And.ApiError.ErrorCode.Should().Be("inexistent_client");
        }
    }
}
