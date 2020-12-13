using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using VoltTools.Controllers;

namespace VoltTools.Models
{
    public class CalendarScheduler
    {
        private System.Threading.Timer timer;
        private CalendarConfiguration config;
        private IDatabase _IDataBase;
        private EmailSender _emailSender;
        public CalendarScheduler(IDatabase IDatabase, EmailSender emailSender)
        {
            _IDataBase = IDatabase;
            _emailSender = emailSender;
        }
        public async void StartSchedular() {
            config = await _IDataBase.GetCalendarConfiguration();

            SetUpTimer();
        }
        private void SetUpTimer()
        {
            TimeSpan timeToGo = config.AlertTime - DateTime.UtcNow.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                timeToGo = (new TimeSpan(1,0,0, 0))-DateTime.UtcNow.TimeOfDay;
                timeToGo = timeToGo.Add(config.AlertTime);
            }
            this.timer = new System.Threading.Timer(async x =>
            {
                await ExecuteTasks();
            }, null, timeToGo, new TimeSpan(1, 0, 0, 0));
        }
        private async Task ExecuteTasks() {
            while (await _IDataBase.IsIfHasAvailableCalendarReminderTask()) {
                await ExecuteTask();
                Sleep();
            }
        }
        private async Task ExecuteTask() {
            var cReminder = await _IDataBase.GetACalendarReminderAsync();
            var cResult = await _IDataBase.UpdateACalendarReminderAsync(cReminder.Id, DateTime.UtcNow);
            if (cResult.MatchedCount <= 0)
                return;
            HandACalendarReminderEvent(cReminder);
        }
        private void HandACalendarReminderEvent(calendarReminder crl) {
            switch (crl.Trigger_Type) {
                case TriggerType.Year:
                    triggerUpOnYear(crl);
                    break;
                case TriggerType.Month:
                    triggerUpOnMonth(crl);
                    break;
                case TriggerType.Day:
                    triggerUpOnDay(crl);
                    break;
            }
        }
        private void triggerUpOnDay(calendarReminder crl) {
            var current = DateTime.UtcNow.AddHours(crl.time_zone);
            if (crl.is_solar)
            {
                var target = new DateTime(current.Year, current.Month, crl.Trigger_Day);
                TimeSpan res = current - target;
                if (res.Days > -1 * (config.reminder_before)
                    && res.Days < config.reminder_after)
                {
                    sendEmail(crl.subject, crl.sender_name, crl.address, crl.events, GetFooterTimeInfo(crl.time_zone));
                }
                else
                {
                    target = target.AddMonths(1);
                    res = current - target;
                    if (res.Days > -1 * (config.reminder_before)
                        && res.Days < config.reminder_after)
                    {
                        sendEmail(crl.subject, crl.sender_name, crl.address, crl.events, GetFooterTimeInfo(crl.time_zone));
                    }
                }
            }
            if (crl.is_lunnar)
            {
                var current_lunnar = ConvertSolarToLunnar(current);
                var target = new DateTime(current_lunnar.Year, current_lunnar.Month, crl.Trigger_Day);
                TimeSpan res = current_lunnar - target;
                if (res.Days > -1 * (config.reminder_before)
                    && res.Days < config.reminder_after)
                {
                    sendEmail(crl.subject, crl.sender_name, crl.address, crl.events, GetFooterTimeInfo(crl.time_zone));
                }
                else
                {
                    target = target.AddMonths(1);
                    res = current_lunnar - target;
                    if (res.Days > -1 * (config.reminder_before)
                        && res.Days < config.reminder_after)
                    {
                        sendEmail(crl.subject, crl.sender_name, crl.address, crl.events, GetFooterTimeInfo(crl.time_zone));
                    }
                }
            }
        }
        private void triggerUpOnMonth(calendarReminder crl) {
            var current = DateTime.UtcNow.AddHours(crl.time_zone);
            if (crl.is_solar)
            {
                var target = new DateTime(current.Year, crl.Trigger_Month, crl.Trigger_Day);
                TimeSpan res = current - target;
                if (res.Days > -1 * (config.reminder_before)
                    && res.Days < config.reminder_after)
                {
                    sendEmail(crl.subject, crl.sender_name, crl.address, crl.events, GetFooterTimeInfo(crl.time_zone));
                }
                else {
                    target = target.AddYears(1);
                    res = current - target;
                    if (res.Days > -1 * (config.reminder_before)
                        && res.Days < config.reminder_after)
                    {
                        sendEmail(crl.subject, crl.sender_name, crl.address, crl.events, GetFooterTimeInfo(crl.time_zone));
                    }
                }
            }
            if (crl.is_lunnar)
            {
                var current_lunnar = ConvertSolarToLunnar(current);
                var target = new DateTime(current_lunnar.Year, crl.Trigger_Month, crl.Trigger_Day);
                TimeSpan res = current_lunnar - target;
                if (res.Days > -1 * (config.reminder_before)
                    && res.Days < config.reminder_after)
                {
                    sendEmail(crl.subject, crl.sender_name, crl.address, crl.events, GetFooterTimeInfo(crl.time_zone));
                }
                else {
                    target = target.AddYears(1);
                    res = current_lunnar - target;
                    if (res.Days > -1 * (config.reminder_before)
                        && res.Days < config.reminder_after)
                    {
                        sendEmail(crl.subject, crl.sender_name, crl.address, crl.events, GetFooterTimeInfo(crl.time_zone));
                    }
                }
            }
        }
        private void triggerUpOnYear(calendarReminder crl) {
            var current = DateTime.UtcNow.AddHours(crl.time_zone);
            if (crl.is_solar)
            {
                var target = new DateTime(crl.Trigger_Year, crl.Trigger_Month, crl.Trigger_Day);
                TimeSpan res = current - target;
                if (res.Days > -1 * (config.reminder_before)
                    && res.Days < config.reminder_after)
                {
                    sendEmail(crl.subject,crl.sender_name,crl.address, crl.events, GetFooterTimeInfo(crl.time_zone));
                }
            }
            if (crl.is_lunnar) {
                var current_lunnar = ConvertSolarToLunnar(current);
                var target = new DateTime(crl.Trigger_Year, crl.Trigger_Month, crl.Trigger_Day);
                TimeSpan res = current_lunnar - target;
                if (res.Days > -1 * (config.reminder_before)
                    && res.Days < config.reminder_after)
                {
                    sendEmail(crl.subject, crl.sender_name, crl.address, crl.events, GetFooterTimeInfo(crl.time_zone));
                }
            }
        }

