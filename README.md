<h1 align="center">🔐 SecureAuthAPI</h1>
<p align="center">
  An advanced, scalable, and secure RESTful API with JWT authentication, role-based access, and modern backend architecture.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/status-active-brightgreen.svg" alt="status"/>
  <img src="https://img.shields.io/github/license/username/SecureAuthAPI"/>
  <img src="https://img.shields.io/github/last-commit/username/SecureAuthAPI"/>
</p>

---

## ✨ Features

- 🔑 JWT Authentication & Refresh Tokens
- 🔐 Role-Based Access Control (RBAC)
- 🧠 Secure Password Hashing (bcrypt/scrypt)
- ⚙️ Configurable `IConfiguration`
- 📡 RESTful API Design

---

## 🖼️ Demo

<!-- Replace with actual video/gif/screenshot links -->
<p align="center">
  <img src="Screenshot 2025-07-16 115530.jpg" width="700" alt="App Demo"/>
  <img src="Screenshot 2025-07-16 115551.jpg" width="700" alt="App Demo"/>
</p>

---

## 🧰 Tech Stack

| Backend | Database | Auth | Tools |
|--------|----------|------|-------|
| .NET 8 | MariaDB  | JWT | Postman, Swagger |
| ASP.NET Core API | SQLite (dev) | Refresh Tokens | Git |

---
## ⚙️ API Endpoints

### 🔐 AuthController
| Method | Endpoint                | Description                    |
|--------|-------------------------|--------------------------------|
| POST   | `/api/Auth/login`       | Authenticates user, returns tokens |
| POST   | `/api/Auth/refresh`     | Issues new access/refresh tokens |
| POST   | `/api/Auth/logout`      | Logs user out, revokes token |

### 👤 UserController
| Method | Endpoint                    | Auth Role | Description                |
|--------|-----------------------------|-----------|----------------------------|
| GET    | `/api/UserController/dashboard` | User/Admin | Returns user dashboard |
| GET    | `/api/UserController/admin-only` | Admin      | Admin-only data         |

---

## 🧠 Token Storage Strategy

| Token Type      | Storage Location | Expiry     | Notes                                 |
|------------------|------------------|------------|---------------------------------------|
| Access Token     | HttpOnly Cookie  | 60 minutes | Used for most API requests            |
| Refresh Token    | HttpOnly Cookie + DB | 7 days     | Used to renew access token on expiry  |

---

## 🛡️ Backend Security Flow

1. On **login**, issue:
   - JWT access token (60 min)
   - Refresh token (stored in DB + sent in cookie)

2. On **API call**:
   - If access token valid → ✅ OK
   - If expired → frontend calls `/refresh`
   - If refresh token valid → ✅ new tokens issued
   - If invalid/revoked → ❌ force logout

3. On **logout**:
   - Refresh token revoked
   - Cookies cleared

---

## 🚀 Getting Started

```bash
# Clone the repo
git clone https://github.com/username/SecureAuthAPI.git
cd SecureAuthAPI

# Install dependencies
dotnet restore

# Configure your environment
cp .env.example .env
# Fill in .env with DB credentials, JWT secrets, etc.

# Run the application
dotnet run
