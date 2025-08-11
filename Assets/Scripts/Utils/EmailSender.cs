using System.Net;
using System.Net.Mail;
using UnityEngine;

public class EmailSender
{
    public static void SendCertificate(string recipientEmail, string filePath)
    {
        string senderEmail = "aman0202casper@gmail.com";      // sender's email
        string senderPassword = "amanchauhan123@@";     // Gmail app password

        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            mail.To.Add(recipientEmail);
            mail.Subject = "Your Quiz Participation Certificate";
            mail.Body = "Dear Participant,\n\nHere is your certificate.\n\nRegards,\nQuiz Team";

            // Attach the certificate
            mail.Attachments.Add(new Attachment(filePath));

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtp.EnableSsl = true;

            smtp.Send(mail);
            Debug.Log("✅ Email sent successfully to " + recipientEmail);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Failed to send email: " + ex.Message);
        }
    }
}
