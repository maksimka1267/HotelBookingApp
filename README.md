# 🏨 HotelBookingApp

**HotelBookingApp** — це міні-застосунок для **бронювання готелів**, створений на платформі **ASP.NET Core (Web API + Razor Pages)** з використанням **Identity** для реєстрації, авторизації та керування ролями користувачів.  
Проєкт розроблено як навчальний, відповідно до вимог тестового завдання.

---

## 📋 Опис проєкту

Мета застосунку — надати користувачеві можливість:
- переглядати список готелів та їхні номери;
- бронювати номери на обрані дати;
- керувати власними бронюваннями;
- виконувати реєстрацію, авторизацію та вихід із системи;
- адміністраторам — керувати списком готелів і номерів.

Проєкт побудовано за архітектурою **API + Razor Pages UI**:  
- API-контролери (`/api/...`) обробляють дані,  
- Razor Pages — відповідають за інтерфейс користувача.

---

## 🛠️ Використані технології

| Категорія | Технології |
|------------|-------------|
| **Back-end** | ASP.NET Core 8.0, Entity Framework Core, Identity |
| **Front-end** | Razor Pages, Bootstrap 5, JavaScript (fetch + toast повідомлення) |
| **База даних** | MySQL |
| **Автентифікація** | ASP.NET Core Identity (cookie-based) |
| **Інше** | Swagger UI для тестування API |

---

## 🧩 Структура проєкту

HotelBookingApp/
│
├── Controllers/
│ ├── AuthController.cs # API для реєстрації, логіну та логауту
│ ├── HotelsController.cs # API для керування готелями (Admin)
│ ├── RoomsController.cs # API для номерів (Admin)
│ └── BookingsController.cs # API для бронювань
│
├── Models/
│ ├── Users.cs # Модель користувача (IdentityUser + Name)
│ ├── Hotel.cs # Готель
│ ├── Room.cs # Номер у готелі
│ └── Booking.cs # Бронювання
│
├── Pages/
│ ├── Index.cshtml # Головна сторінка
│ ├── Account/ # Сторінки реєстрації, логіну, виходу
│ ├── Bookings/ # Перегляд бронювань користувача
│ └── Admin/ # Адмін-панель (готелі, номери)
│
├── Data/
│ └── ApplicationDbContext.cs # Контекст бази даних (EF Core)
│
├── wwwroot/
│ ├── css/site.css # Стилі інтерфейсу
│ ├── js/site.js # Основний JS (анімації, toast-и)
│ └── favicon.ico
│
├── Program.cs # Налаштування застосунку, Identity, DI, Swagger
└── README.md # Поточний файл опису

---

## 🚀 Функціональність

### 🔹 Для всіх користувачів
- Перегляд списку готелів і кімнат.
- Перегляд детальної інформації про готель.
- Реєстрація нового користувача.
- Вхід у систему.

### 🔹 Для зареєстрованих користувачів (Client)
- Створення бронювання на вибраний номер.
- Перегляд власних бронювань у розділі **My Bookings**.
- Вихід із системи (Logout).

### 🔹 Для адміністратора (Admin)
- Повний доступ до **панелі керування**:
  - додавання/редагування/видалення готелів;
  - керування номерами (Rooms);
  - перегляд усіх бронювань.

---

## 🔐 Авторизація та ролі

Після реєстрації:
- користувач автоматично отримує роль **Client**;
- якщо ролі `Admin` або `Client` не існують — вони створюються автоматично;
- адміністратор може бути доданий вручну через базу даних.

Система використовує **cookie-based Identity**, тому:
- не потрібен JWT токен;
- ASP.NET автоматично зберігає стан користувача;
- після входу `User.Identity.IsAuthenticated == true`.

---

## 💾 Налаштування бази даних

У файлі `appsettings.json` вкажіть свій рядок підключення MySQL:

"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=hotelbookingdb;user=root;password=YOUR_PASSWORD"
}

Роутинг
| URL                   | Опис               |
| --------------------- | ------------------ |
| `/`                   | Головна сторінка   |
| `/Account/Login`      | Вхід               |
| `/Account/Register`   | Реєстрація         |
| `/Bookings/Index`     | Мої бронювання     |
| `/Admin/Hotels/Index` | Керування готелями |
| `/Admin/Rooms/Index`  | Керування номерами |
| `/api/auth/register`  | API-реєстрація     |
| `/api/auth/login`     | API-логін          |
| `/api/auth/logout`    | API-вихід          |

Технічні особливості

Identity + Roles: використовується UserManager, SignInManager, RoleManager.

Logout: виконується через API /api/auth/logout + асинхронний JS у шапці.

UI: побудований на Razor Pages з адаптивною версткою Bootstrap 5.

JS: керує виходом, активними пунктами меню, анімацією, toast-повідомленнями.

SeedData: автоматично створює ролі та користувача адміністратора (якщо реалізовано).

Структура таблиць (EF Core)

Users — (Id, Name, Email, PasswordHash, RoleId)

Hotels — (Id, Name, Address, Description)

Rooms — (Id, HotelId, Number, PricePerNight, Capacity)

Bookings — (Id, RoomId, UserId, CheckIn, CheckOut, IsConfirmed)

Основні переваги проєкту

✅ Легкий у розгортанні
✅ Простий та адаптивний UI (Bootstrap 5)
✅ Повна підтримка ASP.NET Identity
✅ Розділення ролей Admin / Client
✅ Інтуїтивний API + Swagger
✅ Код придатний для навчання та продакшн-проєктів

Автор

Максим Чистіков
💼 Технології: ASP.NET Core, EF Core, MySQL, Bootstrap
📅 2025
