/*
 * Author:      熊哲
 * CreateTime:  1/16/2018 12:13:04 PM
 * Description:
 * 
*/
#if UNITY_IOS
using System;
using UnityEngine.iOS;
#elif UNITY_ANDROID
#endif

namespace EZFramework.UniSDK
{
    public class Notification : Base.Notification
    {
#if UNITY_IOS
        protected override void Start()
        {
            base.Start();
            NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
        }

        public override void ScheduleNotification(string message, int seconds)
        {
            ScheduleNotification(message, DateTime.Now.AddSeconds(seconds));
        }
        public override void ScheduleNotification(string message, DateTime time)
        {
            LocalNotification ntf = NewNotification(message, time);
            NotificationServices.ScheduleLocalNotification(ntf);
        }
        public override void RepeatNotification(string message, int hour, int minute, int second)
        {
            RepeatNotification(message, DateTime.Today.AddHours(hour).AddMinutes(minute).AddSeconds(second));
        }
        public override void RepeatNotification(string message, DateTime time)
        {
            LocalNotification ntf = NewNotification(message, time, CalendarUnit.Day);
            NotificationServices.ScheduleLocalNotification(ntf);
        }

        private LocalNotification NewNotification(string message, DateTime time)
        {
            LocalNotification ntf = new LocalNotification();
            ntf.fireDate = time;
            ntf.alertAction = UnityEngine.Application.productName;
            ntf.alertBody = message;
            ntf.soundName = LocalNotification.defaultSoundName;
            ntf.applicationIconBadgeNumber = NotificationServices.scheduledLocalNotifications.Length;
            ntf.hasAction = true;
            return ntf;
        }
        private LocalNotification NewNotification(string message, DateTime time, CalendarUnit interval)
        {
            LocalNotification ntf = NewNotification(message, time);
            ntf.repeatInterval = interval;
            return ntf;
        }

        public override void ResetBadgeNumber()
        {
            LocalNotification ntf = new LocalNotification();
            ntf.fireDate = DateTime.Now.AddSeconds(1);
            ntf.applicationIconBadgeNumber = -1;
            NotificationServices.ScheduleLocalNotification(ntf);
        }
        public override void ClearNotifications()
        {
            NotificationServices.CancelAllLocalNotifications();
            NotificationServices.ClearLocalNotifications();
        }
#elif UNITY_ANDROID
#endif
    }
}