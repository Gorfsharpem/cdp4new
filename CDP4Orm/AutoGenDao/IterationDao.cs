// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationDao.cs" company="RHEA System S.A.">
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

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    using Npgsql;
    using NpgsqlTypes;

    /// <summary>
    /// The Iteration Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class IterationDao : ThingDao, IIterationDao
    {
        /// <summary>
        /// Read the data from the database.
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
        /// <param name="isCachedDtoReadEnabledAndInstant">
        /// The value indicating whether to get cached last state of Dto from revision history.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.Iteration"/>.
        /// </returns>
        public virtual IEnumerable<CDP4Common.DTO.Iteration> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids = null, bool isCachedDtoReadEnabledAndInstant = false)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                if (isCachedDtoReadEnabledAndInstant)
                {
                    sqlBuilder.AppendFormat("SELECT \"Jsonb\" FROM \"{0}\".\"Iteration_Cache\"", partition);

                    if (ids != null && ids.Any())
                    {
                        sqlBuilder.Append(" WHERE \"Iid\" = ANY(:ids)");
                        command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids;
                    }

                    sqlBuilder.Append(";");

                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    command.CommandText = sqlBuilder.ToString();

                    // log the sql command 
                    this.LogCommand(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var thing = this.MapJsonbToDto(reader);
                            if (thing != null)
                            {
                                yield return thing as Iteration;
                            }
                        }
                    }
                }
                else
                {
                    sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"Iteration_View\"", partition);

                    if (ids != null && ids.Any()) 
                    {
                        sqlBuilder.Append(" WHERE \"Iid\" = ANY(:ids)");
                        command.Parameters.Add("ids", NpgsqlDbType.Array | NpgsqlDbType.Uuid).Value = ids;
                    }

                    sqlBuilder.Append(";");

                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;
                    command.CommandText = sqlBuilder.ToString();

                    // log the sql command 
                    this.LogCommand(command);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return this.MapToDto(reader);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The mapping from a database record to data transfer object.
        /// </summary>
        /// <param name="reader">
        /// An instance of the SQL reader.
        /// </param>
        /// <returns>
        /// A deserialized instance of <see cref="CDP4Common.DTO.Iteration"/>.
        /// </returns>
        public virtual CDP4Common.DTO.Iteration MapToDto(NpgsqlDataReader reader)
        {
            string tempModifiedOn;
            string tempSourceIterationIid;
            string tempThingPreference;

            var valueDict = (Dictionary<string, string>)reader["ValueTypeSet"];
            var iid = Guid.Parse(reader["Iid"].ToString());
            var revisionNumber = int.Parse(valueDict["RevisionNumber"]);

            var dto = new CDP4Common.DTO.Iteration(iid, revisionNumber);
            dto.ActualFiniteStateList.AddRange(Array.ConvertAll((string[])reader["ActualFiniteStateList"], Guid.Parse));
            dto.DefaultOption = reader["DefaultOption"] is DBNull ? (Guid?)null : Guid.Parse(reader["DefaultOption"].ToString());
            dto.DiagramCanvas.AddRange(Array.ConvertAll((string[])reader["DiagramCanvas"], Guid.Parse));
            dto.DomainFileStore.AddRange(Array.ConvertAll((string[])reader["DomainFileStore"], Guid.Parse));
            dto.Element.AddRange(Array.ConvertAll((string[])reader["Element"], Guid.Parse));
            dto.ExcludedDomain.AddRange(Array.ConvertAll((string[])reader["ExcludedDomain"], Guid.Parse));
            dto.ExcludedPerson.AddRange(Array.ConvertAll((string[])reader["ExcludedPerson"], Guid.Parse));
            dto.ExternalIdentifierMap.AddRange(Array.ConvertAll((string[])reader["ExternalIdentifierMap"], Guid.Parse));
            dto.Goal.AddRange(Array.ConvertAll((string[])reader["Goal"], Guid.Parse));
            dto.IterationSetup = Guid.Parse(reader["IterationSetup"].ToString());
            dto.Option.AddRange(Utils.ParseOrderedList<Guid>(reader["Option"] as string[,]));
            dto.PossibleFiniteStateList.AddRange(Array.ConvertAll((string[])reader["PossibleFiniteStateList"], Guid.Parse));
            dto.Publication.AddRange(Array.ConvertAll((string[])reader["Publication"], Guid.Parse));
            dto.Relationship.AddRange(Array.ConvertAll((string[])reader["Relationship"], Guid.Parse));
            dto.RequirementsSpecification.AddRange(Array.ConvertAll((string[])reader["RequirementsSpecification"], Guid.Parse));
            dto.RuleVerificationList.AddRange(Array.ConvertAll((string[])reader["RuleVerificationList"], Guid.Parse));
            dto.SharedDiagramStyle.AddRange(Array.ConvertAll((string[])reader["SharedDiagramStyle"], Guid.Parse));
            dto.Stakeholder.AddRange(Array.ConvertAll((string[])reader["Stakeholder"], Guid.Parse));
            dto.StakeholderValue.AddRange(Array.ConvertAll((string[])reader["StakeholderValue"], Guid.Parse));
            dto.StakeholderValueMap.AddRange(Array.ConvertAll((string[])reader["StakeholderValueMap"], Guid.Parse));
            dto.TopElement = reader["TopElement"] is DBNull ? (Guid?)null : Guid.Parse(reader["TopElement"].ToString());
            dto.ValueGroup.AddRange(Array.ConvertAll((string[])reader["ValueGroup"], Guid.Parse));

            if (valueDict.TryGetValue("ModifiedOn", out tempModifiedOn))
            {
                dto.ModifiedOn = Utils.ParseUtcDate(tempModifiedOn);
            }

            if (valueDict.TryGetValue("SourceIterationIid", out tempSourceIterationIid) && tempSourceIterationIid != null)
            {
                dto.SourceIterationIid = Guid.Parse(tempSourceIterationIid);
            }

            if (valueDict.TryGetValue("ThingPreference", out tempThingPreference) && tempThingPreference != null)
            {
                dto.ThingPreference = tempThingPreference.UnEscape();
            }

            return dto;
        }

        /// <summary>
        /// Insert a new database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iteration">
        /// The iteration DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Write(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Iteration iteration, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeWrite = this.BeforeWrite(transaction, partition, iteration, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeWrite = beforeWrite && base.Write(transaction, partition, iteration, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "SourceIterationIid", !this.IsDerived(iteration, "SourceIterationIid") && iteration.SourceIterationIid.HasValue ? iteration.SourceIterationIid.Value.ToString() : null },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    
                    sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Iteration\"", partition);
                    sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\")");
                    sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :defaultOption, :iterationSetup, :topElement);");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iteration.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("defaultOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "DefaultOption") ? Utils.NullableValue(iteration.DefaultOption) : Utils.NullableValue(null);
                    command.Parameters.Add("iterationSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "IterationSetup") ? iteration.IterationSetup : Utils.NullableValue(null);
                    command.Parameters.Add("topElement", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "TopElement") ? Utils.NullableValue(iteration.TopElement) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterWrite(beforeWrite, transaction, partition, iteration, container);
        }

        /// <summary>
        /// Insert a new database record, or updates one if it already exists from the supplied data transfer object.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="iteration">
        /// The iteration DTO that is to be persisted.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be persisted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully persisted.
        /// </returns>
        public virtual bool Upsert(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Iteration iteration, CDP4Common.DTO.Thing container = null)
        {
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            base.Upsert(transaction, partition, iteration, container);

            var valueTypeDictionaryContents = new Dictionary<string, string>
            {
                { "SourceIterationIid", !this.IsDerived(iteration, "SourceIterationIid") && iteration.SourceIterationIid.HasValue ? iteration.SourceIterationIid.Value.ToString() : null },
            }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                    
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Iteration\"", partition);
                sqlBuilder.AppendFormat(" (\"Iid\", \"ValueTypeDictionary\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\")");
                sqlBuilder.AppendFormat(" VALUES (:iid, :valueTypeDictionary, :container, :defaultOption, :iterationSetup, :topElement)");

                command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iteration.Iid;
                command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                command.Parameters.Add("defaultOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "DefaultOption") ? Utils.NullableValue(iteration.DefaultOption) : Utils.NullableValue(null);
                command.Parameters.Add("iterationSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "IterationSetup") ? iteration.IterationSetup : Utils.NullableValue(null);
                command.Parameters.Add("topElement", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "TopElement") ? Utils.NullableValue(iteration.TopElement) : Utils.NullableValue(null);
                sqlBuilder.Append(" ON CONFLICT (\"Iid\")");
                sqlBuilder.Append(" DO UPDATE ");
                sqlBuilder.Append(" SET (\"ValueTypeDictionary\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\")");
                sqlBuilder.Append(" = (:valueTypeDictionary, :container, :defaultOption, :iterationSetup, :topElement);");

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }

            return true;
        }

        /// <summary>
        /// Update a database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="iteration">
        /// The Iteration DTO that is to be updated.
        /// </param>
        /// <param name="container">
        /// The container of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the concept was successfully updated.
        /// </returns>
        public virtual bool Update(NpgsqlTransaction transaction, string partition, CDP4Common.DTO.Iteration iteration, CDP4Common.DTO.Thing container = null)
        {
            bool isHandled;
            var valueTypeDictionaryAdditions = new Dictionary<string, string>();
            var beforeUpdate = this.BeforeUpdate(transaction, partition, iteration, container, out isHandled, valueTypeDictionaryAdditions);
            if (!isHandled)
            {
                beforeUpdate = beforeUpdate && base.Update(transaction, partition, iteration, container);

                var valueTypeDictionaryContents = new Dictionary<string, string>
                {
                    { "SourceIterationIid", !this.IsDerived(iteration, "SourceIterationIid") && iteration.SourceIterationIid.HasValue ? iteration.SourceIterationIid.Value.ToString() : null },
                }.Concat(valueTypeDictionaryAdditions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                using (var command = new NpgsqlCommand())
                {
                    var sqlBuilder = new System.Text.StringBuilder();
                    sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Iteration\"", partition);
                    sqlBuilder.AppendFormat(" SET (\"ValueTypeDictionary\", \"Container\", \"DefaultOption\", \"IterationSetup\", \"TopElement\")");
                    sqlBuilder.AppendFormat(" = (:valueTypeDictionary, :container, :defaultOption, :iterationSetup, :topElement)");
                    sqlBuilder.AppendFormat(" WHERE \"Iid\" = :iid;");

                    command.Parameters.Add("iid", NpgsqlDbType.Uuid).Value = iteration.Iid;
                    command.Parameters.Add("valueTypeDictionary", NpgsqlDbType.Hstore).Value = valueTypeDictionaryContents;
                    command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = container.Iid;
                    command.Parameters.Add("defaultOption", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "DefaultOption") ? Utils.NullableValue(iteration.DefaultOption) : Utils.NullableValue(null);
                    command.Parameters.Add("iterationSetup", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "IterationSetup") ? iteration.IterationSetup : Utils.NullableValue(null);
                    command.Parameters.Add("topElement", NpgsqlDbType.Uuid).Value = !this.IsDerived(iteration, "TopElement") ? Utils.NullableValue(iteration.TopElement) : Utils.NullableValue(null);

                    command.CommandText = sqlBuilder.ToString();
                    command.Connection = transaction.Connection;
                    command.Transaction = transaction;

                    this.ExecuteAndLogCommand(command);
                }
            }

            return this.AfterUpdate(beforeUpdate, transaction, partition, iteration, container);
        }

        /// <summary>
        /// Delete a database record from the supplied data transfer object.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Iteration"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully deleted.
        /// </returns>
        public override bool Delete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            bool isHandled;
            var beforeDelete = this.BeforeDelete(transaction, partition, iid, out isHandled);
            if (!isHandled)
            {
                beforeDelete = beforeDelete && base.Delete(transaction, partition, iid);
            }

            return this.AfterDelete(beforeDelete, transaction, partition, iid);
        }

        /// <summary>
        /// Delete a database record from the supplied data transfer object.
        /// A "Raw" Delete means that the delete is performed without calling BeforeDelete or AfterDelete.
        /// This is typically used during the import of existing data to the Database.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The <see cref="CDP4Common.DTO.Iteration"/> id that is to be deleted.
        /// </param>
        /// <returns>
        /// True if the concept was successfully deleted.
        /// </returns>
        public override bool RawDelete(NpgsqlTransaction transaction, string partition, Guid iid)
        {
            var result = false;

            result = base.Delete(transaction, partition, iid);
            return result;
        }

        /// <summary>
        /// Copy the tables from a source to an Iteration partition
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="targetPartition">
        /// The target iteration partition
        /// </param>
        public void CopyIteration(NpgsqlTransaction transaction, string sourcePartition, string targetPartition)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteState\" SELECT * FROM \"{1}\".\"ActualFiniteState\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteState\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList\" SELECT * FROM \"{1}\".\"ActualFiniteStateList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Alias\" SELECT * FROM \"{1}\".\"Alias\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Alias\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"AndExpression\" SELECT * FROM \"{1}\".\"AndExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"AndExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BinaryRelationship\" SELECT * FROM \"{1}\".\"BinaryRelationship\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BinaryRelationship\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BooleanExpression\" SELECT * FROM \"{1}\".\"BooleanExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BooleanExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Bounds\" SELECT * FROM \"{1}\".\"Bounds\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Bounds\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"BuiltInRuleVerification\" SELECT * FROM \"{1}\".\"BuiltInRuleVerification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"BuiltInRuleVerification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Citation\" SELECT * FROM \"{1}\".\"Citation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Citation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Color\" SELECT * FROM \"{1}\".\"Color\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Color\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DefinedThing\" SELECT * FROM \"{1}\".\"DefinedThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DefinedThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Definition\" SELECT * FROM \"{1}\".\"Definition\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Definition\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramCanvas\" SELECT * FROM \"{1}\".\"DiagramCanvas\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramCanvas\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramEdge\" SELECT * FROM \"{1}\".\"DiagramEdge\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramEdge\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramElementContainer\" SELECT * FROM \"{1}\".\"DiagramElementContainer\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramElementContainer\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramElementThing\" SELECT * FROM \"{1}\".\"DiagramElementThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramElementThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagrammingStyle\" SELECT * FROM \"{1}\".\"DiagrammingStyle\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagrammingStyle\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramObject\" SELECT * FROM \"{1}\".\"DiagramObject\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramObject\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramShape\" SELECT * FROM \"{1}\".\"DiagramShape\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramShape\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DiagramThingBase\" SELECT * FROM \"{1}\".\"DiagramThingBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DiagramThingBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"DomainFileStore\" SELECT * FROM \"{1}\".\"DomainFileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"DomainFileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementBase\" SELECT * FROM \"{1}\".\"ElementBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementDefinition\" SELECT * FROM \"{1}\".\"ElementDefinition\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementDefinition\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementUsage\" SELECT * FROM \"{1}\".\"ElementUsage\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementUsage\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ExclusiveOrExpression\" SELECT * FROM \"{1}\".\"ExclusiveOrExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ExclusiveOrExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ExternalIdentifierMap\" SELECT * FROM \"{1}\".\"ExternalIdentifierMap\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ExternalIdentifierMap\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File\" SELECT * FROM \"{1}\".\"File\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision\" SELECT * FROM \"{1}\".\"FileRevision\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileStore\" SELECT * FROM \"{1}\".\"FileStore\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileStore\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Folder\" SELECT * FROM \"{1}\".\"Folder\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Folder\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Goal\" SELECT * FROM \"{1}\".\"Goal\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Goal\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"HyperLink\" SELECT * FROM \"{1}\".\"HyperLink\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"HyperLink\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"IdCorrespondence\" SELECT * FROM \"{1}\".\"IdCorrespondence\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"IdCorrespondence\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"MultiRelationship\" SELECT * FROM \"{1}\".\"MultiRelationship\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MultiRelationship\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NestedElement\" SELECT * FROM \"{1}\".\"NestedElement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NestedElement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NestedParameter\" SELECT * FROM \"{1}\".\"NestedParameter\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NestedParameter\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NotExpression\" SELECT * FROM \"{1}\".\"NotExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NotExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Option\" SELECT * FROM \"{1}\".\"Option\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Option\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"OrExpression\" SELECT * FROM \"{1}\".\"OrExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"OrExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"OwnedStyle\" SELECT * FROM \"{1}\".\"OwnedStyle\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"OwnedStyle\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Parameter\" SELECT * FROM \"{1}\".\"Parameter\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Parameter\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterBase\" SELECT * FROM \"{1}\".\"ParameterBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterGroup\" SELECT * FROM \"{1}\".\"ParameterGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterOrOverrideBase\" SELECT * FROM \"{1}\".\"ParameterOrOverrideBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterOrOverrideBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterOverride\" SELECT * FROM \"{1}\".\"ParameterOverride\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterOverride\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterOverrideValueSet\" SELECT * FROM \"{1}\".\"ParameterOverrideValueSet\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterOverrideValueSet\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterSubscription\" SELECT * FROM \"{1}\".\"ParameterSubscription\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterSubscription\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterSubscriptionValueSet\" SELECT * FROM \"{1}\".\"ParameterSubscriptionValueSet\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterSubscriptionValueSet\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValue\" SELECT * FROM \"{1}\".\"ParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValueSet\" SELECT * FROM \"{1}\".\"ParameterValueSet\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValueSet\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParameterValueSetBase\" SELECT * FROM \"{1}\".\"ParameterValueSetBase\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParameterValueSetBase\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ParametricConstraint\" SELECT * FROM \"{1}\".\"ParametricConstraint\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ParametricConstraint\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Point\" SELECT * FROM \"{1}\".\"Point\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Point\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteState\" SELECT * FROM \"{1}\".\"PossibleFiniteState\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteState\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteStateList\" SELECT * FROM \"{1}\".\"PossibleFiniteStateList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteStateList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication\" SELECT * FROM \"{1}\".\"Publication\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Publication\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RelationalExpression\" SELECT * FROM \"{1}\".\"RelationalExpression\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RelationalExpression\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Relationship\" SELECT * FROM \"{1}\".\"Relationship\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Relationship\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RelationshipParameterValue\" SELECT * FROM \"{1}\".\"RelationshipParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RelationshipParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Requirement\" SELECT * FROM \"{1}\".\"Requirement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Requirement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsContainer\" SELECT * FROM \"{1}\".\"RequirementsContainer\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsContainer\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsContainerParameterValue\" SELECT * FROM \"{1}\".\"RequirementsContainerParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsContainerParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsGroup\" SELECT * FROM \"{1}\".\"RequirementsGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsSpecification\" SELECT * FROM \"{1}\".\"RequirementsSpecification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsSpecification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RuleVerification\" SELECT * FROM \"{1}\".\"RuleVerification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleVerification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RuleVerificationList\" SELECT * FROM \"{1}\".\"RuleVerificationList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleVerificationList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RuleViolation\" SELECT * FROM \"{1}\".\"RuleViolation\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RuleViolation\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SharedStyle\" SELECT * FROM \"{1}\".\"SharedStyle\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SharedStyle\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SimpleParameterizableThing\" SELECT * FROM \"{1}\".\"SimpleParameterizableThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SimpleParameterizableThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"SimpleParameterValue\" SELECT * FROM \"{1}\".\"SimpleParameterValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"SimpleParameterValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Stakeholder\" SELECT * FROM \"{1}\".\"Stakeholder\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Stakeholder\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeholderValue\" SELECT * FROM \"{1}\".\"StakeholderValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeholderValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap\" SELECT * FROM \"{1}\".\"StakeHolderValueMap\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMapSettings\" SELECT * FROM \"{1}\".\"StakeHolderValueMapSettings\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMapSettings\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing\" SELECT * FROM \"{1}\".\"Thing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"UserRuleVerification\" SELECT * FROM \"{1}\".\"UserRuleVerification\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"UserRuleVerification\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ValueGroup\" SELECT * FROM \"{1}\".\"ValueGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ValueGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteState_PossibleState\" SELECT * FROM \"{1}\".\"ActualFiniteState_PossibleState\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteState_PossibleState\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList_ExcludeOption\" SELECT * FROM \"{1}\".\"ActualFiniteStateList_ExcludeOption\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList_ExcludeOption\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\" SELECT * FROM \"{1}\".\"ActualFiniteStateList_PossibleFiniteStateList\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"AndExpression_Term\" SELECT * FROM \"{1}\".\"AndExpression_Term\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"AndExpression_Term\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementBase_Category\" SELECT * FROM \"{1}\".\"ElementBase_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementBase_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementDefinition_OrganizationalParticipant\" SELECT * FROM \"{1}\".\"ElementDefinition_OrganizationalParticipant\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementDefinition_OrganizationalParticipant\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementDefinition_ReferencedElement\" SELECT * FROM \"{1}\".\"ElementDefinition_ReferencedElement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementDefinition_ReferencedElement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ElementUsage_ExcludeOption\" SELECT * FROM \"{1}\".\"ElementUsage_ExcludeOption\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ElementUsage_ExcludeOption\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ExclusiveOrExpression_Term\" SELECT * FROM \"{1}\".\"ExclusiveOrExpression_Term\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ExclusiveOrExpression_Term\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"File_Category\" SELECT * FROM \"{1}\".\"File_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"File_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"FileRevision_FileType\" SELECT * FROM \"{1}\".\"FileRevision_FileType\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"FileRevision_FileType\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Goal_Category\" SELECT * FROM \"{1}\".\"Goal_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Goal_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"MultiRelationship_RelatedThing\" SELECT * FROM \"{1}\".\"MultiRelationship_RelatedThing\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"MultiRelationship_RelatedThing\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"NestedElement_ElementUsage\" SELECT * FROM \"{1}\".\"NestedElement_ElementUsage\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"NestedElement_ElementUsage\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Option_Category\" SELECT * FROM \"{1}\".\"Option_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Option_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"OrExpression_Term\" SELECT * FROM \"{1}\".\"OrExpression_Term\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"OrExpression_Term\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"PossibleFiniteStateList_Category\" SELECT * FROM \"{1}\".\"PossibleFiniteStateList_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"PossibleFiniteStateList_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication_Domain\" SELECT * FROM \"{1}\".\"Publication_Domain\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Publication_Domain\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Publication_PublishedParameter\" SELECT * FROM \"{1}\".\"Publication_PublishedParameter\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Publication_PublishedParameter\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Relationship_Category\" SELECT * FROM \"{1}\".\"Relationship_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Relationship_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Requirement_Category\" SELECT * FROM \"{1}\".\"Requirement_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Requirement_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"RequirementsContainer_Category\" SELECT * FROM \"{1}\".\"RequirementsContainer_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"RequirementsContainer_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Stakeholder_Category\" SELECT * FROM \"{1}\".\"Stakeholder_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Stakeholder_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Stakeholder_StakeholderValue\" SELECT * FROM \"{1}\".\"Stakeholder_StakeholderValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Stakeholder_StakeholderValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeholderValue_Category\" SELECT * FROM \"{1}\".\"StakeholderValue_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeholderValue_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_Category\" SELECT * FROM \"{1}\".\"StakeHolderValueMap_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_Goal\" SELECT * FROM \"{1}\".\"StakeHolderValueMap_Goal\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_Goal\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_Requirement\" SELECT * FROM \"{1}\".\"StakeHolderValueMap_Requirement\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_Requirement\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_StakeholderValue\" SELECT * FROM \"{1}\".\"StakeHolderValueMap_StakeholderValue\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_StakeholderValue\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"StakeHolderValueMap_ValueGroup\" SELECT * FROM \"{1}\".\"StakeHolderValueMap_ValueGroup\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"StakeHolderValueMap_ValueGroup\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedDomain\" SELECT * FROM \"{1}\".\"Thing_ExcludedDomain\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedDomain\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"Thing_ExcludedPerson\" SELECT * FROM \"{1}\".\"Thing_ExcludedPerson\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"Thing_ExcludedPerson\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);
                sqlBuilder.AppendFormat("INSERT INTO \"{0}\".\"ValueGroup_Category\" SELECT * FROM \"{1}\".\"ValueGroup_Category\";", targetPartition, sourcePartition);
                sqlBuilder.AppendFormat("UPDATE \"{0}\".\"ValueGroup_Category\" SET \"ValidFrom\" = \"SiteDirectory\".get_transaction_time(), \"ValidTo\" = 'infinity';", targetPartition);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }
        }

        /// <summary>
        /// Copy the tables from a source to an Iteration partition
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="enable">
        /// A value indicating whether the user trigger shall be enabled
        /// </param>
        public void ModifyUserTrigger(NpgsqlTransaction transaction, string sourcePartition, bool enable)
        {
            var triggerStatus = enable ? "ENABLE" : "DISABLE";
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActualFiniteState\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActualFiniteStateList\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Alias\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"AndExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"BinaryRelationship\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"BooleanExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Bounds\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"BuiltInRuleVerification\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Citation\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Color\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DefinedThing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Definition\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramCanvas\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramEdge\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramElementContainer\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramElementThing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagrammingStyle\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramObject\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramShape\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DiagramThingBase\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"DomainFileStore\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementBase\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementDefinition\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementUsage\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ExclusiveOrExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ExternalIdentifierMap\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"File\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"FileRevision\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"FileStore\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Folder\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Goal\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"HyperLink\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"IdCorrespondence\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"MultiRelationship\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"NestedElement\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"NestedParameter\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"NotExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Option\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"OrExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"OwnedStyle\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Parameter\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterBase\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterGroup\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterOrOverrideBase\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterOverride\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterOverrideValueSet\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterSubscription\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterSubscriptionValueSet\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterValueSet\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParameterValueSetBase\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ParametricConstraint\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Point\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"PossibleFiniteState\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"PossibleFiniteStateList\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Publication\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RelationalExpression\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Relationship\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RelationshipParameterValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Requirement\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequirementsContainer\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequirementsContainerParameterValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequirementsGroup\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequirementsSpecification\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RuleVerification\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RuleVerificationList\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RuleViolation\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"SharedStyle\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"SimpleParameterizableThing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"SimpleParameterValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Stakeholder\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeholderValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMapSettings\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Thing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"UserRuleVerification\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ValueGroup\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActualFiniteState_PossibleState\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActualFiniteStateList_ExcludeOption\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ActualFiniteStateList_PossibleFiniteStateList\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"AndExpression_Term\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementBase_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementDefinition_OrganizationalParticipant\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementDefinition_ReferencedElement\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ElementUsage_ExcludeOption\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ExclusiveOrExpression_Term\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"File_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"FileRevision_FileType\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Goal_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"MultiRelationship_RelatedThing\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"NestedElement_ElementUsage\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Option_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"OrExpression_Term\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"PossibleFiniteStateList_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Publication_Domain\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Publication_PublishedParameter\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Relationship_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Requirement_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"RequirementsContainer_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Stakeholder_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Stakeholder_StakeholderValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeholderValue_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap_Goal\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap_Requirement\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap_StakeholderValue\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"StakeHolderValueMap_ValueGroup\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Thing_ExcludedDomain\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"Thing_ExcludedPerson\" {1} TRIGGER USER;", sourcePartition, triggerStatus);
                sqlBuilder.AppendFormat("ALTER TABLE \"{0}\".\"ValueGroup_Category\" {1} TRIGGER USER;", sourcePartition, triggerStatus);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }
        }
    }
}
