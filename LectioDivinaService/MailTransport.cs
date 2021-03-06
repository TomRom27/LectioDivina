﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Mail;

using System.Text;
using System.Threading.Tasks;
using AE.Net.Mail;
using LectioDivina.Model;

namespace LectioDivina.Service
{
    public class MailTransport
    {
        public event EventHandler<NotificationEventArgs> Notification;

        public void SendMail(string subject, string body, string toEmail, string toName)
        {

            var fromAddress = new MailAddress(Properties.Settings.Default.SenderMail, "Autor");
            var toAddress = new MailAddress(toEmail, toName);
            string fromPassword = Properties.Settings.Default.SenderPwd;

            var smtp = new SmtpClient
            {
                Host = Properties.Settings.Default.MailSmtpHost, // "smtp.gmail.com",
                Port = Properties.Settings.Default.MaitSmtpPort, // 587,
                EnableSsl = Properties.Settings.Default.MailSmtpUseSSL, //true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (
                var message = new System.Net.Mail.MailMessage(fromAddress, toAddress) { Subject = subject, Body = body })
            {
                smtp.Send(message);
            }
            smtp.Dispose();
        }

        public const string LectioDivinaSubjectPrefix = "Lectio Divina";

        public void SendToPublisher(OneDayContemplation contemplation)
        {
            string subject = LectioDivinaSubjectPrefix + " " + contemplation.Day.DayOfWeek.ToString() + " " + contemplation.Day.ToShortDateString();
            string body = SerializationHelper.Serialize(contemplation);
            SendMail(subject, body, Properties.Settings.Default.MailAccount, "Wydawca");

            if (!String.IsNullOrEmpty(Properties.Settings.Default.MailLectioPublisher))
            {
                subject = "Autor wysłał tekst na " + Localization.Date2PlStr(contemplation.Day) + " " +
                          Localization.Date2PlDayName(contemplation.Day);

                SendMail(subject, subject, Properties.Settings.Default.MailLectioPublisher, "Wydawca");
            }
        }


        public List<OneDayContemplation> RetrieveContemplations(DateTime? dayStart, DateTime? dayEnd)
        {
            OnNotification("Łączę się z serwerem");
            System.Diagnostics.Trace.WriteLine("Connecting to mail box");
            ImapClient ic = new ImapClient(
                Properties.Settings.Default.MailImapHost, //"imap.gmail.com", 
                Properties.Settings.Default.MailAccount, //"name@gmail.com", 
                Properties.Settings.Default.MailPwd, // "pass",
                AuthMethods.Login,
                Properties.Settings.Default.MailImapPort, // 993, 
                Properties.Settings.Default.MailImapUseSSL); //  true);

            System.Diagnostics.Trace.WriteLine("Select mail box");
            // Select a mailbox. Case-insensitive
            ic.SelectMailbox(Properties.Settings.Default.MailImapInboxName);

            string text = "Pobieram...";
            if (dayStart.HasValue)
                text = text + " od " + dayStart.Value.ToShortDateString();
            if (dayEnd.HasValue)
                text = text + " do " + dayEnd.Value.ToShortDateString();
            OnNotification(text);
            AE.Net.Mail.MailMessage[] messages = ic.GetMessages(0, Properties.Settings.Default.MailReadCount,
                headersonly: false);
            System.Diagnostics.Trace.WriteLine("Received messages:" + messages.Length.ToString());
            OnNotification("Wszystkich " + messages.Length);

            List<OneDayContemplation> list = new List<OneDayContemplation>();
            foreach (var message in messages)
            {
                OneDayContemplation contemplation = ConvertMessageToObject(message);
                if (contemplation != null)
                {
                    if ((!dayStart.HasValue || (dayStart.Value <= contemplation.Day)) &&
                        (!dayEnd.HasValue || (contemplation.Day <= dayEnd)))
                    {
                        OnNotification("Znalazłem " + contemplation.Day.ToString("yyyy.MM.dd"));
                        list.Add((contemplation));
                        ic.DeleteMessage(message);
                    }
                }
            }

            ic.Dispose();

            return list;
        }

        private OneDayContemplation ConvertMessageToObject(AE.Net.Mail.MailMessage message)
        {
            System.Diagnostics.Trace.WriteLine("ConvertMessageToObject:");
            System.Diagnostics.Trace.WriteLine("Message.Subject=" + message.Subject);
            System.Diagnostics.Trace.WriteLine("Message.Date =" + message.Date.ToString("yyyy-MM-dd hh:mm:ss"));
            System.Diagnostics.Trace.WriteLine("Message.Body =" + message.Body);
            System.Diagnostics.Trace.WriteLine("Message.Attachments.No =" + message.Attachments.Count);

            OneDayContemplation contemplation = null;
            string xml;

            if (message.Attachments.Count > 0)
            {
                // first we try with first attachment
                try
                {
                    System.Diagnostics.Trace.WriteLine("Trying to use attachment to get contemplation:");
                    xml = GetStringAttachment(message.Attachments.First());
                    contemplation = SerializationHelper.Deserialize<OneDayContemplation>(xml);

                    return contemplation;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("Tried to use attachment to get contemplation, but failed " + ex.Message);
                    // if the "attachment approach" failed, we continue with norma (body) approach
                    // that's only logging in the catch
                }
            }
            try
            {
                xml = message.Body;
                contemplation = SerializationHelper.Deserialize<OneDayContemplation>(xml);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
                // if deserialization of the whole message body fails, it may be 'cause of some additions anti-viruses are adding
                // so, we will try to find the right content;
                try
                {
                    xml = FindActualObject(message.Body);
                    contemplation = SerializationHelper.Deserialize<OneDayContemplation>(xml);
                }
                catch (Exception)
                {
                    System.Diagnostics.Trace.TraceError(ex.Message);
                    contemplation = null;
                }
            }

            return contemplation;
        }

        private string FindActualObject(string message)
        {
            string actualObject;
            const string objectStartTag = "<OneDayContemplation>"; // todo - from class name
            const string objectEndTag = "</OneDayContemplation>";

            var startTagPos = message.IndexOf(objectStartTag, StringComparison.InvariantCulture);
            var endTagPos = message.IndexOf(objectEndTag, StringComparison.InvariantCulture);
            if ((startTagPos >= 0) && (endTagPos >= 0))
                actualObject = message.Substring(startTagPos, endTagPos + objectEndTag.Length);
            else
                actualObject = "";

            return actualObject;
        }

        private void OnNotification(string notification)
        {
            if (Notification != null)
            {
                var args = new NotificationEventArgs(notification);
                Notification.BeginInvoke(this, args, null, null);
            }
        }


        private string GetStringAttachment(AE.Net.Mail.Attachment attachment)
        {
            return attachment.Body; 
        }


    }
}
