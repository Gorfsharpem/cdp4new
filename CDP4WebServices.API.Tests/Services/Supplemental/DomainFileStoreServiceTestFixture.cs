﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainFileStoreServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace CDP4WebServices.API.Tests.Services.Supplemental
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Security;

    using API.Services;
    using API.Services.Authorization;

    using CDP4Authentication;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CDP4Orm.Dao;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Authentication;

    using Moq;
    
    using Npgsql;
    
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="DomainFileStoreService"/>
    /// </summary>
    [TestFixture]
    public class DomainFileStoreServiceTestFixture
    {
        private static Folder folder = new Folder(Guid.NewGuid(), 0);
        private static File file = new File(Guid.NewGuid(), 0);
        private static DomainFileStore domainFileStore = new DomainFileStore(Guid.NewGuid(), 0);

        private DomainFileStoreService domainFileStoreService;
        private Mock<IPermissionService> permissionService;
        private Mock<IIterationService> iterationService;
        private Mock<IDomainFileStoreDao> domainFileStoreDao;
        private Mock<IDbTransaction> transaction;
        private Mock<ICdp4TransactionManager> transactionManager;
        private string iterationPartitionName;
        private Iteration iteration;

        [SetUp]
        public void Setup()
        {
            this.domainFileStoreDao = new Mock<IDomainFileStoreDao>();
            this.iterationService = new Mock<IIterationService>();
            this.permissionService = new Mock<IPermissionService>();
            this.transaction = new Mock<IDbTransaction>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();

            this.domainFileStoreService = new DomainFileStoreService
            {
                IterationService = this.iterationService.Object,
                PermissionService = this.permissionService.Object,
                DomainFileStoreDao = this.domainFileStoreDao.Object,
                TransactionManager = this.transactionManager.Object
            };

            this.iterationPartitionName = "Iteration_" + Guid.NewGuid();

            domainFileStore.IsHidden = false;

            domainFileStore.File.Clear();
            domainFileStore.File.Add(file.Iid);

            domainFileStore.Folder.Clear();
            domainFileStore.Folder.Add(folder.Iid);

            this.iteration = new Iteration(Guid.NewGuid(), 0)
            {
                DomainFileStore = new List<Guid>
                {
                    domainFileStore.Iid
                }
            };

            this.iterationService
                .Setup(x => x.GetActiveIteration(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>()))
                .Returns(this.iteration);

            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), domainFileStore)).Returns(true);
            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), folder)).Returns(true);
            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), file)).Returns(true);

            this.permissionService.Setup(x => x.Credentials)
                .Returns(
                    new Credentials
                    {
                        Person = new AuthenticationPerson(Guid.NewGuid(), 0)
                        {
                            UserName = "TestRunner"
                        }
                    });

            // Without a domainFileStoreSelector, SingleOrDefault(domainFileStoreSelector) could fail, because multiple <see cref="DomainFileStore"/>s could exist.
            // Also if a new DomainFileStore including Files and Folders are created in the same webservice call, then GetShallow for the new DomainFileStores might not return
            // the to be created <see cref="DomainFileStore"/>. The isHidden check will then be ignored.
            var extraDomainFileStoreToTestDomainFileStoreSelectors = new DomainFileStore(Guid.NewGuid(), 0);

            this.domainFileStoreDao
                .Setup(
                    x => x.Read(It.IsAny<NpgsqlTransaction>(), this.iterationPartitionName, It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>()))
                .Returns(new[] { domainFileStore, extraDomainFileStoreToTestDomainFileStoreSelectors });

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);
        }

        [Test]
        public void VerifyIsAllowedAccordingToIsHidden()
        {
            Assert.IsTrue(this.domainFileStoreService.IsAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore));

            domainFileStore.IsHidden = true;
            Assert.IsTrue(this.domainFileStoreService.IsAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore));

            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), domainFileStore)).Returns(false);
            Assert.IsFalse(this.domainFileStoreService.IsAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore));

            domainFileStore.IsHidden = false;
            Assert.IsTrue(this.domainFileStoreService.IsAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore));
        }

        [Test]
        public void VerifyCheckAllowedAccordingToIsHidden()
        {
            Assert.DoesNotThrow(() => this.domainFileStoreService.CheckAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore));

            domainFileStore.IsHidden = true;
            Assert.DoesNotThrow(() => this.domainFileStoreService.CheckAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore));

            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), domainFileStore)).Returns(false);
            Assert.Throws<Exception>(() => this.domainFileStoreService.CheckAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore));

            domainFileStore.IsHidden = false;
            Assert.DoesNotThrow(() => this.domainFileStoreService.CheckAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore));
        }

        [Test]
        [TestCaseSource(nameof(TestWriteCases))]
        public void VerifyHasReadAccess<T>(T thing, bool shouldFail) where T : Thing
        {
            if (shouldFail)
            {
                Assert.Throws<Cdp4ModelValidationException>(() => this.domainFileStoreService.HasReadAccess(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName));
            }
            else
            {
                Assert.That(this.domainFileStoreService.HasReadAccess(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Is.True);

                domainFileStore.IsHidden = true;

                Assert.That(() => this.domainFileStoreService.HasReadAccess(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Is.True);

                this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), domainFileStore)).Returns(false);

                Assert.That(this.domainFileStoreService.HasReadAccess(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Is.False);

                domainFileStore.IsHidden = false;

                Assert.That(this.domainFileStoreService.HasReadAccess(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Is.True);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestWriteCases))]
        public void VerifyCheckSecurity<T>(T thing, bool shouldFail) where T : Thing
        {
            if (shouldFail)
            {
                Assert.Throws<SecurityException>(() => this.domainFileStoreService.HasWriteAccess(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName));

                this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), It.IsAny<ElementDefinition>())).Returns(true);

                Assert.Throws<Cdp4ModelValidationException>(() => this.domainFileStoreService.HasWriteAccess(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName));
            }
            else
            {
                Assert.DoesNotThrow(() => this.domainFileStoreService.HasWriteAccess(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName));

                domainFileStore.IsHidden = true;

                Assert.DoesNotThrow(() => this.domainFileStoreService.HasWriteAccess(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName));

                this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), thing)).Returns(false);

                Assert.Throws<SecurityException>(() => this.domainFileStoreService.HasWriteAccess(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName));
            }
        }

        public static IEnumerable TestWriteCases()
        {
            yield return new object[] { file, false };
            yield return new object[] { folder, false };
            yield return new object[] { domainFileStore, false };
            yield return new object[] { new ElementDefinition(), true };
        }

        public static IEnumerable TestReadCases()
        {
            yield return new object[] { file, false };
            yield return new object[] { folder, false };
            yield return new object[] { domainFileStore, false };
            yield return new object[] { new ElementDefinition(), true };
        }
    }
}
