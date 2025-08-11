using UnityEngine;
using SuperSimple;

public class EmailServiceExample : MonoBehaviour
{
    void Start()
    {
        EmailService.Instance.Initialize(new EmailService.Settings()
        {
            SmtpHost = "smtp.gmail.com",
            EnableSSL = true,
            SenderEmail = "aman0202casper@gmail.com",
            SenderPassword = "zbtd ijee ndtz nupv",
            SenderName = "Aman Chauhan"
        });


        // Send email with the quiz certificate attached
    }
    public void SendMail(string recipientEmail, string attachmentPath)
    {
        EmailService.Instance.SendPlainText(new EmailService.Recipient[] {
        new EmailService.Recipient() {
            Address = recipientEmail,
            DisplayName = "Participant"
        }
    },
        "Your Quiz Certificate",  // Email subject
        "Dear Participant,\n\nPlease find your quiz certificate attached.\n\nBest regards,\nQuiz Team",  // Email body text
        new string[] {
        attachmentPath
        },
        (success) => {
            Debug.Log("Certificate email with attachment sent: " + success);
        });
    }


    void OnDestroy()
    {
        EmailService.Instance.Destroy();
    }
}
