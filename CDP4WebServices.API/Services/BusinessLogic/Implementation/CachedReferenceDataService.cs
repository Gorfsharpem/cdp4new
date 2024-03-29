﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachedReferenceDataService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
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

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4WebServices.API.Services.Authorization;

    using NLog;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="CachedReferenceDataService"/> is to provide a caching service for data that is reusable accross
    /// various services during the course of one request
    /// </summary>
    /// <remarks>
    /// The <see cref="CachedReferenceDataService"/> is non-thread safe and should only be used per HTTP reequest
    /// </remarks>
    public class CachedReferenceDataService : ICachedReferenceDataService
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// the (per request) cached <see cref="ParameterType"/>
        /// </summary>
        private Dictionary<Guid, ParameterType> parameterTypeCache;

        /// <summary>
        /// the (per request) cached <see cref="ParameterTypeComponent"/>
        /// </summary>
        private Dictionary<Guid, ParameterTypeComponent> parameterTypeComponentCache;

        /// <summary>
        /// the (per request) cached <see cref="ParameterTypeComponent"/>
        /// </summary>
        private Dictionary<Guid, DependentParameterTypeAssignment> dependentParameterTypeAssignment;

        /// <summary>
        /// the (per request) cached <see cref="ParameterTypeComponent"/>
        /// </summary>
        private Dictionary<Guid, IndependentParameterTypeAssignment> independentParameterTypeAssignment;

        /// <summary>
        /// the (per request) cached <see cref="MeasurementScale"/>
        /// </summary>
        private Dictionary<Guid, MeasurementScale> measurementScaleCache;

        /// <summary>
        /// the (per request) cached <see cref="MeasurementUnit"/>
        /// </summary>
        private Dictionary<Guid, MeasurementUnit> measurementUnitCache;
        
        /// <summary>
        /// Gets or sets the (injected) <see cref="IParameterTypeService"/> used to query the <see cref="ParameterType"/>s from the data-store
        /// </summary>
        public IParameterTypeService ParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IParameterTypeComponentService"/> used to query the <see cref="ParameterTypeComponent"/>s from the data-store
        /// </summary>
        public IParameterTypeComponentService ParameterTypeComponentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDependentParameterTypeAssignmentService" />
        /// </summary>
        public IDependentParameterTypeAssignmentService DependentParameterTypeAssignmentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIndependentParameterTypeAssignmentService" />
        /// </summary>
        public IIndependentParameterTypeAssignmentService IndependentParameterTypeAssignmentService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IMeasurementScaleService"/> used to query the <see cref="MeasurementScale"/>s from the data-store
        /// </summary>
        public IMeasurementScaleService MeasurementScaleService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IMeasurementUnitService"/> used to query the <see cref="MeasurementUnit"/>s from the data-store
        /// </summary>
        public IMeasurementUnitService MeasurementUnitService { get; set; }

        /// <summary>
        /// Queries the <see cref="ParameterType"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{ParameterType}"/>
        /// </returns>
        public Dictionary<Guid, ParameterType> QueryParameterTypes(NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            if (this.parameterTypeCache == null || this.parameterTypeCache.Count == 0)
            {
                var sw = new Stopwatch();

                Logger.Debug("Querying ParameterTypes");

                this.parameterTypeCache = new Dictionary<Guid, ParameterType>();

                var parameterTypes = this.ParameterTypeService
                    .GetShallow(transaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition, null, securityContext)
                    .OfType<ParameterType>();
                
                foreach (var parameterType in parameterTypes)
                {
                    this.parameterTypeCache.Add(parameterType.Iid, parameterType);
                }

                Logger.Debug($"ParameterTypes Queried in {sw.ElapsedMilliseconds} [ms]");
            }

            return this.parameterTypeCache;
        }

        /// <summary>
        /// Queries the <see cref="ParameterTypeComponent"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, ParameterTypeComponent}"/>
        /// </returns>
        public Dictionary<Guid, ParameterTypeComponent> QueryParameterTypeComponents(NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            if (this.parameterTypeComponentCache == null || this.parameterTypeComponentCache.Count == 0)
            {
                var sw = new Stopwatch();

                Logger.Debug("Querying ParameterTypeComponents");

                this.parameterTypeComponentCache = new Dictionary<Guid, ParameterTypeComponent>();

                var parameterTypeComponents = this.ParameterTypeComponentService
                    .GetShallow(transaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition, null, securityContext)
                    .OfType<ParameterTypeComponent>();

                foreach (var parameterTypeComponent in parameterTypeComponents)
                {
                    this.parameterTypeComponentCache.Add(parameterTypeComponent.Iid, parameterTypeComponent);
                }

                Logger.Debug($"ParameterTypeComponents Queried in {sw.ElapsedMilliseconds} [ms]");
            }

            return this.parameterTypeComponentCache;
        }

        /// <summary>
        /// Queries the <see cref="ParameterType"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, ParameterType}"/>
        /// </returns>
        public Dictionary<Guid, DependentParameterTypeAssignment> QueryDependentParameterTypeAssignments(NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            if (this.dependentParameterTypeAssignment == null || this.dependentParameterTypeAssignment.Count == 0)
            {
                var sw = new Stopwatch();

                Logger.Debug("Querying DependentParameterTypeAssignments");

                this.dependentParameterTypeAssignment = new Dictionary<Guid, DependentParameterTypeAssignment>();

                var dependentParameterTypeAssignments = this.DependentParameterTypeAssignmentService
                    .GetShallow(transaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition, null, securityContext)
                    .OfType<DependentParameterTypeAssignment>();

                foreach (var parameterTypeAssignment in dependentParameterTypeAssignments)
                {
                    this.dependentParameterTypeAssignment.Add(parameterTypeAssignment.Iid, parameterTypeAssignment);
                }

                Logger.Debug($"DependentParameterTypeAssignments Queried in {sw.ElapsedMilliseconds} [ms]");
            }

            return this.dependentParameterTypeAssignment;
        }

        /// <summary>
        /// Queries the <see cref="ParameterType"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, ParameterType}"/>
        /// </returns>
        public Dictionary<Guid, IndependentParameterTypeAssignment> QueryIndependentParameterTypeAssignments(NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            if (this.independentParameterTypeAssignment == null || this.independentParameterTypeAssignment.Count == 0)
            {
                var sw = new Stopwatch();

                Logger.Debug("Querying DependentParameterTypeAssignments");

                this.independentParameterTypeAssignment = new Dictionary<Guid, IndependentParameterTypeAssignment>();

                var independentParameterTypeAssignments = this.IndependentParameterTypeAssignmentService
                    .GetShallow(transaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition, null, securityContext)
                    .OfType<IndependentParameterTypeAssignment>();

                foreach (var parameterTypeAssignment in independentParameterTypeAssignments)
                {
                    this.independentParameterTypeAssignment.Add(parameterTypeAssignment.Iid, parameterTypeAssignment);
                }

                Logger.Debug($"IndependentParameterTypeAssignment Queried in {sw.ElapsedMilliseconds} [ms]");
            }

            return this.independentParameterTypeAssignment;
        }

        /// <summary>
        /// Queries the <see cref="MeasurementScale"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, MeasurementScale}"/>
        /// </returns>
        public Dictionary<Guid, MeasurementScale> QueryMeasurementScales(NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            if (this.measurementScaleCache == null || this.measurementScaleCache.Count == 0)
            {
                var sw = new Stopwatch();

                Logger.Debug("Querying MeasurementScales");

                this.measurementScaleCache = new Dictionary<Guid, MeasurementScale>();

                var measurementScales = this.MeasurementScaleService
                    .GetShallow(transaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition, null, securityContext)
                    .OfType<MeasurementScale>();

                foreach (var measurementScale in measurementScales)
                {
                    this.measurementScaleCache.Add(measurementScale.Iid, measurementScale);
                }

                Logger.Debug($"MeasurementScales Queried in {sw.ElapsedMilliseconds} [ms]");
            }

            return this.measurementScaleCache;
        }

        /// <summary>
        /// Queries the <see cref="MeasurementUnit"/>s from the data from the datasource or from the cached list
        /// </summary>
        /// <returns>
        /// An <see cref="Dictionary{Guid, MeasurementUnit}"/>
        /// </returns>
        public Dictionary<Guid, MeasurementUnit> QueryMeasurementUnits(NpgsqlTransaction transaction, ISecurityContext securityContext)
        {
            if (this.measurementUnitCache == null || this.measurementUnitCache.Count == 0)
            {
                var sw = new Stopwatch();

                Logger.Debug("Querying MeasurementUnits");

                this.measurementUnitCache = new Dictionary<Guid, MeasurementUnit>();

                var measurementUnits = this.MeasurementUnitService
                    .GetShallow(transaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition, null, securityContext)
                    .OfType<MeasurementUnit>();

                foreach (var measurementUnit in measurementUnits)
                {
                    this.measurementUnitCache.Add(measurementUnit.Iid, measurementUnit);
                }

                Logger.Debug($"MeasurementUnits Queried in {sw.ElapsedMilliseconds} [ms]");
            }

            return this.measurementUnitCache;
        }

        /// <summary>
        /// Resets (clears) the cached data from the <see cref="ICachedDataService" />.
        /// </summary>
        public void Reset()
        {
            this.parameterTypeCache?.Clear();
            this.parameterTypeComponentCache?.Clear();
            this.dependentParameterTypeAssignment?.Clear();
            this.independentParameterTypeAssignment?.Clear();
            this.measurementScaleCache?.Clear();
            this.measurementUnitCache?.Clear();

            Logger.Debug($"Cache Reset");
        }
    }
}
