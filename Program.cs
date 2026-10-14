using System;
using wt_lab2_2oop_ostapchuk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wt_lab2_2oop_ostapchuk;

// ==================== ИНТЕРФЕЙСЫ ====================

public interface IRepository<T>
{
    void Add(T item);
    bool Remove(int id);
    T? GetById(int id);
    List<T> GetAll();
    int Count { get; }
}

public interface ISearchable
{
    List<Member> Search(string query);
}

// ==================== МОДЕЛИ ====================

public class Member
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = DateTime.Now;

    public override string ToString()
    {
        return $"[{Id}] {FullName} | Тел: {Phone} | Email: {Email}";
    }
}

public class Trainer
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }

    public override string ToString()
    {
        return $"[{Id}] {FullName} | {Specialization} | Опыт: {ExperienceYears} лет";
    }
}

public class Workout
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TrainerId { get; set; }
    public DateTime DateTime { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxParticipants { get; set; }
    public List<int> EnrolledMemberIds { get; set; } = new();

    public override string ToString()
    {
        return $"[{Id}] {Title} | Тренер #{TrainerId} | {DateTime:dd.MM.yyyy HH:mm} | {DurationMinutes} мин | {EnrolledMemberIds.Count}/{MaxParticipants}";
    }
}

public enum SubscriptionType { Monthly, Quarterly, Yearly, SingleVisit }

public class Subscription
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public SubscriptionType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    public override string ToString()
    {
        return $"[{Id}] Абонемент #{MemberId} | {Type} | {StartDate:dd.MM.yyyy}–{EndDate:dd.MM.yyyy} | {Price} руб";
    }
}

// ==================== СЕРВИСЫ ====================

public class Repository<T> : IRepository<T> where T : class
{
    private readonly List<T> _items = new();
    private int _nextId = 1;

    public int Count => _items.Count;

    public void Add(T item)
    {
        var idProp = typeof(T).GetProperty("Id");
        if (idProp != null) idProp.SetValue(item, _nextId++);
        _items.Add(item);
    }

    public bool Remove(int id)
    {
        var idProp = typeof(T).GetProperty("Id");
        if (idProp == null) return false;
        var item = _items.FirstOrDefault(x => (int)idProp.GetValue(x)! == id);
        if (item == null) return false;
        _items.Remove(item);
        return true;
    }

    public T? GetById(int id)
    {
        var idProp = typeof(T).GetProperty("Id");
        if (idProp == null) return null;
        return _items.FirstOrDefault(x => (int)idProp.GetValue(x)! == id);
    }

    public List<T> GetAll() => new(_items);
}

public class MemberService : IRepository<Member>, ISearchable
{
    private readonly Repository<Member> _repo = new();
    public int Count => _repo.Count;
    public void Add(Member item) => _repo.Add(item);
    public bool Remove(int id) => _repo.Remove(id);
    public Member? GetById(int id) => _repo.GetById(id);
    public List<Member> GetAll() => _repo.GetAll();

