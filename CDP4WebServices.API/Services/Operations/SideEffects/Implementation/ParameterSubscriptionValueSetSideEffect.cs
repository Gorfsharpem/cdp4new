﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionValueSetSideEffect.cs" company="RHEA System S.A.">
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

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;

    using Authorization;

    using CDP4Common.DTO;

    using Npgsql;

    using ParameterSubscription = CDP4Common.DTO.ParameterSubscription;
    using ParameterSubscriptionValueSet = CDP4Common.DTO.ParameterSubscriptionValueSet;

    /// <summary>
    /// The purpose of the <see cref="ParameterSubscriptionValueSetSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterSubscriptionValueSetSideEffect : OperationSideEffect<ParameterSubscriptionValueSet>
    {
        /// <summary>
        /// Execute additional logic  before a create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override bool BeforeCreate(
            ParameterSubscriptionValueSet thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            return false;
        }


        /// <summary>
        /// Execute additional logic before a delete operation
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="container">The container of the <see cref="Thing"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        public override void BeforeDelete(ParameterSubscriptionValueSet thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            throw new InvalidOperationException("ParameterOverrideValueSet Cannot be deleted");
        }
    }
}