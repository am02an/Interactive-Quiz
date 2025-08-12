
# Quiz Project

## 📖 Overview
The **Unity Quiz Project** is an interactive, user-friendly quiz application built with Unity.  
It supports **dynamic question loading**, **random selection from JSON**, **real-time answer validation**, and **certificate generation with QR codes**.  
Perfect for educational quizzes, competitions, or fun trivia games.

---

## 🎯 Features

### **1. User Registration**
- Users provide:
  - **Name**
  - **Class**
  - **Mobile Number**
  - **Email**
- **Validation**:
  - Name & Class cannot be empty.
  - Mobile number must follow a valid format (e.g., 10 digits for India).
  - Email must follow a valid format (Regex check).
- Submit button only activates when **all inputs are valid**.
- Invalid inputs highlight **red** for quick feedback.

---

### **2. Quiz Flow**
- Questions are **loaded dynamically from a JSON file**.
- **Randomized** selection ensures a unique quiz experience every time.
- 5 questions per quiz session (configurable in code).
- Automatic transition to the **next question** after answer selection.
- At the end:
  - A **certificate** is generated with the user's details.
  - A **QR code** is embedded that links to the online certificate.

---

### **3. Certificate Handling**
- Certificates are generated as **PNG images**.
- QR code allows users to verify and view certificates online.
- Optional: Upload certificates to **Bunny.net** or any CDN for public access.

---

### **4. Email Integration**
- Certificates are automatically sent to the user via **SMTP email**.
- Supports **username/password authentication** for secure sending.
- Works with Gmail, Outlook, or any SMTP-compatible email provider.

---

## 🛠️ Setup Guide

### **Prerequisites**
- Unity **2021.3 LTS** or later
- .NET 4.x compatibility
- Internet connection for SMTP & CDN (optional)

---

### **Step 1: Import Required Plugins**
| Plugin / Package | Purpose |
|------------------|---------|
| **TextMeshPro** | High-quality text rendering & UI input fields |
| **Unity UI** | Buttons, panels, and layouts |
| **System.Text.RegularExpressions** | Email & mobile validation |
| **QR Code Generator** | Generates QR codes for certificates |
| **SMTP Mail Client** | Sends emails with attachments |
| **Bunny.net SDK (Optional)** | Uploads certificates to CDN |

---

### **Step 2: Prepare Question Bank**
1. Create a JSON file with your question pool:
```json
[
  {
    "question": "What is the capital of India?",
    "options": ["Delhi", "Mumbai", "Kolkata", "Chennai"],
    "answerIndex": 0
  },
  {
    "question": "2 + 2 = ?",
    "options": ["3", "4", "5", "2"],
    "answerIndex": 1
  }
]
```
2. Save it in `Assets/StreamingAssets/questions.json`.

---

### **Step 3: SMTP Email Setup**
- Go to your email provider and **enable SMTP access**.
- For Gmail:
  - Enable **2-Step Verification**.
  - Create an **App Password**.
- Update Unity's SMTP script with:
```csharp
string smtpHost = "smtp.gmail.com";
int smtpPort = 587;
string emailUser = "youremail@gmail.com";
string emailPass = "yourapppassword";
```

---

### **Step 4: QR Code & Certificate**
- Certificates are auto-generated after quiz completion.
- QR code is embedded using a QR generator library.
- If using **Bunny.net**:
  - Create a **Storage Zone**.
  - Use API keys to upload the certificate.

---

## 📦 Folder Structure
```
Assets/
 ├── Scripts/
 │    ├── Registration/
 │    ├── QuizLogic/
 │    ├── Certificate/
 │    └── Email/
 ├── Resources/
 │    ├── UI/
 │    └── Fonts/
 ├── StreamingAssets/
 │    └── questions.json
```

---

## 🖥️ Usage
1. Launch the Unity project.
2. Enter your details on the **registration screen**.
3. Take the quiz.
4. At the end:
   - View your generated certificate.
   - Scan the QR code to open it online.
   - Check your email for the attached certificate.

---

## 💡 Tips
- Keep questions diverse to make the quiz engaging.
- Test SMTP with a dummy email before production.
- Use `Debug.Log()` in Unity to trace any errors during certificate creation.

---

## 📜 License
This project is free for educational and personal use. For commercial usage, ensure all assets and libraries comply with their licenses.
