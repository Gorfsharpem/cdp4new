// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaInfoProvider.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System;

    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

    /// <summary>
    /// The purpose of the <see cref="MetaInfoProvider"/> is to provide
    /// metadata regarding any kind of <see cref="Thing"/>
    /// </summary>
    public class MetaInfoProvider : MetaDataProvider, IMetaInfoProvider
    {
        /// <summary>
        /// Returns a meta info instance based on the passed in <see cref="Thing"/>.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance.
        /// </param>
        /// <returns>
        /// A concrete meta info instance.
        /// </returns>
        /// <exception cref="TypeLoadException">
        /// If type name not supported
        /// </exception>
        public IMetaInfo GetMetaInfo(Thing thing)
        {
            var typeName = thing.GetType().Name;
            return this.GetMetaInfo(typeName);
        }
    }
}