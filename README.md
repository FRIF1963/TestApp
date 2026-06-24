# CompanyApp

Десктопное WPF-приложение для управления сотрудниками, контрагентами и заказами.

## Технологии

- **WPF + MVVM** (CommunityToolkit.Mvvm)
- **NHibernate** (FluentNHibernate) — ORM
- **MySQL / MariaDB** — СУБД
- **Microsoft.Extensions.DependencyInjection** — DI-контейнер

## Архитектура

```
CompanyApp.Domain          — сущности и enum
CompanyApp.Application   — сервисы, интерфейсы, валидация
CompanyApp.Infrastructure — NHibernate, репозитории, маппинги
CompanyApp.Wpf             — UI (Views, ViewModels)
```

## Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL или MariaDB (локально)

## Настройка базы данных

1. Создайте базу данных:

```sql
CREATE DATABASE company_app CHARACTER SET utf8mb4;
```

2. При необходимости измените строку подключения в `CompanyApp.Wpf/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Port=3306;Database=company_app;User=root;Password=;CharSet=utf8mb4;"
  }
}
```

Схема таблиц создаётся автоматически при первом запуске (NHibernate SchemaUpdate).

## Запуск

```bash
dotnet run --project CompanyApp.Wpf
```

## Функциональность

- Табличное отображение сущностей (вкладки: Сотрудники, Контрагенты, Заказы)
- Добавление, редактирование, просмотр в отдельных формах
- Удаление с проверкой ссылочной целостности
- Валидация данных (ФИО, ИНН, сумма заказа и др.)

## Сущности

| Сущность    | Поля |
|-------------|------|
| Сотрудник   | ФИО, Должность (Руководитель / Работник), Дата рождения |
| Контрагент  | Наименование, ИНН, Куратор (Сотрудник) |
| Заказ       | Дата, Сумма, Сотрудник, Контрагент |

