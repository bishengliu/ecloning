﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public static class LocalSMTP
    {
        public readonly static string Server = "smtp.nki.nl";
        public readonly static int Port = 25;
        public readonly static string Login = "";
        public readonly static string Password = "";
        public readonly static bool EnableSsl = false;


    }
}

/*
using MVCEmail.Models;
using System.Net;
using System.Net.Mail;


//in the controller

    if (ModelState.IsValid)
    {
        var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
        var message = new MailMessage();
        message.To.Add(new MailAddress("recipient@gmail.com"));  // replace with valid value 
        message.From = new MailAddress("sender@outlook.com");  // replace with valid value
        message.Subject = "Your email subject";
        message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
        message.IsBodyHtml = true;

        using (var smtp = new SmtpClient())
        {
            var credential = new NetworkCredential
            {
                UserName = "user@outlook.com",  // replace with valid value
                Password = "password"  // replace with valid value
            };
            smtp.Credentials = credential;
            smtp.Host = "smtp-mail.outlook.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(message);
            return RedirectToAction("Sent");
        }
    }

*/
