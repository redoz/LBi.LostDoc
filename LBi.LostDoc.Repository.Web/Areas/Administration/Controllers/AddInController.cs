﻿/*
 * Copyright 2013 LBi Netherlands B.V.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LBi.LostDoc.Repository.Web.Areas.Administration.Models;
using LBi.LostDoc.Packaging;
using LBi.LostDoc.Repository.Web.Extensibility;
using LBi.LostDoc.Repository.Web.Notifications;


namespace LBi.LostDoc.Repository.Web.Areas.Administration.Controllers
{
    public static class Groups
    {
        public const string Core = "__Core";
    }


    [AdminController("Add-ins", Group = Groups.Core, Order = 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddInController : Controller
    {
        // default action is to list all installed add-ins
        [AdminAction("Installed add-ins", IsDefault = true)]
        public ActionResult Index()
        {

            return View(new AddInOverviewModel
                            {
                                Title = "Installed Add-ins",
                                AddIns = App.Instance.AddIns
                                            .Select(pkg =>
                                                    new AddInModel
                                                        {
                                                            CanInstall = false,
                                                            CanUninstall = true,
                                                            CanUpdate = CheckForUpdates(pkg),
                                                            Package = pkg
                                                        }).ToArray()
                            });
        }



        [AdminAction("Repository", IsDefault = true)]
        public ActionResult Repository()
        {
            const int count = 10;
            AddInModel[] results = App.Instance.AddIns.Repository.Search(null, true, 0, count)
                                      .Select(pkg =>
                                              new AddInModel
                                                  {
                                                      CanInstall = !CheckInstalled(pkg),
                                                      CanUninstall = CheckInstalled(pkg),
                                                      CanUpdate = CheckInstalled(pkg) && this.CheckForUpdates(pkg),
                                                      Package = pkg
                                                  }).ToArray();

            return View(new SearchResultModel
                            {
                                Results = results,
                                NextOffset = results.Length == count ? count : (int?)null
                            });
        }

        public ActionResult Search(string terms, int offset = 0)
        {
            const int count = 10;
            AddInModel[] results = App.Instance.AddIns.Repository.Search(terms, true, offset, count)
                                      .Select(pkg =>
                                              new AddInModel
                                                  {
                                                      CanInstall = !CheckInstalled(pkg),
                                                      CanUninstall = CheckInstalled(pkg),
                                                      CanUpdate = CheckInstalled(pkg) && this.CheckForUpdates(pkg),
                                                      Package = pkg
                                                  }).ToArray();

            return View(new SearchResultModel
                            {
                                Results = results,
                                NextOffset = results.Length == count ? offset + results.Length : (int?)null
                            });
        }

        [HttpPost]
        public ActionResult Install([Bind(Prefix = "package-id")]string id, [Bind(Prefix = "package-version")]string version)
        {
            ActionResult ret;
            var pkg = App.Instance.AddIns.Repository.Get(id, version);
            if (pkg == null)
            {
                ret = new HttpStatusCodeResult(HttpStatusCode.NotFound,
                                               string.Format("Package (Id: '{0}', Version: '{1}') not found.",
                                                             id,
                                                             version));
            }
            else
            {
                // TODO handle errors
                PackageResult result = App.Instance.AddIns.Install(pkg);
                if (result == PackageResult.PendingRestart)
                {
                    App.Instance.Notifications.Add(
                        Severity.Warning,
                        Lifetime.Application,
                        Scope.Administration,
                        "Pending restart",
                        string.Format("Package {0} failed to install, another attempt will be made upon site restart.",
                                      pkg.Id),
                        NotificationActions.Restart);
                }

                ret = this.Redirect(Url.Action("Index"));
            }
            return ret;
        }

        [HttpPost]
        public ActionResult Uninstall([Bind(Prefix = "package-id")]string id, [Bind(Prefix = "package-version")]string version)
        {
            ActionResult ret;
            var pkg = App.Instance.AddIns.Get(id, version);
            if (pkg == null)
            {
                ret = new HttpStatusCodeResult(HttpStatusCode.NotFound,
                                               string.Format("Package (Id: '{0}', Version: '{1}') not found.",
                                                             id,
                                                             version));
            }
            else
            {
                // TODO handle errors
                PackageResult result = App.Instance.AddIns.Uninstall(pkg);
                if (result == PackageResult.PendingRestart)
                {
                    App.Instance.Notifications.Add(
                        Severity.Warning,
                        Lifetime.Application,
                        Scope.Administration,
                        "Pending restart",
                        string.Format("Package {0} failed to uninstall, another attempt will be made upon site restart.",
                                      pkg.Id),
                        NotificationActions.Restart);
                }
                ret = this.Redirect(Url.Action("Index"));
            }
            return ret;
        }

        [HttpPost]
        public ActionResult Update([Bind(Prefix = "package-id")]string id, [Bind(Prefix = "package-version")]string version)
        {
            ActionResult ret;
            var pkg = App.Instance.AddIns.Repository.Get(id, version);
            if (pkg == null)
            {
                ret = new HttpStatusCodeResult(HttpStatusCode.NotFound,
                                               string.Format("Package (Id: '{0}', Version: '{1}') not found.",
                                                             id,
                                                             version));
            }
            else
            {
                // TODO handle errors
                PackageResult result = App.Instance.AddIns.Update(pkg);
                if (result == PackageResult.PendingRestart)
                {
                    App.Instance.Notifications.Add(
                        Severity.Warning,
                        Lifetime.Application,
                        Scope.Administration,
                        "Pending restart",
                        string.Format("Package {0} failed to update, another attempt will be made upon site restart.",
                                      pkg.Id),
                        NotificationActions.Restart);
                }
                ret = this.Redirect(Url.Action("Index"));
            }
            return ret;

        }

        private bool CheckInstalled(AddInPackage pkg)
        {
            return App.Instance.AddIns.Contains(pkg);
        }

        private bool CheckForUpdates(AddInPackage pkg)
        {
            // TODO fix prerelase hack
            return App.Instance.AddIns.Repository.GetUpdate(pkg, true) != null;
        }
    }
}
