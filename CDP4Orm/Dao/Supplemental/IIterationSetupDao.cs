﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIterationSetupDao.cs" company="RHEA System S.A.">
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

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The IterationSetup Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IIterationSetupDao
    {
        /// <summary>
        /// Read the data from the database based on <see cref="IterationSetup.Iid"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iterationId">
        /// The iteration Id.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="IterationSetup"/>.
        /// </returns>
        IEnumerable<IterationSetup> ReadByIteration(NpgsqlTransaction transaction, string partition, Guid iterationId);

        /// <summary>
        /// Read the data from the database based on <see cref="EngineeringModelSetup.Iid"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="engineeringModelSetupId">
        /// The <see cref="EngineeringModelSetup"/> Id.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="IterationSetup"/>.
        /// </returns>
        IEnumerable<IterationSetup> ReadByEngineeringModelSetup(NpgsqlTransaction transaction, string partition, Guid engineeringModelSetupId);
    }
}
