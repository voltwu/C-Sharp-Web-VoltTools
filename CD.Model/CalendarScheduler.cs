using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace CD.Models
{
    public class CalendarScheduler
    {
        private Timer timer;
        private Task task;
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
            body = String.Format("Event: {0}{1}",body,foot);
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
            System.Globalization.ChineseLunisolarCalendar cal = new System.Globalization.ChineseLunisolarCalendar();

            return "<div style=\"margin-top: 50px; \">" +
                "<div><strong>Today</strong></div>" +
                $"<p><strong>Solar:</strong> {current.ToString("yyyy-MM-dd HH:mm:ss")}</p>" +
                $"<p><strong>Lunnar:</strong></p>" +
                $"<p>Time: {lunnar.ToString("yyyy-MM-dd HH:mm:ss")}</p>" +
                $"<p>Is a leap year: {cal.IsLeapYear(lunnar.Year)}</p>" +
                $"<p>Which month is the leap month in this year: {cal.GetLeapMonth(lunnar.Year)-1}</p>" +
                "</div>";
        }
        public DateTime ConvertSolarToLunnar(DateTime dt) {
            System.Globalization.ChineseLunisolarCalendar cal = new System.Globalization.ChineseLunisolarCalendar();
            int year = cal.GetYear(dt);
            int month = cal.GetMonth(dt);
            int dayOfMonth = cal.GetDayOfMonth(dt);
            int leapMonth = cal.GetLeapMonth(year);
            if (leapMonth != 0 && leapMonth <= month)
                month -= 1;
            return new DateTime(year,month,dayOfMonth);
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
        public String description { set; get; }
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
                if (Int32.TryParse(trigger_time.Split("_")[0],out int res))
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