    public List<Member> Search(string query)
    {
        return _repo.GetAll()
            .Where(m => m.FullName.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || m.Phone.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || m.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}

public class WorkoutService : IRepository<Workout>
{
    private readonly Repository<Workout> _repo = new();
    public int Count => _repo.Count;
    public void Add(Workout item) => _repo.Add(item);
    public bool Remove(int id) => _repo.Remove(id);
    public Workout? GetById(int id) => _repo.GetById(id);
    public List<Workout> GetAll() => _repo.GetAll();

    public bool EnrollMember(int workoutId, int memberId)
    {
        var w = _repo.GetById(workoutId);
        if (w == null) return false;
        if (w.EnrolledMemberIds.Count >= w.MaxParticipants) return false;
        if (w.EnrolledMemberIds.Contains(memberId)) return false;
        w.EnrolledMemberIds.Add(memberId);
        return true;
    }
}

// ==================== ПРОГРАММА ====================

class Program
{
    static MemberService _members = new();
    static WorkoutService _workouts = new();

    static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        SeedData();
        PrintWelcome();

        bool exit = false;
        while (!exit)
        {
            PrintMenu();
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1": ShowMembers(); break;
                case "2": ShowWorkouts(); break;
                case "3": AddMember(); break;
                case "4": AddWorkout(); break;
                case "5": SearchMember(); break;
                case "6": Enroll(); break;
case "7": DemoBlock1(); break;
case "8": Level2Demo.Run(); break;
case "9": Level3Demo.Run(); break;
case "10": Level1Block2Demo.Run(); break;
case "11": Level2Block2Demo.Run(); break;
case "12": Level3Block2Demo.Run(); break;
case "13": await Level1Block3Demo.Run(); break;
case "14": await Level2Block3Demo.Run(); break;
case "15": await Level3Block3Demo.Run(); break;
case "0": exit = true; Console.WriteLine("\nДо свидания!"); break;
             default: Console.WriteLine("\nНеизвестная команда."); break;
            }

            if (!exit)
            {
                Console.WriteLine("\nНажмите Enter...");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    static void PrintWelcome()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║   Лабораторная работа 2 — ООП на C#          ║");
        Console.WriteLine("║   Автор: Fiola Ostapchuk                     ║");
        Console.WriteLine("║   Тема: Спортивный клуб / Фитнес-центр       ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine();
    }

    static void PrintMenu()
    {
        Console.WriteLine("─── ГЛАВНОЕ МЕНЮ ─────────────────────────────");
        Console.WriteLine("  1. Показать всех участников");
        Console.WriteLine("  2. Показать все тренировки");
        Console.WriteLine("  3. Добавить участника");
        Console.WriteLine("  4. Добавить тренировку");
        Console.WriteLine("  5. Поиск участника");
        Console.WriteLine("  6. Записать участника на тренировку");
Console.WriteLine("  7. [Уровень 1] Классы, объекты, конструкторы");
Console.WriteLine("  8. [Уровень 2] Инкапсуляция, абстракции, полиморфизм");
Console.WriteLine("  9. [Уровень 3] IItemService, перегрузка, protected");
Console.WriteLine(" 10. [Уровень 1 Блок 2] List<T> и foreach");
Console.WriteLine(" 11. [Уровень 2 Блок 2] Dictionary + TryGetValue");
Console.WriteLine(" 12. [Уровень 3 Блок 2] LINQ: Where/OrderBy/Select/GroupBy");
Console.WriteLine(" 13. [Уровень 1 Блок 3] async/await — асинхронность");
Console.WriteLine(" 14. [Уровень 2 Блок 3] async Task<List<T>> + try-catch-finally");
Console.WriteLine(" 15. [Уровень 3 Блок 3] Task.WhenAll + CancellationToken");Console.WriteLine("──────────────────────────────────────────────");
        Console.Write("Ваш выбор: ");
    }

    static void ShowMembers()
    {
        var list = _members.GetAll();
        Console.WriteLine($"\nВсего: {list.Count}\n");
        if (list.Count == 0) { Console.WriteLine("Список пуст."); return; }
        foreach (var m in list) Console.WriteLine(m);
    }

    static void ShowWorkouts()
    {
        var list = _workouts.GetAll();
        Console.WriteLine($"\nВсего: {list.Count}\n");
        if (list.Count == 0) { Console.WriteLine("Список пуст."); return; }
        foreach (var w in list) Console.WriteLine(w);
    }

    static void AddMember()
    {
        Console.Write("\nФИО: ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Телефон: ");
        string phone = Console.ReadLine() ?? "";
        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("ФИО пустое."); return; }
        _members.Add(new Member { FullName = name, Phone = phone, Email = email });
        Console.WriteLine("\n✅ Добавлено.");
    }

    static void AddWorkout()
    {
        Console.Write("\nНазвание: ");
        string title = Console.ReadLine() ?? "";
        Console.Write("Id тренера: ");
        int tid = int.TryParse(Console.ReadLine(), out var t) ? t : 0;
        Console.Write("Длительность (мин): ");
        int dur = int.TryParse(Console.ReadLine(), out var d) ? d : 60;
        Console.Write("Максимум участников: ");
        int max = int.TryParse(Console.ReadLine(), out var m) ? m : 20;
        if (string.IsNullOrWhiteSpace(title)) { Console.WriteLine("Название пустое."); return; }
        _workouts.Add(new Workout { Title = title, TrainerId = tid, DateTime = DateTime.Now.AddDays(1), DurationMinutes = dur, MaxParticipants = max });
        Console.WriteLine("\n✅ Добавлено.");
    }

    static void SearchMember()
    {
        Console.Write("\nПоиск: ");
        string q = Console.ReadLine() ?? "";
        var list = _members.Search(q);
        Console.WriteLine($"\nНайдено: {list.Count}\n");
        foreach (var m in list) Console.WriteLine(m);
    }

    static void Enroll()
    {
        Console.Write("\nId тренировки: ");
        int wid = int.TryParse(Console.ReadLine(), out var w) ? w : 0;
        Console.Write("Id участника: ");
        int mid = int.TryParse(Console.ReadLine(), out var m) ? m : 0;
        if (_workouts.EnrollMember(wid, mid))
            Console.WriteLine("\n✅ Записан.");
        else
            Console.WriteLine("\n❌ Не удалось.");
    }

    static void SeedData()
    {
        _members.Add(new Member { FullName = "Анна Смирнова", Phone = "+375 29 111-11-11", Email = "anna@mail.com" });
        _members.Add(new Member { FullName = "Пётр Иванов", Phone = "+375 29 222-22-22", Email = "petr@mail.com" });
        _members.Add(new Member { FullName = "Мария Козлова", Phone = "+375 29 333-33-33", Email = "maria@mail.com" });

        _workouts.Add(new Workout { Title = "Йога", TrainerId = 1, DateTime = DateTime.Now.AddDays(1), DurationMinutes = 60, MaxParticipants = 15 });
        _workouts.Add(new Workout { Title = "Кроссфит", TrainerId = 2, DateTime = DateTime.Now.AddDays(2), DurationMinutes = 45, MaxParticipants = 10 });
        _workouts.Add(new Workout { Title = "Пилатес", TrainerId = 1, DateTime = DateTime.Now.AddDays(3), DurationMinutes = 50, MaxParticipants = 12 });
    }
    // ==================== ДЕМО БЛОКА 1 ====================
    static void DemoBlock1()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║  ДЕМО БЛОКА 1: Классы, объекты, конструкторы ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine();

        // === 1. Создание через явный конструктор с параметрами ===
        Console.WriteLine("1) Создание через конструктор new Person(id, name, phone, email):");
        var p1 = new Person(1, "Иван Петров", "+375 29 000-00-00", "ivan@mail.com");
        p1.PrintInfo();

        // === 2. Создание через инициализатор объектов ===
        Console.WriteLine("2) Создание через инициализатор объектов new Person { ... }:");
        var p2 = new Person
        {
            Id = 2,
            FullName = "Анна Сидорова",
            Phone = "+375 29 111-11-11",
            Email = "anna@mail.com"
        };
        p2.PrintInfo();

        // === 3. Создание с частичным заполнением ===
        Console.WriteLine("3) Объект с частичным заполнением:");
        var p3 = new Person { Id = 3, FullName = "Пётр Смирнов" };
        p3.PrintInfo();

        // === 4. Массив объектов ===
        Console.WriteLine("4) Массив объектов Person:");
        var people = new Person[]
        {
            new Person(4, "Мария Козлова", "+375 29 333-33-33", "maria@mail.com"),
            new Person(5, "Сергей Волков", "+375 29 444-44-44", "sergey@mail.com"),
            new Person { Id = 6, FullName = "Ольга Морозова", Email = "olga@mail.com" }
        };

        for (int i = 0; i < people.Length; i++)
        {
            Console.WriteLine($"  --- Объект #{i + 1} ---");
            people[i].PrintInfo();
        }

        Console.WriteLine("=== Демонстрация Блока 1 завершена ===");
    }
}

// ==================== УРОВЕНЬ 2 БЛОКА 1 ====================

// === Абстрактный класс ===
public abstract class BaseEntity
{
public int Id { get; set; }
    public DateTime CreatedAt { get; private set; }

    protected BaseEntity()
    {
        CreatedAt = DateTime.Now;
    }

    public abstract string GetDescription();

    public virtual void PrintShort()
    {
        Console.WriteLine($"     [{Id}] создан {CreatedAt:dd.MM.yyyy HH:mm}");
    }
}

// === Интерфейс ===
public interface IStorable
{
    bool IsActive { get; }
    void Deactivate();
    void Activate();
}

// === Класс с инкапсуляцией ===
public class ClubMember : BaseEntity, IStorable
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // private set — только чтение снаружи
    public decimal Balance { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ClubMember() { }

    public ClubMember(string fullName, string phone, string email)
    {
        FullName = fullName;
        Phone = phone;
        Email = email;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма должна быть положительной.");
        Balance += amount;
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0 || amount > Balance) return false;
        Balance -= amount;
        return true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public override string GetDescription()
    {
        return $"Участник: {FullName} | Баланс: {Balance} руб | {(IsActive ? "активен" : "неактивен")}";
    }

    public override string ToString() => $"[{Id}] {FullName} | Баланс: {Balance} руб";
}

// === Наследник 1 ===
public class VipMember : ClubMember
{
    public int DiscountPercent { get; private set; } = 20;

    public VipMember() { }

    public VipMember(string fullName, string phone, string email, int discountPercent)
        : base(fullName, phone, email)
    {
        if (discountPercent < 0 || discountPercent > 50)
            throw new ArgumentException("Скидка должна быть 0–50%.");
        DiscountPercent = discountPercent;
    }

    public override string GetDescription()
    {
        return $"⭐ VIP: {FullName} | Скидка: {DiscountPercent}% | Баланс: {Balance} руб";
    }

    public void UpdateDiscount(int newPercent)
    {
        if (newPercent < 0 || newPercent > 50)
            throw new ArgumentException("Скидка должна быть 0–50%.");
        DiscountPercent = newPercent;
    }
}

// === Наследник 2 ===
public class TrainerEntity : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public decimal HourlyRate { get; private set; }

    public TrainerEntity() { }

    public TrainerEntity(string fullName, string specialization, decimal hourlyRate)
    {
        FullName = fullName;
        Specialization = specialization;
        HourlyRate = hourlyRate;
    }

