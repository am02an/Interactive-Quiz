# Creating the README text file with the given content

readme_content = """
# Quiz Project

## Overview
This Unity-based Quiz Project allows users to take quizzes with a clean and user-friendly interface. The quiz supports:
- Displaying questions dynamically.
- Random question selection from a question pool (JSON-based).
- Validating user answers.
- Showing certificates after quiz completion with QR code integration.
- Input validation on user registration forms.
- Email validation and mobile number validation.
- Error feedback for invalid inputs.
- Sending quiz certificates via email.

---

## Features & Flow

### User Registration
- User fills in Name, Class, Mobile Number, and Email fields.
- Each input is validated for format and completeness.
- Submit button activates only after all inputs are valid.
- Input fields highlight red if invalid.

### Quiz Flow
- Questions are randomly picked from a JSON question pool.
- User selects answers, and the system moves to the next question.
- After 5 questions, the quiz ends and generates a certificate.
- Certificate includes user info and QR code linking to the certificate image.

### Certificate Handling
- Certificates are generated as PNG images.
- QR code can be scanned to view certificate details.
- Optionally, certificate images can be uploaded to a CDN (e.g., Bunny.net).

### Email Sending
- Users receive their certificates via email.
- SMTP handling includes username/password authentication.

---

## Plugins and Packages Used

| Plugin/Package          | Purpose                                             |
|-------------------------|-----------------------------------------------------|
| **TextMeshPro**         | High-quality text rendering and input fields.       |
| **Unity UI**            | For buttons, input fields, and interface elements.  |
| **System.Text.RegularExpressions** | Email and mobile number validation using regex.     |
| **Bunny.net (optional)**| CDN for hosting certificate images (optional).      |
| **SMTP Mail Client**    | Sending certificate emails via SMTP.                |
| **QR Code Generator**   | To create QR codes for certificates (custom or asset). |

---

## Folder Structure (Suggested)