        private void sendEmail(string subject, string sender_name,String emailAddresss, string body, string foot)
        {
            body += foot;
            Email email = new Email() {
                sendername = sender_name,
                body = body,
                subject = subject,
                address = emailAddresss
            };
            _emailSender.send(email);
        }

        public String GetFooterTimeInfo(int time_zone) {
            var current = DateTime.UtcNow.AddHours(time_zone);
            var lunnar = ConvertSolarToLunnar(current);
            return "<div>" +
                "Today:" +
                $"<p>solar time: {current.ToString("yyyy-mm-dd HH:mm:ss")}</p>" +
                $"<p>lunnar time: {lunnar.ToString("yyyy-mm-dd HH:mm:ss")}</p>" +
                "</div>";
        }
        public DateTime ConvertSolarToLunnar(DateTime dt) {
            System.Globalization.ChineseLunisolarCalendar cal = new System.Globalization.ChineseLunisolarCalendar();
            return new DateTime(cal.GetYear(dt)
                ,cal.GetMonth(dt)
                ,cal.GetDayOfMonth(dt)
                ,cal.GetHour(dt)
                ,cal.GetMinute(dt)
                ,cal.GetSecond(dt));
        }
        private void Sleep()
        {
            Thread.Sleep(new Random().Next(1, 5));
        }
    }
    public class CalendarConfiguration {
        public ObjectId Id { set; get; }
        public String alert_timer { set; get; }
        public int reminder_before { set; get; }
        public int reminder_after { set; get; }
        public TimeSpan AlertTime {
            get {
                String[] res = alert_timer.Split(":");
                int hours = res.Count()>0?Int32.Parse(res[0]):10;
                int minuts = res.Count() > 1 ? Int32.Parse(res[1]) : 0;
                int seconds = res.Count() > 2 ? Int32.Parse(res[2]) : 0;
                return new TimeSpan(hours,minuts,seconds);
            }
        }
    }
    public class calendarReminder {
        public ObjectId Id { set; get; }
        public String trigger_time { set; get; }
        public String events { set; get; }
        public int time_zone { set; get; }
        public String subject { set; get; }
        public String address { set; get; }
        public String sender_name { set; get; }
        public bool is_lunnar { set; get; }
        public bool is_solar { set; get; }
        public bool is_valid { set; get; }
        public DateTime last_executetime { set; get; }
        public TriggerType Trigger_Type {
            get {
                if (trigger_time.StartsWith("x"))
                {
                    return TriggerType.Year;
                }
                else {
                    var month = trigger_time.Split("_")[1];
                    if (Int32.TryParse(month, out int imonth)) {
                        return TriggerType.Month;
                    }
                    return TriggerType.Day;
                }
            }
        }
        public Int32 Trigger_Year {
            get {
                return Int32.Parse(trigger_time.Split("_")[0]);
            }
        }
        public Boolean Trigger_Year_Exists
        {
            get
            {
                return Int32.TryParse(trigger_time.Split("_")[0],out int ab);
            }
        }
        public Int32 Trigger_Month
        {
            get
            {
                return Int32.Parse(trigger_time.Split("_")[1]);
            }
        }
        public Boolean Trigger_Month_Exists
        {
            get
            {
                return Int32.TryParse(trigger_time.Split("_")[1], out int ab);
            }
        }
        public Int32 Trigger_Day
        {
            get
            {
                return Int32.Parse(trigger_time.Split("_")[2]);
            }
        }
        public Boolean Trigger_Day_Exists
        {
            get
            {
                return Int32.TryParse(trigger_time.Split("_")[2], out int ab);
            }
        }
        public List<MailAddress> MailAddresses
        {
            get
            {
                List<String> strs = JsonConvert.DeserializeObject<List<String>>(address);
                List<MailAddress> md = new List<MailAddress>();
                foreach (String ad in strs)
                    md.Add(new MailAddress(ad));
                return md;
            }
        }
    }
    public enum TriggerType { 
        Year,Month,Day
    }
}