    public void UpdatePrice(decimal newRate)
    {
        if (newRate <= 0)
            throw new ArgumentException("Ставка должна быть положительной.");
        if (newRate > 10000)
            throw new ArgumentException("Ставка слишком высокая (макс. 10000).");
        HourlyRate = newRate;
    }

    public override string GetDescription()
    {
        return $"Тренер: {FullName} | {Specialization} | {HourlyRate} руб/час";
    }
}

// === Демонстрация Уровня 2 ===
public static class Level2Demo
{
    public static void Run()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║  УРОВЕНЬ 2: Инкапсуляция, абстракции, ПОЛИ  ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine();

        // === 1. ИНКАПСУЛЯЦИЯ ===
        Console.WriteLine("1) ИНКАПСУЛЯЦИЯ — баланс меняется только через Deposit():");
        var m = new ClubMember("Анна Смирнова", "+375 29 111-11-11", "anna@mail.com") { Id = 1 };
        Console.WriteLine($"   Баланс изначально: {m.Balance} руб");

        m.Deposit(500);
        Console.WriteLine($"   После Deposit(500): {m.Balance} руб");
        m.Deposit(250);
        Console.WriteLine($"   После Deposit(250): {m.Balance} руб");

        try { m.Deposit(-100); }
        catch (ArgumentException ex) { Console.WriteLine($"   ❌ Deposit(-100): {ex.Message}"); }

        try { m.Deposit(0); }
        catch (ArgumentException ex) { Console.WriteLine($"   ❌ Deposit(0): {ex.Message}"); }
        Console.WriteLine();

        // === 2. ПОЛИМОРФИЗМ ===
        Console.WriteLine("2) ПОЛИМОРФИЗМ — массив BaseEntity[]:");
        BaseEntity[] entities = new BaseEntity[]
        {
            new ClubMember("Пётр Иванов", "+375 29 222-22-22", "petr@mail.com") { Id = 2 },
            new VipMember("Мария Козлова", "+375 29 333-33-33", "maria@mail.com", 25) { Id = 3 },
            new TrainerEntity("Сергей Волков", "Кроссфит", 1500m) { Id = 4 },
            new VipMember("Ольга Морозова", "+375 29 555-55-55", "olga@mail.com", 30) { Id = 5 }
        };

        foreach (var e in entities)
        {
            Console.WriteLine($"   [{e.GetType().Name}]");
            Console.WriteLine($"     {e.GetDescription()}");
            e.PrintShort();
        }
        Console.WriteLine();

        // === 3. ИНТЕРФЕЙС ===
        Console.WriteLine("3) РАБОТА ЧЕРЕЗ ИНТЕРФЕЙС IStorable:");
        IStorable[] cards = new IStorable[]
        {
            new ClubMember("Иван Петров", "+375 29 000-00-00", "ivan@mail.com") { Id = 10 },
            new VipMember("Светлана Иванова", "+375 29 999-99-99", "sveta@mail.com", 15) { Id = 11 }
        };

        foreach (var c in cards)
        {
            Console.WriteLine($"   IsActive: {c.IsActive}");
            c.Deactivate();
            Console.WriteLine($"   После Deactivate: {c.IsActive}");
            c.Activate();
            Console.WriteLine($"   После Activate: {c.IsActive}");
            Console.WriteLine();
        }

        // === 4. UPDATE С ВАЛИДАЦИЕЙ ===
        Console.WriteLine("4) UPDATE С ВАЛИДАЦИЕЙ (UpdatePrice/UpdateDiscount):");
        var trainer = new TrainerEntity("Игорь Кузнецов", "Йога", 800m) { Id = 20 };
        Console.WriteLine($"   {trainer.GetDescription()}");
        trainer.UpdatePrice(1000m);
        Console.WriteLine($"   После UpdatePrice(1000): {trainer.HourlyRate} руб/час");

        try { trainer.UpdatePrice(-500); }
        catch (ArgumentException ex) { Console.WriteLine($"   ❌ UpdatePrice(-500): {ex.Message}"); }

        try { trainer.UpdatePrice(99999); }
        catch (ArgumentException ex) { Console.WriteLine($"   ❌ UpdatePrice(99999): {ex.Message}"); }

        Console.WriteLine();
        Console.WriteLine("=== Демонстрация Уровня 2 завершена ===");
    }
}

// ==================== УРОВЕНЬ 3 БЛОКА 1 ====================

// === Интерфейс сервиса (без generic для надёжности) ===
public interface IItemService
{
    void Add(ClubMember item);
    bool Remove(int id);
    ClubMember GetById(int id);
    List<ClubMember> GetAll();
    int Count { get; }

    List<ClubMember> Find(string query);
    List<ClubMember> Find(string query, bool exactMatch);
    ClubMember Find(int id);
}

// === Реализация сервиса ===
public class ItemService : IItemService
{
    protected readonly List<ClubMember> _items = new();
    private int _nextId = 1;

    public int Count => _items.Count;

    public virtual void Add(ClubMember item)
    {
        item.Id = _nextId++;
        _items.Add(item);
    }

    public bool Remove(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item == null) return false;
        _items.Remove(item);
        return true;
    }

    public ClubMember GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    public List<ClubMember> GetAll() => new(_items);

    public List<ClubMember> Find(string query)
    {
        return _items.Where(x => SearchMatch(x, query, false)).ToList();
    }

    public List<ClubMember> Find(string query, bool exactMatch)
    {
        return _items.Where(x => SearchMatch(x, query, exactMatch)).ToList();
    }

    public ClubMember Find(int id) => GetById(id);

    protected virtual bool SearchMatch(ClubMember item, string query, bool exact)
    {
        string text = item.GetDescription();
        return exact
            ? text.Equals(query, StringComparison.OrdinalIgnoreCase)
            : text.Contains(query, StringComparison.OrdinalIgnoreCase);
    }
}

// === Наследник сервиса — демонстрация protected ===
public class VipItemService : ItemService
{
    public override void Add(ClubMember item)
    {
        Console.WriteLine("   [VipItemService] Добавление с логированием...");
        base.Add(item);
    }
}

// === Наследник ClubMember — демонстрация protected Balance ===
public class PremiumMember : ClubMember
{
    public PremiumMember() { }
    public PremiumMember(string f, string p, string e) : base(f, p, e) { }

    public bool PayFromBalance(decimal amount)
    {
        return Withdraw(amount);
    }

    public override string GetDescription()
    {
        return $"Премиум: {FullName} | Баланс: {Balance} руб";
    }
}

