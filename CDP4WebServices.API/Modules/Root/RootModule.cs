﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootModule.cs" company="RHEA System S.A.">
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

namespace CDP4WebServices.API.Modules
{
    using System.Drawing.Imaging;
    using System.IO;

    using CDP4Common.DTO;

    using CDP4WebServices.API.Configuration;

    using Nancy;

    /// <summary>
    /// The root module handler.
    /// </summary>
    public class RootModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootModule"/> class.
        /// </summary>
        public RootModule()
        {
            this.Get["/"] = _ =>
                Properties.Resources.RootPage
                    .Replace("{{basePath}}", this.Request.Url.BasePath)
                    .Replace("{{sdkVersion}}", typeof(Thing).Assembly.GetName().Version.ToString())
                    .Replace("{{apiVersion}}", typeof(AppConfig).Assembly.GetName().Version.ToString());

            this.Get["/images/comet_logo"] = _ =>
            {
                var ms = new MemoryStream();
                Properties.Resources.comet_logo.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                return this.Response.FromStream(ms, "image/png");
            };

            this.Get["/images/rhea_logo"] = _ =>
            {
                var ms = new MemoryStream();
                Properties.Resources.rhea_logo.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                return this.Response.FromStream(ms, "image/png");
            };
        }
    }
}
