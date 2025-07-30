# 🧠 TaskerMaster · To-Do List Web App

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blue?logo=dotnet&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-InMemory-green?logo=dotnet)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-purple?logo=bootstrap)
![Status](https://img.shields.io/badge/Project%20Status-Active-brightgreen)
![License](https://img.shields.io/badge/License-MIT-lightgrey)

---

🎯 **TaskerMaster** е уеб приложение за управление на задачи (To-Do List), изградено с ASP.NET Core и Entity Framework. Приложението предоставя **динамична и интерактивна среда** за потребители и администратори, като включва следните ключови функционалности:

---

## 🚀 Основни функционалности

- ✅ **Регистрация и вход** с Identity
- 📝 **Създаване, редакция и изтриване** на задачи
- 🗂️ **Групиране на задачите по категории и приоритети**
- 📊 **Админ табло с графики и статистики** (Pie & Bar диаграми чрез Chart.js)
- 🎨 **Смяна на светла/тъмна тема** с локално запазване (Light/Dark mode)
- 🧩 **Модерен дизайн с Bootstrap 5**, икони, анимации и адаптивност
- 🛡️ **Ролева система** – само администраторът има достъп до админ таблото

---

## 📷 Превю

| Начална страница | Табло на администратора | Тъмна тема |
|------------------|-------------------------|------------|
| ![Home](docs/preview-home.png) | ![Admin](docs/preview-admin.png) | ![Dark](docs/preview-dark.png) |

---

## 📁 Структура на проекта

📦 to-do-list
┣ 📂Controllers
┣ 📂Models
┣ 📂Views
┃ ┣ 📂Admin
┃ ┣ 📂Categories
┃ ┗ 📂Shared
┣ 📂wwwroot
┃ ┣ 📂css (light-theme.css / dark-theme.css)
┃ ┣ 📂js
┗ Startup.cs / Program.cs
---

## ⚙️ Технологии

- **ASP.NET Core 8.0**
- **Entity Framework Core (InMemory / SQL Server)**
- **Bootstrap 5**
- **Chart.js**
- **Identity (User & Role management)**
- **Responsive Design**