// === Демонстрация Уровня 3 ===
public static class Level3Demo
{
    public static void Run()
    {
        Console.Clear();
        Console.WriteLine("========= УРОВЕНЬ 3 БЛОКА 1 =========");
        Console.WriteLine("IItemService, перегрузка, protected");
        Console.WriteLine();

        // 1. IItemService через интерфейсную ссылку
        Console.WriteLine("1) Через интерфейсную ссылку IItemService:");
        IItemService service = new ItemService();
        service.Add(new ClubMember("Анна Смирнова", "+375 29 111-11-11", "anna@mail.com"));
        service.Add(new ClubMember("Пётр Иванов", "+375 29 222-22-22", "petr@mail.com"));
        service.Add(new PremiumMember("Мария Козлова", "+375 29 333-33-33", "maria@mail.com"));
        Console.WriteLine($"   Всего записей: {service.Count}");
        foreach (var m in service.GetAll())
            Console.WriteLine($"     {m.GetDescription()}");
        Console.WriteLine();

        // 2. Перегрузка Find
        Console.WriteLine("2) Перегрузка Find (3 варианта):");
        Console.WriteLine("   Find(\"Анна\") — поиск по подстроке:");
        foreach (var m in service.Find("Анна"))
            Console.WriteLine($"     {m.GetDescription()}");

        Console.WriteLine("   Find(..., true) — точное совпадение:");
        foreach (var m in service.Find("Премиум: Мария Козлова", true))
            Console.WriteLine($"     {m.GetDescription()}");

        Console.WriteLine("   Find(2) — по Id:");
        var byId = service.Find(2);
        if (byId != null) Console.WriteLine($"     {byId.GetDescription()}");
        Console.WriteLine();

        // 3. Наследник сервиса — protected SearchMatch
        Console.WriteLine("3) Наследник ItemService — VipItemService:");
        var vipService = new VipItemService();
        vipService.Add(new ClubMember("VIP Клиент", "+375 29 777-77-77", "vip@mail.com"));
        Console.WriteLine($"   Найдено через Find: {vipService.Find("VIP").Count}");
        Console.WriteLine();

        // 4. protected в иерархии ClubMember -> PremiumMember
        Console.WriteLine("4) protected в ClubMember -> PremiumMember:");
        var premium = new PremiumMember("Елена Премиум", "+375 29 888-88-88", "elena@mail.com");
        premium.Deposit(1000m);
        Console.WriteLine($"   Баланс после Deposit(1000): {premium.Balance} руб");

        bool paid = premium.PayFromBalance(300m);
        Console.WriteLine($"   PayFromBalance(300) = {paid}, остаток: {premium.Balance} руб");

        bool failed = premium.PayFromBalance(5000m);
        Console.WriteLine($"   PayFromBalance(5000) = {failed}, остаток: {premium.Balance} руб");
        Console.WriteLine();

        Console.WriteLine("=== Демонстрация Уровня 3 завершена ===");
    }
}

// ==================== БЛОК 2, УРОВЕНЬ 1 ====================

public static class Level1Block2Demo
{
    public static void Run()
    {
        Console.Clear();
        Console.WriteLine("======================================");
        Console.WriteLine("  БЛОК 2, УРОВЕНЬ 1: List<T> и foreach");
        Console.WriteLine("======================================");
        Console.WriteLine();

        // === 1. Объявление динамической коллекции List<T> ===
        List<ClubMember> members = new List<ClubMember>();

        // === 2. Наполнение списка 7 тестовыми объектами ===
        members.Add(new ClubMember("Анна Смирнова", "+375 29 111-11-11", "anna@mail.com"));
        members.Add(new ClubMember("Пётр Иванов", "+375 29 222-22-22", "petr@mail.com"));
        members.Add(new ClubMember("Мария Козлова", "+375 29 333-33-33", "maria@mail.com"));
        members.Add(new PremiumMember("Сергей Волков", "+375 29 444-44-44", "sergey@mail.com"));
        members.Add(new PremiumMember("Ольга Морозова", "+375 29 555-55-55", "olga@mail.com"));
        members.Add(new ClubMember("Иван Петров", "+375 29 666-66-66", "ivan@mail.com"));
        members.Add(new PremiumMember("Елена Соколова", "+375 29 777-77-77", "elena@mail.com"));

        // Заполним балансы у некоторых участников
        members[0].Deposit(500m);
        members[2].Deposit(1000m);
        members[4].Deposit(2500m);
        members[6].Deposit(1500m);

        // === 3. Итерация через foreach ===
        Console.WriteLine($"Всего участников в списке: {members.Count}");
        Console.WriteLine("--- Перебор коллекции через foreach ---");
        Console.WriteLine();

        int index = 1;
        foreach (ClubMember member in members)
        {
            Console.WriteLine($"{index}. {member.GetDescription()}");
            Console.WriteLine($"     Телефон: {member.Phone}, Email: {member.Email}");
            Console.WriteLine($"     Создан: {member.CreatedAt:dd.MM.yyyy HH:mm}");
            Console.WriteLine();
            index++;
        }

        // === 4. Дополнительно: суммарный баланс через foreach ===
        decimal totalBalance = 0m;
        foreach (var m in members)
        {
            totalBalance += m.Balance;
        }

        Console.WriteLine("--- Итоги ---");
        Console.WriteLine($"Общий баланс всех участников: {totalBalance} руб");
        Console.WriteLine($"Средний баланс: {totalBalance / members.Count:F2} руб");

        Console.WriteLine();
        Console.WriteLine("=== Демонстрация Блока 2, Уровня 1 завершена ===");
    }
}

// ==================== БЛОК 2, УРОВЕНЬ 2 ====================

public static class Level2Block2Demo
{
    public static void Run()
    {
        Console.Clear();
        Console.WriteLine("==========================================");
        Console.WriteLine("  БЛОК 2, УРОВЕНЬ 2: Dictionary + TryGetValue");
        Console.WriteLine("==========================================");
        Console.WriteLine();

        // === 1. Создание Dictionary<int, ClubMember> ===
        Dictionary<int, ClubMember> membersById = new Dictionary<int, ClubMember>();

        // === 2. Наполнение словаря ===
        var m1 = new ClubMember("Анна Смирнова", "+375 29 111-11-11", "anna@mail.com") { Id = 101 };
        var m2 = new ClubMember("Пётр Иванов", "+375 29 222-22-22", "petr@mail.com") { Id = 102 };
        var m3 = new PremiumMember("Мария Козлова", "+375 29 333-33-33", "maria@mail.com") { Id = 103 };
        var m4 = new ClubMember("Сергей Волков", "+375 29 444-44-44", "sergey@mail.com") { Id = 104 };
        var m5 = new PremiumMember("Ольга Морозова", "+375 29 555-55-55", "olga@mail.com") { Id = 105 };

        m1.Deposit(500m);
        m3.Deposit(1500m);
        m5.Deposit(2500m);

        membersById.Add(m1.Id, m1);
        membersById.Add(m2.Id, m2);
        membersById.Add(m3.Id, m3);
        membersById.Add(m4.Id, m4);
        membersById.Add(m5.Id, m5);

        Console.WriteLine($"Словарь создан. Записей: {membersById.Count}");
        Console.WriteLine();

        // === 3. Прямой доступ по ключу (индексатор) ===
        Console.WriteLine("1) ПРЯМОЙ ДОСТУП по ключу membersById[103]:");
        var direct = membersById[103];
        Console.WriteLine($"   {direct.GetDescription()}");
        Console.WriteLine();

        // === 4. Безопасный поиск через TryGetValue ===
        Console.WriteLine("2) БЕЗОПАСНЫЙ ПОИСК через TryGetValue:");
        Console.WriteLine();

        int[] idsToFind = { 101, 103, 999, 105, 777 };

        foreach (int id in idsToFind)
        {
            Console.WriteLine($"   Поиск Id = {id}:");
            if (membersById.TryGetValue(id, out ClubMember? found))
            {
                Console.WriteLine($"     ✅ Найден: {found.GetDescription()}");
            }
            else
            {
                Console.WriteLine($"     ❌ НЕ найден (Id {id} отсутствует в словаре)");
            }
            Console.WriteLine();
        }

        // === 5. Проверка наличия ключа через ContainsKey ===
        Console.WriteLine("3) ПРОВЕРКА через ContainsKey:");
        int checkId = 102;
        if (membersById.ContainsKey(checkId))
        {
            Console.WriteLine($"   Ключ {checkId} есть. Значение: {membersById[checkId].GetDescription()}");
        }
        else
        {
            Console.WriteLine($"   Ключа {checkId} нет.");
        }
        Console.WriteLine();

        // === 6. Перебор словаря (KeyValuePair) ===
        Console.WriteLine("4) ПЕРЕБОР словаря (KeyValuePair):");
        foreach (KeyValuePair<int, ClubMember> kvp in membersById)
        {
            Console.WriteLine($"   [{kvp.Key}] {kvp.Value.GetDescription()}");
        }
        Console.WriteLine();

        // === 7. Обновление и удаление ===
        Console.WriteLine("5) ОБНОВЛЕНИЕ и УДАЛЕНИЕ:");
        var m2Updated = new ClubMember("Пётр Иванов (обновлён)", "+375 29 222-22-22", "petr-new@mail.com") { Id = 102 };
        membersById[102] = m2Updated;
        Console.WriteLine($"   Id 102 обновлён: {membersById[102].GetDescription()}");

        bool removed = membersById.Remove(104);
        Console.WriteLine($"   Remove(104) = {removed}. Осталось записей: {membersById.Count}");
        Console.WriteLine();

        // === 8. Статистика по словарю ===
        Console.WriteLine("6) СТАТИСТИКА:");
        decimal totalBalance = 0m;
        foreach (var kvp in membersById)
            totalBalance += kvp.Value.Balance;

        Console.WriteLine($"   Всего участников: {membersById.Count}");
        Console.WriteLine($"   Общий баланс: {totalBalance} руб");
        Console.WriteLine();

        Console.WriteLine("=== Демонстрация Блока 2, Уровня 2 завершена ===");
    }
}

