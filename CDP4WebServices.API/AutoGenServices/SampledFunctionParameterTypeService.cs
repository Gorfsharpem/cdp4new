// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampledFunctionParameterTypeService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
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
    using System.Linq;
    using System.Security;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using CDP4WebServices.API.Services.Authorization;
    using Npgsql;

    /// <summary>
    /// The <see cref="SampledFunctionParameterType"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class SampledFunctionParameterTypeService : ServiceBase, ISampledFunctionParameterTypeService
    {
        /// <summary>
        /// Gets or sets the <see cref="IAliasService"/>.
        /// </summary>
        public IAliasService AliasService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAttachmentService"/>.
        /// </summary>
        public IAttachmentService AttachmentService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefinitionService"/>.
        /// </summary>
        public IDefinitionService DefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDependentParameterTypeAssignmentService"/>.
        /// </summary>
        public IDependentParameterTypeAssignmentService DependentParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IHyperLinkService"/>.
        /// </summary>
        public IHyperLinkService HyperLinkService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIndependentParameterTypeAssignmentService"/>.
        /// </summary>
        public IIndependentParameterTypeAssignmentService IndependentParameterTypeService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ISampledFunctionParameterTypeDao"/>.
        /// </summary>
        public ISampledFunctionParameterTypeDao SampledFunctionParameterTypeDao { get; set; }

        /// <summary>
        /// Get the requested <see cref="SampledFunctionParameterType"/>s from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="SampledFunctionParameterType"/>, optionally with contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            return this.RequestUtils.QueryParameters.ExtentDeep
                        ? this.GetDeep(transaction, partition, ids, containerSecurityContext)
                        : this.GetShallow(transaction, partition, ids, containerSecurityContext);
        }

        /// <summary>
        /// Add the supplied value to the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be persisted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="SampledFunctionParameterType"/> id that will be the source for each link table record.
        /// </param>
        /// <param name="value">
        /// A value for which a link table record will be created.
        /// </param>
        /// <returns>
        /// True if the link was created.
        /// </returns>
        public bool AddToCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.SampledFunctionParameterTypeDao.AddToCollectionProperty(transaction, partition, propertyName, iid, value);
        }

        /// <summary>
        /// Remove the supplied value from the association property as indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="propertyName">
        /// The association property from where the supplied value will be removed.
        /// </param>
        /// <param name="iid">
        /// The <see cref="SampledFunctionParameterType"/> id that is the source of the link table records.
        /// </param>
        /// <param name="value">
        /// A value for which the link table record will be removed.
        /// </param>
        /// <returns>
        /// True if the link was removed.
        /// </returns>
        public bool DeleteFromCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, object value)
        {
            return this.SampledFunctionParameterTypeDao.DeleteFromCollectionProperty(transaction, partition, propertyName, iid, value);
        }

        /// <summary>
        /// Reorder the supplied value collection of the association link table indicated by the supplied property name.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="propertyName">
        /// The association property name that will be reordered.
        /// </param>
        /// <param name="iid">
        /// The <see cref="SampledFunctionParameterType"/> id that is the source for the reordered link table record.
        /// </param>
        /// <param name="orderUpdate">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the link was created.
        /// </returns>
        public bool ReorderCollectionProperty(NpgsqlTransaction transaction, string partition, string propertyName, Guid iid, CDP4Common.Types.OrderedItem orderUpdate)
        {
            return this.SampledFunctionParameterTypeDao.ReorderCollectionProperty(transaction, partition, propertyName, iid, orderUpdate);
        }

        /// <summary>
        /// Update the containment order as indicated by the supplied orderedItem.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource order will be updated.
        /// </param>
        /// <param name="orderedItem">
        /// The order update information containing the new order key.
        /// </param>
        /// <returns>
        /// True if the contained item was successfully reordered.
        /// </returns>
        public bool ReorderContainment(NpgsqlTransaction transaction, string partition, CDP4Common.Types.OrderedItem orderedItem)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Delete the supplied <see cref="SampledFunctionParameterType"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SampledFunctionParameterType"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SampledFunctionParameterType"/> to be removed.
        /// </param>
        /// <returns>
        /// True if the removal was successful.
        /// </returns>
        public bool DeleteConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, DeleteOperation))
            {
                throw new SecurityException("The person " + this.PermissionService.Credentials.Person.UserName + " does not have an appropriate delete permission for " + thing.GetType().Name + ".");
            }

            return this.SampledFunctionParameterTypeDao.Delete(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Delete the supplied <see cref="SampledFunctionParameterType"/> instance.
        /// A "Raw" Delete means that the delete is performed without calling before-, or after actions, or other side effects.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be removed.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SampledFunctionParameterType"/> to delete.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SampledFunctionParameterType"/> to be removed.
        /// </param>
        /// <returns>
        /// True if the removal was successful.
        /// </returns>
        public bool RawDeleteConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container = null)
        {

            return this.SampledFunctionParameterTypeDao.RawDelete(transaction, partition, thing.Iid);
        }

        /// <summary>
        /// Update the supplied <see cref="SampledFunctionParameterType"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SampledFunctionParameterType"/> <see cref="Thing"/> to update.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SampledFunctionParameterType"/> to be updated.
        /// </param>
        /// <returns>
        /// True if the update was successful.
        /// </returns>
        public bool UpdateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, UpdateOperation))
            {
                throw new SecurityException("The person " + this.PermissionService.Credentials.Person.UserName + " does not have an appropriate update permission for " + thing.GetType().Name + ".");
            }

            var sampledFunctionParameterType = thing as SampledFunctionParameterType;
            return this.SampledFunctionParameterTypeDao.Update(transaction, partition, sampledFunctionParameterType, container);
        }

        /// <summary>
        /// Persist the supplied <see cref="SampledFunctionParameterType"/> instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SampledFunctionParameterType"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SampledFunctionParameterType"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        public bool CreateConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            if (!this.IsInstanceModifyAllowed(transaction, thing, partition, CreateOperation))
            {
                throw new SecurityException("The person " + this.PermissionService.Credentials.Person.UserName + " does not have an appropriate create permission for " + thing.GetType().Name + ".");
            }

            var sampledFunctionParameterType = thing as SampledFunctionParameterType;
            var createSuccesful = this.SampledFunctionParameterTypeDao.Write(transaction, partition, sampledFunctionParameterType, container);
            return createSuccesful && this.CreateContainment(transaction, partition, sampledFunctionParameterType);
        }

        /// <summary>
        /// Persist the supplied <see cref="SampledFunctionParameterType"/> instance. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="SampledFunctionParameterType"/> <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="SampledFunctionParameterType"/> to be persisted.
        /// </param>
        /// <param name="sequence">
        /// The order sequence used to persist this instance. Default is not used (-1).
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        public bool UpsertConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1)
        {
            var sampledFunctionParameterType = thing as SampledFunctionParameterType;
            var createSuccesful = this.SampledFunctionParameterTypeDao.Upsert(transaction, partition, sampledFunctionParameterType, container);
            return createSuccesful && this.UpsertContainment(transaction, partition, sampledFunctionParameterType);
        }

        /// <summary>
        /// Get the requested data from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="SampledFunctionParameterType"/>.
        /// </returns>
        public IEnumerable<Thing> GetShallow(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            var authorizedContext = this.AuthorizeReadRequest("SampledFunctionParameterType", containerSecurityContext, partition);
            var isAllowed = authorizedContext.ContainerReadAllowed && this.BeforeGet(transaction, partition, idFilter);
            if (!isAllowed || (idFilter != null && !idFilter.Any()))
            {
                return Enumerable.Empty<Thing>();
            }

            var sampledFunctionParameterTypeColl = new List<Thing>(this.SampledFunctionParameterTypeDao.Read(transaction, partition, idFilter, this.TransactionManager.IsCachedDtoReadEnabled(transaction)));

            return this.AfterGet(sampledFunctionParameterTypeColl, transaction, partition, idFilter);
        }

        /// <summary>
        /// Get the requested data from the ORM layer by chaining the containment properties.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="SampledFunctionParameterType"/> and contained <see cref="Thing"/>s.
        /// </returns>
        public IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext)
        {
            var idFilter = ids == null ? null : ids.ToArray();
            if (idFilter != null && !idFilter.Any())
            {
                return Enumerable.Empty<Thing>();
            }

            var results = new List<Thing>(this.GetShallow(transaction, partition, idFilter, containerSecurityContext));
            var sampledFunctionParameterTypeColl = results.Where(i => i.GetType() == typeof(SampledFunctionParameterType)).Cast<SampledFunctionParameterType>().ToList();

            results.AddRange(this.AliasService.GetDeep(transaction, partition, sampledFunctionParameterTypeColl.SelectMany(x => x.Alias), containerSecurityContext));
            results.AddRange(this.AttachmentService.GetDeep(transaction, partition, sampledFunctionParameterTypeColl.SelectMany(x => x.Attachment), containerSecurityContext));
            results.AddRange(this.DefinitionService.GetDeep(transaction, partition, sampledFunctionParameterTypeColl.SelectMany(x => x.Definition), containerSecurityContext));
            results.AddRange(this.DependentParameterTypeService.GetDeep(transaction, partition, sampledFunctionParameterTypeColl.SelectMany(x => x.DependentParameterType).ToIdList(), containerSecurityContext));
            results.AddRange(this.HyperLinkService.GetDeep(transaction, partition, sampledFunctionParameterTypeColl.SelectMany(x => x.HyperLink), containerSecurityContext));
            results.AddRange(this.IndependentParameterTypeService.GetDeep(transaction, partition, sampledFunctionParameterTypeColl.SelectMany(x => x.IndependentParameterType).ToIdList(), containerSecurityContext));

            return results;
        }

        /// <summary>
        /// Execute additional logic after each GET function call.
        /// </summary>
        /// <param name="resultCollection">
        /// An instance collection that was retrieved from the persistence layer.
        /// </param>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from which the requested resource is to be retrieved.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="includeReferenceData">
        /// Control flag to indicate if reference library data should be retrieved extent=deep or extent=shallow.
        /// </param>
        /// <returns>
        /// A post filtered instance of the passed in resultCollection.
        /// </returns>
        public override IEnumerable<Thing> AfterGet(IEnumerable<Thing> resultCollection, NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, bool includeReferenceData = false)
        {
            var filteredCollection = new List<Thing>();
            foreach (var thing in resultCollection)
            {
                if (this.IsInstanceReadAllowed(transaction, thing, partition))
                {
                    filteredCollection.Add(thing);
                }
                else
                {
                    Logger.Trace("The person {0} does not have a read permission for {1}.", this.PermissionService.Credentials.Person.UserName, thing.GetType().Name);
                }
            }

            return filteredCollection;
        }

        /// <summary>
        /// Persist the <see cref="SampledFunctionParameterType"/> containment tree to the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="sampledFunctionParameterType">
        /// The <see cref="SampledFunctionParameterType"/> instance to persist.
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        private bool CreateContainment(NpgsqlTransaction transaction, string partition, SampledFunctionParameterType sampledFunctionParameterType)
        {
            var results = new List<bool>();

            foreach (var alias in this.ResolveFromRequestCache(sampledFunctionParameterType.Alias))
            {
                results.Add(this.AliasService.CreateConcept(transaction, partition, alias, sampledFunctionParameterType));
            }

            foreach (var attachment in this.ResolveFromRequestCache(sampledFunctionParameterType.Attachment))
            {
                results.Add(this.AttachmentService.CreateConcept(transaction, partition, attachment, sampledFunctionParameterType));
            }

            foreach (var definition in this.ResolveFromRequestCache(sampledFunctionParameterType.Definition))
            {
                results.Add(this.DefinitionService.CreateConcept(transaction, partition, definition, sampledFunctionParameterType));
            }

            foreach (var dependentParameterType in this.ResolveFromRequestCache(sampledFunctionParameterType.DependentParameterType))
            {
                results.Add(this.DependentParameterTypeService.CreateConcept(transaction, partition, (DependentParameterTypeAssignment)dependentParameterType.V, sampledFunctionParameterType, dependentParameterType.K));
            }

            foreach (var hyperLink in this.ResolveFromRequestCache(sampledFunctionParameterType.HyperLink))
            {
                results.Add(this.HyperLinkService.CreateConcept(transaction, partition, hyperLink, sampledFunctionParameterType));
            }

            foreach (var independentParameterType in this.ResolveFromRequestCache(sampledFunctionParameterType.IndependentParameterType))
            {
                results.Add(this.IndependentParameterTypeService.CreateConcept(transaction, partition, (IndependentParameterTypeAssignment)independentParameterType.V, sampledFunctionParameterType, independentParameterType.K));
            }

            return results.All(x => x);
        }
                
        /// <summary>
        /// Persist the <see cref="SampledFunctionParameterType"/> containment tree to the ORM layer. Update if it already exists.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="sampledFunctionParameterType">
        /// The <see cref="SampledFunctionParameterType"/> instance to persist.
        /// </param>
        /// <returns>
        /// True if the persistence was successful.
        /// </returns>
        private bool UpsertContainment(NpgsqlTransaction transaction, string partition, SampledFunctionParameterType sampledFunctionParameterType)
        {
            var results = new List<bool>();

            foreach (var alias in this.ResolveFromRequestCache(sampledFunctionParameterType.Alias))
            {
                results.Add(this.AliasService.UpsertConcept(transaction, partition, alias, sampledFunctionParameterType));
            }

            foreach (var attachment in this.ResolveFromRequestCache(sampledFunctionParameterType.Attachment))
            {
                results.Add(this.AttachmentService.UpsertConcept(transaction, partition, attachment, sampledFunctionParameterType));
            }

            foreach (var definition in this.ResolveFromRequestCache(sampledFunctionParameterType.Definition))
            {
                results.Add(this.DefinitionService.UpsertConcept(transaction, partition, definition, sampledFunctionParameterType));
            }

            foreach (var dependentParameterType in this.ResolveFromRequestCache(sampledFunctionParameterType.DependentParameterType))
            {
                results.Add(this.DependentParameterTypeService.UpsertConcept(transaction, partition, (DependentParameterTypeAssignment)dependentParameterType.V, sampledFunctionParameterType, dependentParameterType.K));
            }

            foreach (var hyperLink in this.ResolveFromRequestCache(sampledFunctionParameterType.HyperLink))
            {
                results.Add(this.HyperLinkService.UpsertConcept(transaction, partition, hyperLink, sampledFunctionParameterType));
            }

            foreach (var independentParameterType in this.ResolveFromRequestCache(sampledFunctionParameterType.IndependentParameterType))
            {
                results.Add(this.IndependentParameterTypeService.UpsertConcept(transaction, partition, (IndependentParameterTypeAssignment)independentParameterType.V, sampledFunctionParameterType, independentParameterType.K));
            }

            return results.All(x => x);
        }
    }
}
