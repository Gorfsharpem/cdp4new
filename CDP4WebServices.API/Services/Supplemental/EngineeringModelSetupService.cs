﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelSetupService.cs" company="RHEA System S.A.">
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
    using System.Linq;

    using CDP4Common.DTO;

    using Helpers;

    using Npgsql;

    /// <summary>
    /// Extension for the code-generated <see cref="EngineeringModelSetupService"/>
    /// </summary>
    public partial class EngineeringModelSetupService
    {
        /// <summary>
        /// Get the <see cref="EngineeringModelSetup"/> associated to the <paramref name="engineeringModelId"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="engineeringModelId">The identifier of the associated <see cref="EngineeringModel"/></param>
        /// <returns>The requested <see cref="EngineeringModelSetup"/></returns>
        /// <remarks>
        /// The <see cref="EngineeringModelSetup"/> objects are read from the Cache Table and not from the View table
        /// </remarks>
        public EngineeringModelSetup GetEngineeringModelSetupFromDataBaseCache(NpgsqlTransaction transaction, Guid engineeringModelId)
        {
            return this.EngineeringModelSetupDao
                .Read(transaction, Cdp4TransactionManager.SITE_DIRECTORY_PARTITION, null, true)
                .FirstOrDefault(x => x.EngineeringModelIid == engineeringModelId);
        }
    }
}
