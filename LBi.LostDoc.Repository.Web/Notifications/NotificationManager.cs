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
using System.Linq;
using System.Web;

namespace LBi.LostDoc.Repository.Web.Notifications
{

    public class Notification
    {
        public Notification(Guid id, NotificationType type, LifeTime lifetime, string title, string message, IEnumerable<NotificationAction> actions)
        {
            this.Id = id;
            this.Type = type;
            this.LifeTime = lifetime;
            this.Title = title;
            this.Message = message;
            this.Actions = actions.ToArray();
        }

        protected NotificationType Type { get; set; }
        public Guid Id { get; protected set; }
        public LifeTime LifeTime { get; protected set; }
        public string Title { get; protected set; }
        public string Message { get; protected set; }
        public NotificationAction[] Actions { get; protected set; }
    }

    public class NotificationManager
    {

        private List<Notification> _notifications;

        // TODO 
        public NotificationManager()
        {
         this._notifications = new List<Notification>();   
        }

        public void Add(Guid id, NotificationType type, LifeTime lifeTime, string title, string message, params NotificationAction[] actions)
        {
            var note = new Notification(id, type, lifeTime, title, message, actions);
            this._notifications.Add(note);
        }


    }

    public class NotificationAction
    {
        public NotificationAction(string text, Uri target)
        {
            this.Text = text;
            this.Target = target;
        }

        protected Uri Target { get; set; }

        public string Text { get; set; }
    }

    public enum LifeTime
    {
        UntilRestart,
        PageLoad
    }

    public enum NotificationType
    {
        Action,
        Information,
        Warning,
        Error
    }
}