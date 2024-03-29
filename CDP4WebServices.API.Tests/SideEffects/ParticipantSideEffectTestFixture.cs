﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantSideEffectTestFixture.cs" company="RHEA System S.A.">
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

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;

    using CDP4Common;

    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ParticipantSideEffect"/> class.
    /// </summary>
    [TestFixture]
    public class ParticipantSideEffectTestFixture
    {
        private const string SelectedDomainKey = "SelectedDomain";

        private Mock<ISecurityContext> securityContext;

        private NpgsqlTransaction npgsqlTransaction;

        private ClasslessDTO rawUpdateInfo;

        private ParticipantSideEffect participantSideEffect;

        [SetUp]
        public void SetUp()
        {
            this.securityContext = new Mock<ISecurityContext>();
            this.npgsqlTransaction = null;
            this.participantSideEffect = new ParticipantSideEffect();
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenInvalidOrNullSelectedDomain()
        {
            var participant = new CDP4Common.DTO.Participant();
            participant.Domain.Add(Guid.NewGuid());

            //null selected domain verification
            this.rawUpdateInfo = new ClasslessDTO()
            {
                { SelectedDomainKey, null }
            };

            Assert.Throws<InvalidOperationException>(
                () =>
                    this.participantSideEffect.BeforeUpdate(
                        participant,
                        null,
                        this.npgsqlTransaction,
                        "partition",
                        this.securityContext.Object,
                        this.rawUpdateInfo));

            //invalid selected domain verification
            this.rawUpdateInfo = new ClasslessDTO()
            {
                { SelectedDomainKey, default }
            };

            Assert.Throws<InvalidOperationException>(
                () =>
                    this.participantSideEffect.BeforeUpdate(
                        participant,
                        null,
                        this.npgsqlTransaction,
                        "partition",
                        this.securityContext.Object,
                        this.rawUpdateInfo));
        }
    }
}
