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

using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LBi.LostDoc.Packaging;
using LBi.LostDoc.Repository.Web.Areas.Administration.Models;
using LBi.LostDoc.Repository.Web.Extensibility;
using LBi.LostDoc.Repository.Web.Notifications;

namespace LBi.LostDoc.Repository.Web.Areas.Administration.Controllers
{
    // TODO the TEXT isn't translateable, but it's good enough for now
    [AdminController("addins", Text = "Add-ins", Group = Groups.Core, Order = 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddInController : Controller
    {
        // default action is to list all installed add-ins
        [HttpPost]
        public ActionResult Install([Bind(Prefix = "package-id")] string id, 
                                    [Bind(Prefix = "package-version")] string version)
        {
            ActionResult ret;
            var pkg = App.Instance.AddIns.Repository.Get(id, version);
            if (pkg == null)
            {
                string message = string.Format("Package (Id: '{0}', Version: '{1}') not found.", id, version);
                ret = new HttpStatusCodeResult(HttpStatusCode.NotFound, message);
            }
            else
            {
                // TODO handle errors
                PackageResult result = App.Instance.AddIns.Install(pkg);
                if (result == PackageResult.PendingRestart)
                {
                    string message =
                        string.Format("Package {0} failed to install, another attempt will be made upon site restart.", 
                                      pkg.Id);
                    App.Instance.Notifications.Add(Severity.Warning, 
                                                   Lifetime.Application, 
                                                   Scope.Administration, 
                                                   "Pending restart", 
                                                   message, 
                                                   NotificationActions.Restart);
                }

                ret = this.Redirect(this.Url.Action("Installed"));
            }

            return ret;
        }

        [AdminAction("installed", Text = "Installed", IsDefault = true)]
        public ActionResult Installed()
        {
            return this.View(new AddInOverviewModel
                                 {
                                     Title = "Manage Add-ins", 
                                     AddIns = App.Instance.AddIns
                                                 .Select(pkg =>
                                                         new AddInModel
                                                             {
                                                                 CanInstall = false, 
                                                                 CanUninstall = true, 
                                                                 CanUpdate = this.CheckForUpdates(pkg), 
                                                                 Package = pkg
                                                             }).ToArray()
                                 });
        }

        [AdminAction("repository", Text = "Online")]
        public ActionResult Repository()
        {
            const int COUNT = 10;
            AddInModel[] results = App.Instance.AddIns.Repository.Search(null, true, 0, COUNT)
                                      .Select(pkg =>
                                              new AddInModel
                                                  {
                                                      CanInstall = !this.CheckInstalled(pkg), 
                                                      CanUninstall = this.CheckInstalled(pkg), 
                                                      CanUpdate = this.CheckInstalled(pkg) && this.CheckForUpdates(pkg), 
                                                      Package = pkg
                                                  }).ToArray();

            return this.View(new SearchResultModel
                                 {
                                     Title = "Online Add-ins", 
                                     Results = results, 
                                     NextOffset = results.Length == COUNT ? COUNT : (int?)null
                                 });
        }

        public ActionResult Search(string terms, int offset = 0)
        {
            const int COUNT = 10;
            AddInModel[] results = App.Instance.AddIns.Repository.Search(terms, true, offset, COUNT)
                                      .Select(pkg =>
                                              new AddInModel
                                                  {
                                                      CanInstall = !this.CheckInstalled(pkg), 
                                                      CanUninstall = this.CheckInstalled(pkg), 
                                                      CanUpdate = this.CheckInstalled(pkg) && this.CheckForUpdates(pkg), 
                                                      Package = pkg
                                                  }).ToArray();

            return this.View(new SearchResultModel
                                 {
                                     Results = results, 
                                     NextOffset = results.Length == COUNT ? offset + results.Length : (int?)null
                                 });
        }

        [HttpPost]
        public ActionResult Uninstall([Bind(Prefix = "package-id")] string id, 
                                      [Bind(Prefix = "package-version")] string version)
        {
            ActionResult ret;
            var pkg = App.Instance.AddIns.Get(id, version);
            if (pkg == null)
            {
                string message = string.Format("Package (Id: '{0}', Version: '{1}') not found.", id, version);
                ret = new HttpStatusCodeResult(HttpStatusCode.NotFound, message);
            }
            else
            {
                // TODO handle errors
                PackageResult result = App.Instance.AddIns.Uninstall(pkg);
                if (result == PackageResult.PendingRestart)
                {
                    string message =
                        string.Format(
                            "Package {0} failed to uninstall, another attempt will be made upon site restart.", pkg.Id);
                    App.Instance.Notifications.Add(
                        Severity.Warning, 
                        Lifetime.Application, 
                        Scope.Administration, 
                        "Pending restart", 
                        message, 
                        NotificationActions.Restart);
                }

                ret = this.Redirect(this.Url.Action("Installed"));
            }

            return ret;
        }

        [HttpPost]
        public ActionResult Update([Bind(Prefix = "package-id")] string id, 
                                   [Bind(Prefix = "package-version")] string version)
        {
            ActionResult ret;
            var pkg = App.Instance.AddIns.Repository.Get(id, version);
            if (pkg == null)
            {
                string message = string.Format("Package (Id: '{0}', Version: '{1}') not found.", id, version);
                ret = new HttpStatusCodeResult(HttpStatusCode.NotFound, 
                                               message);
            }
            else
            {
                // TODO handle errors
                PackageResult result = App.Instance.AddIns.Update(pkg);
                if (result == PackageResult.PendingRestart)
                {
                    string message =
                        string.Format("Package {0} failed to update, another attempt will be made upon site restart.", 
                                      pkg.Id);
                    App.Instance.Notifications.Add(
                        Severity.Warning, 
                        Lifetime.Application, 
                        Scope.Administration, 
                        "Pending restart", 
                        message, 
                        NotificationActions.Restart);
                }

                ret = this.Redirect(this.Url.Action("Installed"));
            }

            return ret;
        }

        [HttpPost]
        public ActionResult UploadPackage(HttpPostedFileBase package)
        {
            //string target = Path.Combine(AppConfig.AddInPackagePath, Path.GetFileName(package.FileName));
            //package.SaveAs(target);
            //App.Instance.Container
            return null;
        }

        private bool CheckForUpdates(AddInPackage pkg)
        {
            // TODO fix prerelase hack
            return App.Instance.AddIns.Repository.GetUpdate(pkg, true) != null;
        }

        private bool CheckInstalled(AddInPackage pkg)
        {
            return App.Instance.AddIns.Contains(pkg);
        }
    }
}