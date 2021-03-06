﻿/*
 * Copyright 2013 DigitasLBi Netherlands B.V.
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
using LBi.LostDoc.Repository.Web.Notifications;

namespace LBi.LostDoc.Repository.Web.Areas.Administration.Controllers
{
    public static class NotificationActions
    {
        public static NotificationAction Restart
        {
            get
            {
                return new NotificationAction("Restart now",
                                              new Uri("/lostdoc/system/restart?auth=" +
                                                      Uri.EscapeDataString(Guid.NewGuid().ToBase36String()),
                                                      UriKind.Relative));

                // TODO fix hardcoded URL ^
            }
        }

        public static NotificationAction Refresh
        {
            get
            {
                return new NotificationAction("Refresh page",
                                              new Uri("javascript:document.location.reload(true);",
                                                      UriKind.RelativeOrAbsolute));
            }
        }
    }
}