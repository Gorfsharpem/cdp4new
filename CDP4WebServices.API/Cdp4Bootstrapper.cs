﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4Bootstrapper.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Autofac;

    using CDP4Authentication;

    using CDP4Common.Helpers;
    using CDP4Common.MetaInfo;

    using CDP4JsonSerializer;

    using CDP4MessagePackSerializer;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Authentication;
    using CDP4Orm.Dao.Cache;
    using CDP4Orm.Dao.Resolve;
    using CDP4Orm.Dao.Revision;
    using CDP4Orm.MigrationEngine;

    using CDP4WebService.Authentication;

    using CDP4WebServices.API.ChangeNotification;
    using CDP4WebServices.API.Configuration;
    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authentication;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.ChangeLog;
    using CDP4WebServices.API.Services.CherryPick;
    using CDP4WebServices.API.Services.ContributorsLocation;
    using CDP4WebServices.API.Services.DataStore;
    using CDP4WebServices.API.Services.Email;
    using CDP4WebServices.API.Services.FileHandling;
    using CDP4WebServices.API.Services.Operations;
    using CDP4WebServices.API.Services.Operations.SideEffects;
    using CDP4WebServices.API.Services.Supplemental;

    using Hangfire;

    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Bootstrappers.Autofac;
    using Nancy.Conventions;
    using Nancy.Responses;

    using NLog;

    using IServiceProvider = Services.IServiceProvider;
    using PersonResolver = Services.Authentication.PersonResolver;

    /// <summary>
    /// Provides application startup bootstrap operations.
    /// </summary>
    public class Cdp4Bootstrapper : AutofacNancyBootstrapper
    {
        /// <summary>
        /// The SiteDirectory route
        /// </summary>
        private const string SiteDirectoryRoute = "/SiteDirectory";

        /// <summary>
        /// The SiteDirectory route
        /// </summary>
        private const string EngineeringModelRoute = "/EngineeringModel";

        /// <summary>
        /// The security realm
        /// </summary>
        private const string Realm = "CDP4";

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the favorite icon.
        /// </summary>
        protected override byte[] FavIcon { get; } = LoadFavoriteIcon();

        /// <summary>
        /// Application container scope hook.
        /// </summary>
        /// <param name="existingContainer">
        /// The inversion of control container.
        /// </param>
        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            var sw = Stopwatch.StartNew();

            Logger.Info("Start Configuration of Application Container");

            // Perform registration that should have an application lifetime
            existingContainer.Update(
                builder =>
                {
                    builder.RegisterTypeAsPropertyInjectedSingleton<HeaderInfoProvider, IHeaderInfoProvider>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<DataModelUtils, IDataModelUtils>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<DefaultPermissionProvider, IDefaultPermissionProvider>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<EngineeringModelSetupDao, IEngineeringModelSetupDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<PersonPermissionDao, IPersonPermissionDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<AuthenticationDao, IAuthenticationDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<PersonRoleDao, IPersonRoleDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<ParticipantDao, IParticipantDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<ParticipantRoleDao, IParticipantRoleDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<DomainOfExpertiseDao, IDomainOfExpertiseDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<ParticipantPermissionDao, IParticipantPermissionDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<OrganizationalParticipantDao, IOrganizationalParticipantDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<PersonService, IPersonService>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<AuthenticationPluginInjector, IAuthenticationPluginInjector>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<PersonResolver, IPersonResolver>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<UserValidator, IUserValidator>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<CDP4WebServiceAuthentication, ICDP4WebServiceAuthentication>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<MigrationService, IMigrationService>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<EmailService, IEmailService>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<CherryPickService, ICherryPickService>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<ContainmentService, IContainmentService>();
                });

            Logger.Info($"Application Container configured in {sw.ElapsedMilliseconds} [ms]");

            this.ConfigureRecurringJobs();
        }

        /// <summary>
        /// Configures recurring jobs
        /// </summary>
        public void ConfigureRecurringJobs()
        {
            var sw = Stopwatch.StartNew();

            Logger.Info("Configuring cron jobs");

            var builder = new ContainerBuilder();
            builder.RegisterType<ChangeNoticationService>().InstancePerBackgroundJob();
            GlobalConfiguration.Configuration.UseAutofacActivator(builder.Build());

            if (AppConfig.Current.Changelog.AllowEmailNotification)
            {
                RecurringJob.AddOrUpdate<ChangeNoticationService>("ChangeNotificationService.Execute", notificationService => notificationService.Execute(), Cron.Weekly(DayOfWeek.Monday, 0, 15));
            }

            Logger.Info($"Cron jobs configured in {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Request container scope hook.
        /// </summary>
        /// <param name="container">
        /// The inversion of control container.
        /// </param>
        /// <param name="context">
        /// The nancy context of the request.
        /// </param>
        protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
        {
            // Perform registrations that should have a request lifetime
            var sw = Stopwatch.StartNew();

            Logger.Info("Start Configuration of Request Container");

            base.ConfigureRequestContainer(container, context);

            // wireup service providers for each request
            container.Update(
                builder =>
                {
                    // obfuscation service
                    builder.RegisterTypeAsPropertyInjectedSingleton<ObfuscationService, IObfuscationService>();

                    // local storage controller to stream binary to disk
                    builder.RegisterTypeAsPropertyInjectedSingleton<LocalFileStorage, ILocalFileStorage>();

                    // wire up the datastore controller
                    builder.RegisterTypeAsPropertyInjectedSingleton<DataStoreController, IDataStoreController>();

                    // wireup command logger for this request
                    builder.RegisterTypeAsPropertyInjectedSingleton<CommandLogger, ICommandLogger>();

                    // expose request context as injectable wrapper
                    builder.Register(c => new Cdp4RequestContext(context)).As<ICdp4RequestContext>().SingleInstance();

                    // wireup service provider
                    builder.RegisterTypeAsPropertyInjectedSingleton<ServiceProvider, IServiceProvider>();

                    // wireup change log service
                    builder.RegisterTypeAsPropertyInjectedSingleton<ChangeLogService, IChangeLogService>();

                    // wireup class meta info provider
                    builder.RegisterTypeAsPropertyInjectedSingleton<MetaInfoProvider, IMetaInfoProvider>();

                    // wireup class cdp4JsonSerializer
                    builder.RegisterTypeAsPropertyInjectedSingleton<Cdp4JsonSerializer, ICdp4JsonSerializer>();

                    // wireup class cdp4JsonSerializer
                    builder.RegisterTypeAsPropertyInjectedSingleton<MessagePackSerializer, IMessagePackSerializer>();

                    // wireup AccessRightKind service
                    builder.RegisterTypeAsPropertyInjectedSingleton<AccessRightKindService, IAccessRightKindService>();

                    // wireup permission service
                    builder.RegisterTypeAsPropertyInjectedSingleton<OrganizationalParticipationResolverService, IOrganizationalParticipationResolverService>();

                    // wireup permission service
                    builder.RegisterTypeAsPropertyInjectedSingleton<PermissionService, IPermissionService>();

                    // wireup util classes
                    builder.RegisterTypeAsPropertyInjectedSingleton<RequestUtils, IRequestUtils>();

                    // wireup exchange file processor
                    builder.RegisterTypeAsPropertyInjectedSingleton<ExchangeFileProcessor, IExchangeFileProcessor>();

                    // wireup revision read service provider
                    builder.RegisterTypeAsPropertyInjectedSingleton<RevisionDao, IRevisionDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<RevisionService, IRevisionService>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<RevisionResolver, IRevisionResolver>();

                    // wireup cache service provider
                    builder.RegisterTypeAsPropertyInjectedSingleton<CacheDao, ICacheDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<CacheService, ICacheService>();

                    // wireup resolve and container read service provider
                    builder.RegisterTypeAsPropertyInjectedSingleton<ResolveDao, IResolveDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<ContainerDao, IContainerDao>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<ResolveService, IResolveService>();

                    // wireup transaction manager
                    builder.RegisterTypeAsPropertyInjectedSingleton<Cdp4TransactionManager, ICdp4TransactionManager>();

                    // auto-wire all derived types by parent type
                    builder.RegisterDerivedTypesAsPropertyInjectedSingleton<IMetaInfo>();
                    builder.RegisterDerivedTypesAsPropertyInjectedSingleton<BaseDao>();
                    builder.RegisterDerivedTypesAsPropertyInjectedSingleton<ServiceBase>();

                    // wireup ModelCreatorManager
                    builder.RegisterTypeAsPropertyInjectedSingleton<ModelCreatorManager, IModelCreatorManager>();

                    // wireup operation processing
                    builder.RegisterDerivedTypesAsPropertyInjectedSingleton<IBusinessLogicService>();
                    builder.RegisterDerivedTypesAsPropertyInjectedSingleton<IOperationSideEffect>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<OperationSideEffectProcessor, IOperationSideEffectProcessor>();
                    builder.RegisterTypeAsPropertyInjectedSingleton<OperationProcessor, IOperationProcessor>();

                    // wireup contributor location resolver
                    builder.RegisterTypeAsPropertyInjectedSingleton<ContributorLocationResolver, IContributorLocationResolver>();

                    // wireup file archiving service
                    builder.RegisterTypeAsPropertyInjectedSingleton<FileArchiveService, IFileArchiveService>();

                    // wireup EngineeringModel zip export service
                    builder.RegisterTypeAsPropertyInjectedSingleton<EngineeringModelZipExportService, IEngineeringModelZipExportService>();

                    // wireup AccessRightKind validation service
                    builder.RegisterTypeAsPropertyInjectedSingleton<AccessRightKindValidationService, IAccessRightKindValidationService>();

                    // wireup PermissionInstanceFilter service
                    builder.RegisterTypeAsPropertyInjectedSingleton<PermissionInstanceFilterService, IPermissionInstanceFilterService>();

                    // wireup PersonService service
                    builder.RegisterTypeAsPropertyInjectedSingleton<PersonService, IPersonService>();
                });

            // apply logging configuration
            container.Resolve<ICommandLogger>().LoggingEnabled = AppConfig.Current.Backtier.LogSqlCommands;

            Logger.Info("Request Container configured in {0} [ms]", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Application startup hook.
        /// </summary>
        /// <param name="container">
        /// The inversion of control container.
        /// </param>
        /// <param name="pipelines">
        /// The pipelines.
        /// </param>
        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            var sw = Stopwatch.StartNew();

            Logger.Info("Initiate Application Startup");

            Logger.Debug("Configurating CDP4 Authentication");
            // No registrations should be performed in here, however you may resolve things that are needed during application startup.
            var cdp4WebServiceAuthentication = container.Resolve<ICDP4WebServiceAuthentication>();

            // hook up the basic authentication
            cdp4WebServiceAuthentication.Enable(
                pipelines,
                new CDP4WebServiceAuthenticationConfiguration(container.Resolve<IUserValidator>(), Realm),
                new[] { SiteDirectoryRoute, EngineeringModelRoute });

            // hook up the on error handler
            Logger.Debug("Setting up on error pipeline");
            pipelines.OnError += (ctx, ex) =>
            {
                // log any uncatched errors
                var subject = ctx.CurrentUser is Credentials credentials ? credentials.Person : null;
                var headerInforProvider = container.Resolve<IHeaderInfoProvider>();
                var requestMessage = $"[{ctx.Request.Method}][{ctx.Request.Url.Path}{ctx.Request.Url.Query}]";
                Logger.Fatal(ex, LoggerUtils.GetLogMessage(subject, ctx.Request.UserHostAddress, false, requestMessage));

                var errorResponse = new JsonResponse($"exception:{ex.Message}", new DefaultJsonSerializer());
                headerInforProvider.RegisterResponseHeaders(errorResponse,ContentTypeKind.JSON);
                return errorResponse.WithStatusCode(HttpStatusCode.InternalServerError);
            };

            Logger.Debug("Setting view-location conventions");
            // clear all view location conventions (to save on irrelevant locations being visited) and supply the Views convention to use
            this.Conventions.ViewLocationConventions.Clear();
            this.Conventions.ViewLocationConventions.Add((viewName, model, context) => $"Views/{viewName}");
            // add the folder for the static content containing the compiled app
            this.Conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("assets"));

            MigrationEngine.MigrateAllAtStartUp();

            Logger.Info($"Application Startup Finished in {sw.ElapsedMilliseconds} [ms]");
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            var sw = Stopwatch.StartNew();

            Logger.Info("Initiate Request Startup");

            //Enable CORS
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response
                    .WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "PUT, GET, POST, DELETE, OPTIONS")
                    .WithHeader("Access-Control-Allow-Headers", "Content-Type, x-requested-with, Authorization, Accept, Origin, user-agent, Accept-CDP, CDP4-Token, CDP4-Common, CDP4-Server")
                    .WithHeader("Access-Control-Expose-Headers", "Content-Type, x-requested-with, Authorization, Accept, Origin, user-agent, Accept-CDP, CDP4-Token, CDP4-Common, CDP4-Server");
            });

            Logger.Info($"Request Startup Finished in {sw.ElapsedMilliseconds} [ms]");
        }

        /// <summary>
        /// Retrieve a specific module instance from the container
        /// </summary>
        /// <param name="container">
        /// Container to use
        /// </param>
        /// <param name="moduleType">
        /// Type of the module
        /// </param>
        /// <returns>
        /// An <see cref="INancyModule"/> instance
        /// </returns>
        /// <remarks>
        /// This is an override of the <see cref="AutofacNancyBootstrapper"/> 'GetModule' implementation.
        /// It uses the same code but ensures that property auto-wiring is used for the <see cref="INancyModule"/>
        /// </remarks>
        protected override INancyModule GetModule(ILifetimeScope container, Type moduleType)
        {
            return container.Update(builder =>
                    builder.RegisterType(moduleType).As<INancyModule>()
                        .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies))
                .Resolve<INancyModule>();
        }

        /// <summary>
        /// Method in charge of loading the favicon image.
        /// </summary>
        /// <returns>Image byte array</returns>
        private static byte[] LoadFavoriteIcon()
        {
            using (var ms = new MemoryStream())
            {
                Properties.Resources.comet.Save(ms);
                return ms.ToArray();
            }
        }
    }
}
