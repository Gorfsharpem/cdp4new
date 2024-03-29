﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieSessionManager.cs" company="RHEA System S.A.">
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

namespace CDP4WebService.Authentication
{
    /// <summary>
    /// Manages session cookies.
    /// </summary>
    public class CookieSessionManager : ICookieSessionManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CookieSessionManager"/> class.
        /// </summary>
        public CookieSessionManager()
        {
            this.CreateCookieCache();
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public CookieSessionCache Cache { get; private set; }

        /// <summary>
        /// Reset the current Cache, by forcing dispose on the current instance.
        /// </summary>
        public void ResetCache()
        {
            this.Cache.Dispose();

            this.CreateCookieCache();
        }

        /// <summary>
        /// The create cookie cache.
        /// </summary>
        private void CreateCookieCache()
        {
            this.Cache = new CookieSessionCache("CDP4");
        }
    }
}
