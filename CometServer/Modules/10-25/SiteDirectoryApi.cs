﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteDirectoryApi.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Threading.Tasks;

    using Carter.Response;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations;
    using CometServer.Services.Protocol;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;

    using NLog;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// This is an API endpoint class to support interaction with the site directory contained model data
    /// </summary>
    public class SiteDirectoryApi : ApiBase
    {
        /// <summary>
        /// The top container.
        /// </summary>
        private const string TopContainer = "SiteDirectory";

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The supported get query parameters.
        /// </summary>
        private static readonly string[] SupportedGetQueryParameters =
            {
                QueryParameters.ExtentQuery, 
                QueryParameters.IncludeReferenceDataQuery,
                QueryParameters.IncludeAllContainersQuery, 
                QueryParameters.IncludeFileDataQuery,
                QueryParameters.RevisionNumberQuery, 
                QueryParameters.RevisionFromQuery, 
                QueryParameters.RevisionToQuery
            };

        /// <summary>
        /// The supported post query parameter.
        /// </summary>
        private static readonly string[] SupportedPostQueryParameter =
            { 
                QueryParameters.RevisionNumberQuery, 
                QueryParameters.ExportQuery
            };

        /// <summary>
        /// Gets or sets the model creator manager service.
        /// </summary>
        public IModelCreatorManager ModelCreatorManager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteDirectoryApi"/> class.
        /// </summary>
        public SiteDirectoryApi()
        {
            this.Get("SiteDirectory", async (req, res) =>
            {
                if (!req.HttpContext.User.Identity.IsAuthenticated)
                {
                    res.UpdateWithNotAuthenticatedSettings();
                    await res.AsJson("not authenticated");
                }
                else
                {
                    await this.Authorize(req.HttpContext.User.Identity.Name);

                    await this.GetResponseData(req, res);
                }
            });

            this.Get("SiteDirectory/{*path}", async (req, res) =>
            {
                if (!req.HttpContext.User.Identity.IsAuthenticated)
                {
                    res.UpdateWithNotAuthenticatedSettings();
                    await res.AsJson("not authenticated");
                }
                else
                {
                    await this.Authorize(req.HttpContext.User.Identity.Name);

                    await this.GetResponseData(req, res);
                }
            });

            this.Post("SiteDirectory/{iid:guid}", async (req, res) =>
            {
                if (!req.HttpContext.User.Identity.IsAuthenticated)
                {
                    res.UpdateWithNotAuthenticatedSettings();
                    await res.AsJson("not authenticated");
                }
                else
                {
                    await this.Authorize(req.HttpContext.User.Identity.Name);

                    await this.PostResponseData(req, res);
                }
            });
        }

        /// <summary>
        /// Handles the GET request
        /// </summary>
        /// <param name="httpRequest">
        /// The <see cref="HttpRequest"/> that is being handled
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task GetResponseData(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            this.TransactionManager.SetCachedDtoReadEnabled(true);

            var sw = Stopwatch.StartNew();
            var requestToken = this.GenerateRandomToken();
            
            try
            {
                Logger.Info(this.ConstructLog(httpRequest,$"{requestToken} database operations started"));

                // validate (and set) the supplied query parameters
                HttpRequestHelper.ValidateSupportedQueryParameter(httpRequest.Query, SupportedGetQueryParameters);
                this.RequestUtils.QueryParameters = new QueryParameters(httpRequest.Query);

                var fromRevision = this.RequestUtils.QueryParameters.RevisionNumber;

                IReadOnlyList<Thing> things = null;

                // get prepared data source transaction
                transaction = this.TransactionManager.SetupTransaction(ref connection, this.CredentialsService.Credentials);

                if (fromRevision > -1)
                {
                    // gather all Things that are newer then the indicated revision

                    things = this.RevisionService.Get(transaction, TopContainer, fromRevision, true).ToList();
                }
                else if (this.RevisionResolver.TryResolve(transaction, TopContainer, this.RequestUtils.QueryParameters.RevisionFrom, this.RequestUtils.QueryParameters.RevisionTo, out var resolvedValues))
                {
                    var routeSegments = HttpRequestHelper.ParseRouteSegments(httpRequest.Path);

                    var iid = routeSegments.Last();

                    if (!Guid.TryParse(iid, out var guid))
                    {
                        await httpResponse.AsJson("The identifier of the object to query was not found or the route is invalid.");
                        httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        return;
                    }

                    things = this.RevisionService.Get(transaction, TopContainer, guid, resolvedValues.FromRevision, resolvedValues.ToRevision).ToList();
                }
                else
                {
                    // gather all Things as indicated by the request URI 
                    var routeSegments = HttpRequestHelper.ParseRouteSegments(httpRequest.Path);
                    
                    things = this.GetContainmentResponse(transaction, TopContainer, routeSegments).ToList();
                }

                await transaction.CommitAsync();

                sw.Stop();
                Logger.Info("Database operations {0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);

                sw.Start();
                Logger.Info("return {0} response started", requestToken);

                var version = this.RequestUtils.GetRequestDataModelVersion(httpRequest);

                await this.WriteJsonResponse(things, version, httpResponse, HttpStatusCode.OK, requestToken);
            }
            catch (SecurityException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Debug(ex, this.ConstructFailureLog(httpRequest, $"unauthorized request {requestToken} returned after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Error(ex, this.ConstructFailureLog(httpRequest,$"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }

                if (connection != null)
                {
                    await connection.DisposeAsync();
                }

                sw.Stop();
                Logger.Info("Response {0} returned in {1} [ms]", requestToken, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Handles the POST requset
        /// </summary>
        /// <param name="httpRequest">
        /// The <see cref="HttpRequest"/> that is being handled
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task PostResponseData(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            var sw = new Stopwatch();
            sw.Start();
            var requestToken = this.GenerateRandomToken();
            
            try
            {
                Logger.Info(this.ConstructLog(httpRequest, $"{requestToken} started"));

                HttpRequestHelper.ValidateSupportedQueryParameter(httpRequest.Query, SupportedPostQueryParameter);
                this.RequestUtils.QueryParameters = new QueryParameters(httpRequest.Query);

                var version = this.RequestUtils.GetRequestDataModelVersion(httpRequest);
                
                var isMultiPart = httpRequest.GetMultipartBoundary() != string.Empty;
                
                if (isMultiPart)
                {
                    // multipart message received
                    throw new InvalidOperationException($"Multipart post messages are not allowed for the {TopContainer} route");
                }

                this.JsonSerializer.Initialize(this.MetaInfoProvider, version);
                var operationData = this.JsonSerializer.Deserialize<CdpPostOperation>(httpRequest.Body);

                transaction = this.TransactionManager.SetupTransaction(ref connection, this.CredentialsService.Credentials);

                // defer all reference data check until after transaction commit
                using (var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED", transaction.Connection, transaction))
                {
                    command.ExecuteAndLogNonQuery(this.TransactionManager.CommandLogger);
                }

                // retrieve the revision for this transaction (or get next revision if it does not exist)
                var transactionRevision = this.RevisionService.GetRevisionForTransaction(transaction, TopContainer);

                this.OperationProcessor.Process(operationData, transaction, TopContainer);

                // save revision-history
                var actor = this.CredentialsService.Credentials.Person.Iid;
                var changedThings = this.RevisionService.SaveRevisions(transaction, TopContainer, actor, transactionRevision).ToList();

                // commit the operation + revision-history
                await transaction.CommitAsync();

                if (this.ModelCreatorManager.IsUserTriggerDisable)
                {
                    // re-enable user triggers
                    transaction = this.TransactionManager.SetupTransaction(ref connection, this.CredentialsService.Credentials);
                    this.ModelCreatorManager.EnableUserTrigger(transaction);
                    await transaction.CommitAsync();
                }

                if (this.RequestUtils.QueryParameters.RevisionNumber == -1)
                {
                    Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);
                    await this.WriteJsonResponse(changedThings, version, httpResponse);
                    return;
                }

                // get the latest revision state including revisions that may have happened meanwhile
                Logger.Info(this.ConstructLog(httpRequest));
                var fromRevision = this.RequestUtils.QueryParameters.RevisionNumber;

                // use new transaction to include latest database state
                transaction = this.TransactionManager.SetupTransaction(ref connection, this.CredentialsService.Credentials);
                var revisionResponse = this.RevisionService.Get(transaction, TopContainer, fromRevision, true).ToArray();
                await transaction.CommitAsync();

                Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);

                await this.WriteJsonResponse(revisionResponse, version, httpResponse);
            }
            catch (InvalidOperationException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Error(ex, this.ConstructFailureLog(httpRequest,$"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (SecurityException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Debug(ex, this.ConstructFailureLog(httpRequest, $"unauthorized request {requestToken} returned after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Error(ex, this.ConstructFailureLog(httpRequest, $"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Get the SiteDirectory containment response from the request path.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="routeParams">
        /// The route Parameters.
        /// </param>
        /// <returns>
        /// The list of containment <see cref="Thing"/>.
        /// </returns>
        private IEnumerable<Thing> GetContainmentResponse(NpgsqlTransaction transaction, string partition, string[] routeParams)
        {
            var processor = new ResourceProcessor(transaction, this.ServiceProvider, this.RequestUtils, this.MetaInfoProvider);

            if (routeParams.Length == 1)
            {
                // sitedirectory singleton resource request (IncludeReferenceData is handled in the sitedirectory service logic)

                var things = processor.GetResource(TopContainer, partition, null, new RequestSecurityContext { ContainerReadAllowed = true });

                foreach (var thing in things)
                {
                    yield return thing;
                }
            }
            else
            {
                var things = this.ProcessRequestPath(processor, TopContainer, partition, routeParams, out var resolvedResourcePath);

                foreach (var thing in things)
                {
                    yield return thing;
                }

                if (resolvedResourcePath.Count() > 1 && this.RequestUtils.QueryParameters.IncludeReferenceData)
                {
                    // add reference data information if the resource is a model reference data library
                    if (resolvedResourcePath.Last().GetType() == typeof(ModelReferenceDataLibrary))
                    {
                        foreach (var thing in this.CollectReferenceDataLibraryChain(processor, (ModelReferenceDataLibrary)resolvedResourcePath.Last()))
                        {
                            yield return thing;
                        }
                    }
                }
            }
        }
    }
}