// ==================== БЛОК 2, УРОВЕНЬ 3 ====================

public static class Level3Block2Demo
{
    public static void Run()
    {
        Console.Clear();
        Console.WriteLine("====================================================");
        Console.WriteLine("  БЛОК 2, УРОВЕНЬ 3: LINQ (Method Syntax)");
        Console.WriteLine("====================================================");
        Console.WriteLine();

        // === Подготовка коллекции ===
        var members = new List<ClubMember>
        {
            new ClubMember("Анна Смирнова", "+375 29 111-11-11", "anna@mail.com") { Id = 1 },
            new ClubMember("Пётр Иванов", "+375 29 222-22-22", "petr@mail.com") { Id = 2 },
            new ClubMember("Мария Козлова", "+375 29 333-33-33", "maria@mail.com") { Id = 3 },
            new PremiumMember("Сергей Волков", "+375 29 444-44-44", "sergey@mail.com") { Id = 4 },
            new PremiumMember("Ольга Морозова", "+375 29 555-55-55", "olga@mail.com") { Id = 5 },
            new ClubMember("Иван Петров", "+375 29 666-66-66", "ivan@mail.com") { Id = 6 },
            new PremiumMember("Елена Соколова", "+375 29 777-77-77", "elena@mail.com") { Id = 7 },
            new ClubMember("Дмитрий Новиков", "+375 29 888-88-88", "dmitry@mail.com") { Id = 8 }
        };

        // Заполним балансы
        members[0].Deposit(500m);
        members[1].Deposit(1200m);
        members[2].Deposit(750m);
        members[3].Deposit(3000m);
        members[4].Deposit(2500m);
        members[5].Deposit(300m);
        members[6].Deposit(4000m);
        members[7].Deposit(0m);

        // ==================== 1. WHERE ====================
        Console.WriteLine("1) ФИЛЬТРАЦИЯ (Where):");
        Console.WriteLine("   Участники с балансом > 1000 руб:");
        var richMembers = members.Where(m => m.Balance > 1000m).ToList();
        foreach (var m in richMembers)
            Console.WriteLine($"     {m.GetDescription()}");
        Console.WriteLine();

        Console.WriteLine("   Только VIP-участники (PremiumMember):");
        var vips = members.Where(m => m is PremiumMember).ToList();
        foreach (var m in vips)
            Console.WriteLine($"     {m.GetDescription()}");
        Console.WriteLine();

        // ==================== 2. ORDERBY ====================
        Console.WriteLine("2) СОРТИРОВКА (OrderBy / OrderByDescending):");
        Console.WriteLine("   По балансу (по возрастанию):");
        var byBalanceAsc = members.OrderBy(m => m.Balance).ToList();
        foreach (var m in byBalanceAsc)
            Console.WriteLine($"     {m.Balance,6} руб | {m.FullName}");
        Console.WriteLine();

        Console.WriteLine("   По балансу (по убыванию):");
        var byBalanceDesc = members.OrderByDescending(m => m.Balance).ToList();
        foreach (var m in byBalanceDesc)
            Console.WriteLine($"     {m.Balance,6} руб | {m.FullName}");
        Console.WriteLine();

        // ==================== 3. SELECT ====================
        Console.WriteLine("3) ПРОЕКЦИЯ (Select):");
        Console.WriteLine("   Только имена и email:");
        var namesAndEmails = members
            .Select(m => new { m.FullName, m.Email })
            .ToList();
        foreach (var x in namesAndEmails)
            Console.WriteLine($"     {x.FullName,-25} | {x.Email}");
        Console.WriteLine();

        Console.WriteLine("   Краткая сводка (Id + ФИО + тип):");
        var summary = members
            .Select(m => $"[{m.Id}] {m.FullName} ({m.GetType().Name})")
            .ToList();
        foreach (var s in summary)
            Console.WriteLine($"     {s}");
        Console.WriteLine();

        // ==================== 4. АГРЕГАТЫ ====================
        Console.WriteLine("4) АГРЕГАТНЫЕ ОПЕРАЦИИ:");
        Console.WriteLine($"   Count  — всего участников: {members.Count}");
        Console.WriteLine($"   Sum    — общий баланс:     {members.Sum(m => m.Balance)} руб");
        Console.WriteLine($"   Average — средний баланс:  {members.Average(m => m.Balance):F2} руб");
        Console.WriteLine($"   Min    — минимальный:      {members.Min(m => m.Balance)} руб");
        Console.WriteLine($"   Max    — максимальный:     {members.Max(m => m.Balance)} руб");
        Console.WriteLine();

        Console.WriteLine("   Топ-3 по балансу (комбинация Where + OrderByDescending + Take):");
        var top3 = members
            .OrderByDescending(m => m.Balance)
            .Take(3)
            .ToList();
        foreach (var m in top3)
            Console.WriteLine($"     {m.Balance,6} руб | {m.FullName}");
        Console.WriteLine();

        // ==================== 5. GROUPBY ====================
        Console.WriteLine("5) ГРУППИРОВКА (GroupBy):");

        // Группировка по типу участника
        var byType = members
            .GroupBy(m => m.GetType().Name)
            .Select(g => new
            {
                TypeName = g.Key,
                Count = g.Count(),
                TotalBalance = g.Sum(m => m.Balance),
                AvgBalance = g.Average(m => m.Balance)
            })
            .ToList();

        Console.WriteLine("   По типу участника:");
        foreach (var g in byType)
        {
            Console.WriteLine($"     --- {g.TypeName} ---");
            Console.WriteLine($"       Количество: {g.Count}");
            Console.WriteLine($"       Сумма балансов: {g.TotalBalance} руб");
            Console.WriteLine($"       Средний баланс: {g.AvgBalance:F2} руб");
        }
        Console.WriteLine();

        // Группировка по диапазону баланса
        var byBalanceRange = members
            .GroupBy(m => m.Balance switch
            {
                0 => "0 руб",
                < 500 => "< 500 руб",
                < 2000 => "500–2000 руб",
                _ => "> 2000 руб"
            })
            .Select(g => new { Range = g.Key, Count = g.Count() })
            .ToList();

        Console.WriteLine("   По диапазону баланса:");
        foreach (var g in byBalanceRange)
            Console.WriteLine($"     {g.Range,-15}: {g.Count} чел.");
        Console.WriteLine();

        // ==================== 6. КОМБИНАЦИЯ ====================
        Console.WriteLine("6) КОМБИНАЦИЯ Where + OrderBy + Select:");
        Console.WriteLine("   Активные VIP с балансом > 2000, отсортированные по имени:");
        var result = members
            .Where(m => m is PremiumMember)
            .Where(m => m.Balance > 2000m)
            .OrderBy(m => m.FullName)
            .Select(m => $"{m.FullName} — {m.Balance} руб")
            .ToList();

        if (result.Any())
        {
            foreach (var r in result)
                Console.WriteLine($"     {r}");
        }
        else
        {
            Console.WriteLine("     Ничего не найдено.");
        }
        Console.WriteLine();

        // ==================== 7. Any / All ====================
        Console.WriteLine("7) ПРОВЕРКА Any / All:");
        bool anyZero = members.Any(m => m.Balance == 0);
        Console.WriteLine($"   Есть ли участник с балансом 0? {anyZero}");

        bool allPositive = members.All(m => m.Balance >= 0);
        Console.WriteLine($"   Все балансы неотрицательные? {allPositive}");
        Console.WriteLine();

        Console.WriteLine("=== Демонстрация Блока 2, Уровня 3 завершена ===");
    }
}

// ==================== БЛОК 3, УРОВЕНЬ 1 ====================

public static class Level1Block3Demo
{
    public static async Task Run()
    {
        Console.Clear();
        Console.WriteLine("==============================================");
        Console.WriteLine("  БЛОК 3, УРОВЕНЬ 1: async / await");
        Console.WriteLine("==============================================");
        Console.WriteLine();

        // === 1. Одиночный асинхронный вызов ===
        Console.WriteLine("1) ОДИНОЧНЫЙ вызов асинхронного метода:");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        string result = await LoadDataAsync("Сервер БД");
        Console.WriteLine($"   Результат: {result}");
        Console.WriteLine($"   Прошло времени: {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine();

        // === 2. Несколько асинхронных операций ===
        Console.WriteLine("2) ПАРАЛЛЕЛЬНЫЕ вызовы (Task.WhenAll):");
        stopwatch.Restart();

        Task<string> task1 = LoadDataAsync("Источник A");
        Task<string> task2 = LoadDataAsync("Источник B");
        Task<string> task3 = LoadDataAsync("Источник C");

        string[] results = await Task.WhenAll(task1, task2, task3);

        Console.WriteLine($"   Все результаты:");
        foreach (var r in results)
            Console.WriteLine($"     • {r}");
        Console.WriteLine($"   Прошло времени (3 задачи параллельно): {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine();

        // === 3. Последовательные вызовы (для сравнения) ===
        Console.WriteLine("3) ПОСЛЕДОВАТЕЛЬНЫЕ вызовы:");
        stopwatch.Restart();

        await LoadDataAsync("Источник D");
        await LoadDataAsync("Источник E");
        await LoadDataAsync("Источник F");

        Console.WriteLine($"   Прошло времени (3 задачи последовательно): {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine();

        // === 4. Асинхронная обработка коллекции ===
        Console.WriteLine("4) АСИНХРОННАЯ обработка List<ClubMember>:");
        var members = new List<ClubMember>
        {
            new ClubMember("Анна Смирнова", "+375 29 111-11-11", "anna@mail.com") { Id = 1 },
            new ClubMember("Пётр Иванов", "+375 29 222-22-22", "petr@mail.com") { Id = 2 },
            new PremiumMember("Мария Козлова", "+375 29 333-33-33", "maria@mail.com") { Id = 3 }
        };

        stopwatch.Restart();
        int processed = await ProcessMembersAsync(members);
        Console.WriteLine($"   Обработано участников: {processed}");
        Console.WriteLine($"   Прошло времени: {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine();

        // === 5. Асинхронный метод с возвратом значения ===
        Console.WriteLine("5) Асинхронный метод с возвратом значения:");
        decimal totalDeposits = await CalculateTotalAsync(members);
        Console.WriteLine($"   Итоговая сумма депозитов: {totalDeposits} руб");
        Console.WriteLine();

        // === 6. Try/catch с async ===
        Console.WriteLine("6) ОБРАБОТКА ОШИБОК в async:");
        try
        {
            await FailingOperationAsync();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"   ❌ Поймано исключение: {ex.Message}");
        }
        Console.WriteLine();

        Console.WriteLine("=== Демонстрация Блока 3, Уровня 1 завершена ===");
        Console.WriteLine($"Общее время выполнения: {stopwatch.ElapsedMilliseconds} мс (только последний этап)");
    }

    // === Асинхронный метод, имитирующий длительную операцию ===
    private static async Task<string> LoadDataAsync(string source)
    {
        Console.WriteLine($"   [START] Загрузка из {source}...");

        // Имитация длительной операции (1 секунда)
        await Task.Delay(1000);

        Console.WriteLine($"   [DONE]  Загрузка из {source} завершена");
        return $"Данные из {source}";
    }

    // === Асинхронная обработка списка ===
    private static async Task<int> ProcessMembersAsync(List<ClubMember> members)
    {
        Console.WriteLine($"   Начало обработки {members.Count} участников...");
        foreach (var m in members)
        {
            await Task.Delay(300);
            Console.WriteLine($"      обработан: {m.FullName} (баланс: {m.Balance} руб)");
        }
        return members.Count;
    }

    // === Асинхронный метод с расчётом ===
    private static async Task<decimal> CalculateTotalAsync(List<ClubMember> members)
    {
        Console.WriteLine("   Запуск расчёта...");
        await Task.Delay(500);

        foreach (var m in members)
            m.Deposit(100m);

        await Task.Delay(500);
        return members.Sum(m => m.Balance);
    }

    // === Метод, который выбрасывает исключение ===
    private static async Task FailingOperationAsync()
    {
        Console.WriteLine("   Запуск операции, которая упадёт...");
        await Task.Delay(500);
        throw new InvalidOperationException("Что-то пошло не так в асинхронной операции!");
    }
}

// ==================== БЛОК 3, УРОВЕНЬ 2 ====================

public static class Level2Block3Demo
{
    // Флаг для имитации сбоя (для try-catch)
    private static bool _simulateError = false;

    public static async Task Run()
    {
        Console.Clear();
        Console.WriteLine("====================================================");
        Console.WriteLine("  БЛОК 3, УРОВЕНЬ 2: async Task<List<T>> + try-catch");
        Console.WriteLine("====================================================");
        Console.WriteLine();

        // === 1. Успешная загрузка коллекции ===
        Console.WriteLine("1) УСПЕШНАЯ загрузка списка участников:");
        _simulateError = false;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        List<ClubMember> members = await LoadMembersFromDatabaseAsync();

        Console.WriteLine($"   Получено из БД: {members.Count} участников");
        Console.WriteLine($"   Время: {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine();
        foreach (var m in members)
            Console.WriteLine($"     {m.GetDescription()}");
        Console.WriteLine();

        // === 2. Успешная загрузка с фильтрацией ===
        Console.WriteLine("2) ЗАГРУЗКА с фильтром (баланс > 1000):");
        var filtered = await LoadMembersFromDatabaseAsync(minBalance: 1000m);
        Console.WriteLine($"   Найдено: {filtered.Count} участников");
        foreach (var m in filtered)
            Console.WriteLine($"     {m.GetDescription()}");
        Console.WriteLine();

        // === 3. Загрузка с имитацией ошибки ===
        Console.WriteLine("3) ЗАГРУЗКА с ОШИБКОЙ (try-catch-finally):");
        _simulateError = true;

        try
        {
            Console.WriteLine("   [TRY] Пытаемся загрузить данные...");
            List<ClubMember> data = await LoadMembersFromDatabaseAsync();
            Console.WriteLine($"   [TRY] Успех: {data.Count} записей");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"   [CATCH] ❌ Ошибка InvalidOperationException: {ex.Message}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"   [CATCH] ❌ Ошибка TimeoutException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   [CATCH] ❌ Другая ошибка: {ex.GetType().Name}: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("   [FINALLY] ✅ Блок finally выполнен (закрытие соединения с БД)");
        }
        Console.WriteLine();

        // === 4. Загрузка из внешнего API (с таймаутом) ===
        Console.WriteLine("4) ЗАГРУЗКА из внешнего API с таймаутом:");
        try
        {
            List<ClubMember> apiData = await LoadMembersFromApiAsync(timeoutMs: 500);
            Console.WriteLine($"   Получено из API: {apiData.Count} записей");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"   [CATCH] ❌ {ex.Message}");
        }
        finally
        {
            Console.WriteLine("   [FINALLY] ✅ HTTP-соединение закрыто");
        }
        Console.WriteLine();

        // === 5. Параллельная загрузка из нескольких источников ===
        Console.WriteLine("5) ПАРАЛЛЕЛЬНАЯ загрузка из 3 источников:");
        _simulateError = false;

        stopwatch.Restart();
        Task<List<ClubMember>> task1 = LoadMembersFromDatabaseAsync();
        Task<List<ClubMember>> task2 = LoadMembersFromDatabaseAsync();
        Task<List<ClubMember>> task3 = LoadMembersFromApiAsync(1000);

        List<ClubMember>[] results = await Task.WhenAll(task1, task2, task3);

        int total = results.Sum(r => r.Count);
        Console.WriteLine($"   Итого загружено: {total} записей из 3 источников");
        Console.WriteLine($"   Время: {stopwatch.ElapsedMilliseconds} мс (параллельно)");
        Console.WriteLine();

        // === 6. Демонстрация finally при успехе ===
        Console.WriteLine("6) Демонстрация finally при успешной операции:");
        _simulateError = false;
        var result = await SafeLoadAsync();
        Console.WriteLine($"   Результат: {result}");
        Console.WriteLine();

        Console.WriteLine("=== Демонстрация Блока 3, Уровня 2 завершена ===");
    }

    // === async Task<List<T>> — имитация загрузки из БД ===
    private static async Task<List<ClubMember>> LoadMembersFromDatabaseAsync(decimal minBalance = 0m)
    {
        Console.WriteLine("      [БД] Подключение к базе данных...");
        await Task.Delay(800);   // имитация сетевой задержки

        try
        {
            if (_simulateError)
            {
                await Task.Delay(200);
                throw new InvalidOperationException("Соединение с БД потеряно (id=0x80004005)");
            }

            Console.WriteLine("      [БД] Выполнение SQL-запроса...");
            await Task.Delay(400);

            var members = new List<ClubMember>
            {
                new ClubMember("Анна Смирнова", "+375 29 111-11-11", "anna@mail.com") { Id = 1 },
                new ClubMember("Пётр Иванов", "+375 29 222-22-22", "petr@mail.com") { Id = 2 },
                new PremiumMember("Мария Козлова", "+375 29 333-33-33", "maria@mail.com") { Id = 3 },
                new ClubMember("Сергей Волков", "+375 29 444-44-44", "sergey@mail.com") { Id = 4 },
                new PremiumMember("Ольга Морозова", "+375 29 555-55-55", "olga@mail.com") { Id = 5 }
            };

            members[0].Deposit(500m);
            members[1].Deposit(1500m);
            members[2].Deposit(800m);
            members[3].Deposit(2000m);
            members[4].Deposit(3000m);

            var filtered = members.Where(m => m.Balance >= minBalance).ToList();
            Console.WriteLine($"      [БД] Получено {filtered.Count} записей");
            return filtered;
        }
        finally
        {
            await Task.Delay(100);
            Console.WriteLine("      [БД] Соединение закрыто (finally)");
        }
    }

    // === async Task<List<T>> — имитация загрузки из API с таймаутом ===
    private static async Task<List<ClubMember>> LoadMembersFromApiAsync(int timeoutMs = 1000)
    {
        Console.WriteLine($"      [API] GET /api/members (timeout={timeoutMs} мс)...");

        using var cts = new System.Threading.CancellationTokenSource(timeoutMs);

        try
        {
            await Task.Delay(2000, cts.Token);   // имитация долгого запроса
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"Превышен таймаут {timeoutMs} мс при обращении к API");
        }
        finally
        {
            Console.WriteLine("      [API] HTTP-соединение закрыто (finally)");
        }

        return new List<ClubMember>
        {
            new ClubMember("API User 1", "+375 29 000-00-01", "api1@mail.com") { Id = 100 },
            new ClubMember("API User 2", "+375 29 000-00-02", "api2@mail.com") { Id = 101 }
        };
    }

    // === Демонстрация try-finally при успехе ===
    private static async Task<string> SafeLoadAsync()
    {
        Console.WriteLine("      [SAFE] Начало загрузки...");
        try
        {
            await Task.Delay(300);
            Console.WriteLine("      [SAFE] Данные получены.");
            return "OK";
        }
        finally
        {
            Console.WriteLine("      [SAFE] Ресурсы освобождены (finally)");
        }
    }
}

// ==================== БЛОК 3, УРОВЕНЬ 3 ====================

public static class Level3Block3Demo
{
    public static async Task Run()
    {
        Console.Clear();
        Console.WriteLine("=======================================================");
        Console.WriteLine("  БЛОК 3, УРОВЕНЬ 3: Task.WhenAll + CancellationToken");
        Console.WriteLine("=======================================================");
        Console.WriteLine();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // ========== 1. Task.WhenAll — параллельные задачи ==========
        Console.WriteLine("1) PARALLEL: 4 независимые задачи через Task.WhenAll:");
        stopwatch.Restart();

        var taskA = LoadFromSourceAsync("База данных", 1000);
        var taskB = LoadFromSourceAsync("Внешний API", 1500);
        var taskC = LoadFromSourceAsync("Кэш Redis", 800);
        var taskD = LoadFromSourceAsync("Файловая система", 1200);

        string[] parallelResults = await Task.WhenAll(taskA, taskB, taskC, taskD);

        Console.WriteLine($"   Все задачи завершены за {stopwatch.ElapsedMilliseconds} мс (параллельно)");
        Console.WriteLine("   Результаты:");
        foreach (var r in parallelResults)
            Console.WriteLine($"     • {r}");
        Console.WriteLine();

        // ========== 2. Для сравнения — последовательно ==========
        Console.WriteLine("2) SEQUENTIAL (для сравнения):");
        stopwatch.Restart();

        await LoadFromSourceAsync("Источник 1", 1000);
        await LoadFromSourceAsync("Источник 2", 1500);
        await LoadFromSourceAsync("Источник 3", 800);

        Console.WriteLine($"   Все задачи завершены за {stopwatch.ElapsedMilliseconds} мс (последовательно)");
        Console.WriteLine();

        // ========== 3. WhenAll с возвратом разных значений ==========
        Console.WriteLine("3) WhenAll с РАЗНЫМИ типами возврата:");
        stopwatch.Restart();

        var taskCount = CountRecordsAsync();
        var taskSum = SumBalancesAsync();
        var taskTop = GetTopMemberAsync();

        await Task.WhenAll(taskCount, taskSum, taskTop);

        Console.WriteLine($"   Count участников: {taskCount.Result}");
        Console.WriteLine($"   Сумма балансов:   {taskSum.Result} руб");
        Console.WriteLine($"   Топ-участник:     {taskTop.Result}");
        Console.WriteLine($"   Общее время: {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine();

        // ========== 4. CancellationToken — ручная отмена ==========
        Console.WriteLine("4) ОТМЕНА операции через CancellationToken:");
        Console.WriteLine("   Запускаем долгую операцию (10 сек), отменим через 1,5 сек...");

        var cts = new System.Threading.CancellationTokenSource();
        var longTask = LongOperationAsync(cts.Token, "Импорт данных");

        // Автоматическая отмена через 1.5 секунды
        var canceller = Task.Run(async () =>
        {
            await Task.Delay(1500);
            Console.WriteLine("   >>> Вызываем CancellationTokenSource.Cancel()");
            cts.Cancel();
        });

        try
        {
            await longTask;
            Console.WriteLine("   Операция завершилась успешно (не должна — её отменили)");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("   ✅ Операция успешно отменена через CancellationToken");
        }
        finally
        {
            cts.Dispose();
            await canceller;
        }
        Console.WriteLine();

        // ========== 5. CancellationToken с таймаутом ==========
        Console.WriteLine("5) ОТМЕНА по ТАЙМАУТУ (CancelAfter):");

        using (var timeoutCts = new System.Threading.CancellationTokenSource())
        {
            timeoutCts.CancelAfter(1000);   // авто-отмена через 1 сек

            try
            {
                await LongOperationAsync(timeoutCts.Token, "Генерация отчёта");
                Console.WriteLine("   Отчёт сгенерирован");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("   ⏰ Операция отменена по таймауту 1000 мс");
            }
        }
        Console.WriteLine();

        // ========== 6. WhenAll + отмена всех задач ==========
        Console.WriteLine("6) Отмена ГРУППЫ задач через общий CancellationToken:");

        using (var groupCts = new System.Threading.CancellationTokenSource())
        {
            var tasks = new[]
            {
                LongOperationAsync(groupCts.Token, "Задача 1"),
                LongOperationAsync(groupCts.Token, "Задача 2"),
                LongOperationAsync(groupCts.Token, "Задача 3")
            };

            // Отменить через 1 секунду
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                Console.WriteLine("   >>> Cancel() всех задач");
                groupCts.Cancel();
            });

            try
            {
                await Task.WhenAll(tasks);
                Console.WriteLine("   Все задачи завершены");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("   ✅ Группа задач отменена одновременно");
            }
        }
        Console.WriteLine();

        // ========== 7. Итоговое сравнение времени ==========
        Console.WriteLine("7) ИТОГОВОЕ сравнение:");
        stopwatch.Restart();

        // Параллельно 3 задачи по 500 мс
        await Task.WhenAll(
            DelayedAsync(500, "Параллельно 1"),
            DelayedAsync(500, "Параллельно 2"),
            DelayedAsync(500, "Параллельно 3")
        );
        long parallelTime = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        await DelayedAsync(500, "Последовательно 1");
        await DelayedAsync(500, "Последовательно 2");
        await DelayedAsync(500, "Последовательно 3");
        long sequentialTime = stopwatch.ElapsedMilliseconds;

        Console.WriteLine($"   Параллельно  (3×500мс): {parallelTime} мс");
        Console.WriteLine($"   Последовательно (3×500мс): {sequentialTime} мс");
        Console.WriteLine($"   Ускорение: {(double)sequentialTime / parallelTime:F1}x");
        Console.WriteLine();

        Console.WriteLine("=== Демонстрация Блока 3, Уровня 3 завершена ===");
    }

    // === Имитация загрузки из источника ===
    private static async Task<string> LoadFromSourceAsync(string sourceName, int delayMs)
    {
        Console.WriteLine($"      [{sourceName}] START (delay={delayMs} мс)");
        await Task.Delay(delayMs);
        Console.WriteLine($"      [{sourceName}] DONE");
        return $"{sourceName}: 42 записи";
    }

    // === Три разных асинхронных операции для WhenAll с разными типами ===
    private static async Task<int> CountRecordsAsync()
    {
        await Task.Delay(700);
        return 157;
    }

    private static async Task<decimal> SumBalancesAsync()
    {
        await Task.Delay(900);
        return 12345.67m;
    }

    private static async Task<string> GetTopMemberAsync()
    {
        await Task.Delay(1100);
        return "Анна Смирнова";
    }

    // === Долгая операция с поддержкой отмены ===
    private static async Task LongOperationAsync(System.Threading.CancellationToken token, string name)
    {
        Console.WriteLine($"      [{name}] Начало (10 шагов по 1 сек)...");

        try
        {
            for (int i = 1; i <= 10; i++)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(1000, token);
                Console.WriteLine($"      [{name}] Шаг {i}/10 выполнен");
            }
            Console.WriteLine($"      [{name}] Завершено!");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"      [{name}] ⚠ Отменено на текущем шаге");
            throw;
        }
        finally
        {
            Console.WriteLine($"      [{name}] Освобождение ресурсов (finally)");
        }
    }

    // === Простая задержка с сообщением ===
    private static async Task DelayedAsync(int ms, string name)
    {
        await Task.Delay(ms);
        Console.WriteLine($"      [{name}] завершено за {ms} мс");
    }
}